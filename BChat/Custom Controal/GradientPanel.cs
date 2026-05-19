// =====================================================================
//  GradientPanel.cs — v13 (Fixed Cascading Between Nested GradientPanels)
//
//  ✅ إصلاح #1: OnLocationChanged → يمسح الـ Blur Cache عند التحريك
//  ✅ إصلاح #2: _isPaintingBackground → instance field (مش static) لمنع مشاركة الحالة
//  ✅ إصلاح #3: DrawSiblingsBehind → يتجاهل نفسه (sib == this)
//  ✅ إصلاح #4: try/catch في PaintSceneBackground و CaptureSceneBehind
//  ✅ إصلاح الحواف: PaintSceneBackground بدل GetSolidAncestorColor
//  ✅ إصلاح بكسل الزوايا: TextureBrush + FillPath بدل SetClip
//  ✅ Blur احترافي: Downscale → Blur → Upscale + Cache
//  ✅ Blur يرى الـ Siblings (كنترولات خلفنا)
//  ✅ Per-corner radius مستقل لكل زاوية
//  ✅ Alpha / rgba كاملة
//  ✅ إصلاح #5 (v13): _sceneCaptureActive [ThreadStatic] لمنع الـ Cascading
//       بين GradientPanels المتداخلة — يمنع translate مضاعف وتشوّه الرسم
// =====================================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BChat
{
    // ══════════════════════════════════════════════════════════════
    //  CornerRadiusEx — تخصيص كل زاوية بشكل مستقل
    // ══════════════════════════════════════════════════════════════
    [TypeConverter(typeof(CornerRadiusExConverter))]
    public class CornerRadiusEx
    {
        public int TopLeft { get; set; }
        public int TopRight { get; set; }
        public int BottomRight { get; set; }
        public int BottomLeft { get; set; }

        public CornerRadiusEx() { }
        public CornerRadiusEx(int all)
        { TopLeft = TopRight = BottomRight = BottomLeft = all; }
        public CornerRadiusEx(int tl, int tr, int br, int bl)
        { TopLeft = tl; TopRight = tr; BottomRight = br; BottomLeft = bl; }

        public override string ToString() => $"{TopLeft}, {TopRight}, {BottomRight}, {BottomLeft}";
    }

    public class CornerRadiusExConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext ctx, Type t)
            => t == typeof(string) || base.CanConvertFrom(ctx, t);

        public override object ConvertFrom(ITypeDescriptorContext ctx,
            System.Globalization.CultureInfo culture, object value)
        {
            if (value is string s)
            {
                var parts = s.Split(',');
                if (parts.Length == 4 &&
                    int.TryParse(parts[0].Trim(), out int tl) &&
                    int.TryParse(parts[1].Trim(), out int tr) &&
                    int.TryParse(parts[2].Trim(), out int br) &&
                    int.TryParse(parts[3].Trim(), out int bl))
                    return new CornerRadiusEx(tl, tr, br, bl);

                if (int.TryParse(s.Trim(), out int all))
                    return new CornerRadiusEx(all);
            }
            return base.ConvertFrom(ctx, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext ctx,
            System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is CornerRadiusEx cr)
                return cr.ToString();
            return base.ConvertTo(ctx, culture, value, destType);
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  GradientPanel
    // ══════════════════════════════════════════════════════════════
    [ToolboxItem(true)]
    [DefaultProperty("GradientStartColor")]
    [Description("بانل عصري مع Gradient وShadow ناعم وGlassmorphism Blur")]
    public class GradientPanel : Panel
    {
        // ─── Gradient ─────────────────────────────────────────────
        private Color _gradientStart = Color.FromArgb(56, 203, 180);
        private Color _gradientEnd = Color.FromArgb(35, 120, 220);
        private Color _gradientMid = Color.Empty;
        private float _gradientAngle = 135f;
        private bool _useThreeColors = false;

        // ─── Corner Radius ────────────────────────────────────────
        private int _cornerRadius = 22;
        private CornerRadiusEx _cornerRadiusEx = new CornerRadiusEx(22);

        // ─── Shadow ───────────────────────────────────────────────
        private bool _showShadow = true;
        private Color _shadowColor = Color.FromArgb(80, 20, 80, 180);
        private int _shadowRadius = 18;
        private int _shadowOffsetX = 0;
        private int _shadowOffsetY = 4;

        // ─── Hover Glow ───────────────────────────────────────────
        private bool _hoverGlow = true;
        private Color _hoverGlowColor = Color.FromArgb(120, 56, 203, 180);
        private int _hoverGlowRadius = 24;

        // ─── Appearance ───────────────────────────────────────────
        private bool _showShimmer = true;
        private int _shimmerOpacity = 40;
        private bool _showGlassBorder = true;
        private int _glassBorderAlpha = 45;

        // ─── Blur / Glass ─────────────────────────────────────────
        private bool _useBlur = false;
        private int _blurRadius = 10;

        // ─── Blur Cache ───────────────────────────────────────────
        private Bitmap? _blurCache;
        private Size _blurCacheSize = Size.Empty;
        private int _blurCacheRadius = -1;
        private Point _blurCacheLocation = new Point(-99999, -99999);

        // ─── State ────────────────────────────────────────────────
        private bool _isHovered = false;

        // ─── ✅ FIXED v12: instance field — كل بانل له حالته المستقلة
        private bool _isPaintingBackground;

        // ─── ✅ NEW v13: [ThreadStatic] — يمنع الـ Cascading بين GradientPanels المتداخلة
        //     عندما GradientPanel يكون فوق GradientPanel آخر (أب أو أخ)
        //     بدون هذا الـ Flag، الـ Parent.InvokePaint يستدعي PaintSceneBackground مرة ثانية
        //     مما يسبب translate مضاعف وتشوّه الرسم بين الأب والأولاد
        [ThreadStatic]
        private static bool _sceneCaptureActive;

        // ─── Win32 ────────────────────────────────────────────────
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ERASEBKGND = 0x0014;

        // ══════════════════════════════════════════════════════════
        //  CreateParams
        // ══════════════════════════════════════════════════════════
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style &= ~0x00800000;
                cp.ExStyle &= ~0x00000200;
                cp.ExStyle &= ~0x00020000;
                cp.ExStyle &= ~0x00000001;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCPAINT) { m.Result = IntPtr.Zero; return; }
            if (m.Msg == WM_ERASEBKGND) { m.Result = new IntPtr(1); return; }
            base.WndProc(ref m);
        }

        // ══════════════════════════════════════════════════════════
        //  Constructor
        // ══════════════════════════════════════════════════════════
        public GradientPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);

            UpdateStyles();
            BackColor = Color.Transparent;
            BorderStyle = BorderStyle.None;
            Size = new Size(300, 200);
            UpdatePadding();
        }

        // ══════════════════════════════════════════════════════════
        //  Properties — Gradient
        // ══════════════════════════════════════════════════════════
        [Category("✦ Gradient")]
        [Description("لون بداية الجراديينت")]
        public Color GradientStartColor
        { get => _gradientStart; set { _gradientStart = value; Invalidate(); } }

        [Category("✦ Gradient")]
        [Description("لون نهاية الجراديينت")]
        public Color GradientEndColor
        { get => _gradientEnd; set { _gradientEnd = value; Invalidate(); } }

        [Category("✦ Gradient")]
        [Description("لون وسط اختياري")]
        public Color GradientMidColor
        { get => _gradientMid; set { _gradientMid = value; Invalidate(); } }

        [Category("✦ Gradient")]
        [DefaultValue(false)]
        public bool UseThreeColors
        { get => _useThreeColors; set { _useThreeColors = value; Invalidate(); } }

        [Category("✦ Gradient")]
        [DefaultValue(135f)]
        public float GradientAngle
        { get => _gradientAngle; set { _gradientAngle = value % 360f; Invalidate(); } }

        // ══════════════════════════════════════════════════════════
        //  Properties — Corner Radius
        // ══════════════════════════════════════════════════════════
        [Category("✦ Corner")]
        [DefaultValue(22)]
        [Description("يضبط الـ 4 زوايا بنفس القيمة — للتخصيص المستقل استخدم CornerRadiusEx")]
        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = Math.Max(0, value);
                _cornerRadiusEx = new CornerRadiusEx(_cornerRadius);
                InvalidateBlurCache();
                Invalidate();
            }
        }

        [Category("✦ Corner")]
        [Description("تخصيص كل زاوية بشكل مستقل — TopLeft, TopRight, BottomRight, BottomLeft")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadiusEx CornerRadiusEx
        {
            get => _cornerRadiusEx;
            set { _cornerRadiusEx = value ?? new CornerRadiusEx(0); InvalidateBlurCache(); Invalidate(); }
        }

        // ══════════════════════════════════════════════════════════
        //  Properties — Shadow
        // ══════════════════════════════════════════════════════════
        [Category("✦ Shadow")]
        [DefaultValue(true)]
        public bool ShowShadow
        { get => _showShadow; set { _showShadow = value; UpdatePadding(); Invalidate(); } }

        [Category("✦ Shadow")]
        public Color ShadowColor
        { get => _shadowColor; set { _shadowColor = value; Invalidate(); } }

        [Category("✦ Shadow")]
        [DefaultValue(18)]
        public int ShadowRadius
        { get => _shadowRadius; set { _shadowRadius = Math.Max(0, value); UpdatePadding(); Invalidate(); } }

        [Category("✦ Shadow")]
        [DefaultValue(0)]
        public int ShadowOffsetX
        { get => _shadowOffsetX; set { _shadowOffsetX = value; UpdatePadding(); Invalidate(); } }

        [Category("✦ Shadow")]
        [DefaultValue(4)]
        public int ShadowOffsetY
        { get => _shadowOffsetY; set { _shadowOffsetY = value; UpdatePadding(); Invalidate(); } }

        [Category("✦ Shadow")]
        [DefaultValue(true)]
        public bool HoverGlow
        { get => _hoverGlow; set { _hoverGlow = value; Invalidate(); } }

        [Category("✦ Shadow")]
        public Color HoverGlowColor
        { get => _hoverGlowColor; set { _hoverGlowColor = value; Invalidate(); } }

        [Category("✦ Shadow")]
        [DefaultValue(24)]
        public int HoverGlowRadius
        { get => _hoverGlowRadius; set { _hoverGlowRadius = Math.Max(0, value); Invalidate(); } }

        // ══════════════════════════════════════════════════════════
        //  Properties — Appearance
        // ══════════════════════════════════════════════════════════
        [Category("✦ Appearance")]
        [DefaultValue(true)]
        public bool ShowShimmer
        { get => _showShimmer; set { _showShimmer = value; Invalidate(); } }

        [Category("✦ Appearance")]
        [DefaultValue(40)]
        public int ShimmerOpacity
        { get => _shimmerOpacity; set { _shimmerOpacity = Math.Clamp(value, 0, 120); Invalidate(); } }

        [Category("✦ Appearance")]
        [DefaultValue(true)]
        public bool ShowGlassBorder
        { get => _showGlassBorder; set { _showGlassBorder = value; Invalidate(); } }

        [Category("✦ Appearance")]
        [DefaultValue(45)]
        public int GlassBorderAlpha
        { get => _glassBorderAlpha; set { _glassBorderAlpha = Math.Clamp(value, 0, 255); Invalidate(); } }

        // ══════════════════════════════════════════════════════════
        //  Properties — Glass / Blur
        // ══════════════════════════════════════════════════════════
        [Category("✦ Glass")]
        [DefaultValue(false)]
        [Description("تفعيل الـ Blur خلف البانل (Glassmorphism)")]
        public bool UseBlur
        { get => _useBlur; set { _useBlur = value; InvalidateBlurCache(); Invalidate(); } }

        [Category("✦ Glass")]
        [DefaultValue(10)]
        [Description("شدة الضبابية: 1 = خفيف  |  40 = كثيف")]
        public int BlurRadius
        { get => _blurRadius; set { _blurRadius = Math.Clamp(value, 1, 40); InvalidateBlurCache(); Invalidate(); } }

        // ══════════════════════════════════════════════════════════
        //  Suppress default background
        // ══════════════════════════════════════════════════════════
        protected override void OnPaintBackground(PaintEventArgs e) { }

        // ══════════════════════════════════════════════════════════
        //  OnPaint
        // ══════════════════════════════════════════════════════════
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            var rc = GetCardRect();
            if (rc.Width <= 0 || rc.Height <= 0) { base.OnPaint(e); return; }

            using var path = BuildPath(rc, _cornerRadiusEx);

            // ── 1: امسح الكنترول كله بالمشهد الحقيقي خلفنا ────
            PaintSceneBackground(g);

            // ── 2: Blur مضبّب داخل الـ Path فقط ────────────────
            if (_useBlur)
                DrawBlurredBackground(g, path);

            // ── 3: الظل / Glow ───────────────────────────────────
            if (_showShadow)
            {
                bool isHov = _isHovered && _hoverGlow && !DesignMode;
                var glowC = isHov ? _hoverGlowColor : _shadowColor;
                int glowR = isHov ? _hoverGlowRadius : _shadowRadius;
                PaintShadow(g, rc, glowC, DesignMode ? 4 : glowR);
            }

            // ── 4: Gradient داخل الـ Path ────────────────────────
            g.SetClip(path);
            PaintGradient(g, rc);
            if (_showShimmer && _shimmerOpacity > 0)
                PaintShimmer(g, rc);
            g.ResetClip();

            // ── 5: Glass Border ───────────────────────────────────
            if (_showGlassBorder && _glassBorderAlpha > 0)
                using (var pen = new Pen(Color.FromArgb(_glassBorderAlpha, 255, 255, 255), 1.2f))
                    g.DrawPath(pen, path);

            // ── 6: أبناء الكنترول ────────────────────────────────
            base.OnPaint(e);
        }

        // ══════════════════════════════════════════════════════════
        //  Glassmorphism Blur Engine
        // ══════════════════════════════════════════════════════════
        private void DrawBlurredBackground(Graphics g, GraphicsPath clipPath)
        {
            if (Parent == null) return;

            if (_blurCache == null ||
                _blurCacheSize != Size ||
                _blurCacheRadius != _blurRadius ||
                _blurCacheLocation != Location)
            {
                _blurCache?.Dispose();
                _blurCache = BuildBlurredBitmap();
                _blurCacheSize = Size;
                _blurCacheRadius = _blurRadius;
                _blurCacheLocation = Location;
            }

            using var tb = new TextureBrush(_blurCache);
            g.FillPath(tb, clipPath);
        }

        private Bitmap BuildBlurredBitmap()
        {
            const float scale = 0.25f;
            int smallW = Math.Max(1, (int)(Width * scale));
            int smallH = Math.Max(1, (int)(Height * scale));

            using var full = CaptureSceneBehind();

            using var small = new Bitmap(smallW, smallH, PixelFormat.Format32bppArgb);
            using (var sg = Graphics.FromImage(small))
            {
                sg.InterpolationMode = InterpolationMode.Bilinear;
                sg.DrawImage(full, 0, 0, smallW, smallH);
            }

            int smallRadius = Math.Max(1, (int)(_blurRadius * scale));
            using var blurredSmall = GaussianBlur(small, smallRadius);

            var result = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using (var rg = Graphics.FromImage(result))
            {
                rg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                rg.DrawImage(blurredSmall, 0, 0, Width, Height);
            }
            return result;
        }

        // ══════════════════════════════════════════════════════════
        //  Scene Capture (Parent + Siblings behind us)
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// ✅ FIXED v13: _sceneCaptureActive [ThreadStatic] + instance flag + try/catch
        /// يرسم المشهد الكامل خلف البانل دون cascading عند التداخل بين GradientPanels
        /// </summary>
        private void PaintSceneBackground(Graphics g)
        {
            if (Parent == null) return;
            if (_isPaintingBackground) return;   // منع الـ Recursion داخل نفس الـ Instance
            if (_sceneCaptureActive) return;   // ✅ منع الـ Cascading بين GradientPanels المتداخلة

            _isPaintingBackground = true;
            _sceneCaptureActive = true;
            try
            {
                var state = g.Save();
                g.TranslateTransform(-Left, -Top);

                using (var pe = new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height)))
                {
                    InvokePaintBackground(Parent, pe);
                    InvokePaint(Parent, pe);           // ← الآن أي GradientPanel أب لن يستدعي PaintSceneBackground
                }

                DrawSiblingsBehind(g);
                g.Restore(state);
            }
            catch { /* تجاهل أخطاء الرسم الخلفي */ }
            finally
            {
                _isPaintingBackground = false;
                _sceneCaptureActive = false;
            }
        }

        /// <summary>
        /// ✅ FIXED v13: _sceneCaptureActive [ThreadStatic] + instance flag + try/catch
        /// يلتقط المشهد الكامل خلف البانل في Bitmap دون cascading
        /// </summary>
        private Bitmap CaptureSceneBehind()
        {
            var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            if (Parent == null) return bmp;
            if (_isPaintingBackground) return bmp;   // منع الـ Recursion داخل نفس الـ Instance
            if (_sceneCaptureActive) return bmp;   // ✅ منع الـ Cascading بين GradientPanels المتداخلة

            _isPaintingBackground = true;
            _sceneCaptureActive = true;
            try
            {
                using var g = Graphics.FromImage(bmp);
                g.TranslateTransform(-Left, -Top);

                using (var pe = new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height)))
                {
                    InvokePaintBackground(Parent, pe);
                    InvokePaint(Parent, pe);
                }

                DrawSiblingsBehind(g);
            }
            catch { /* إذا فشل الـ Capture، ارجع Bitmap فاضي */ }
            finally
            {
                _isPaintingBackground = false;
                _sceneCaptureActive = false;
            }
            return bmp;
        }

        /// <summary>
        /// ✅ FIXED v12: يتجاهل نفسه (sib == this) — يرسم الكنترولات التي خلفنا في الـ Z-Order
        /// </summary>
        private void DrawSiblingsBehind(Graphics g)
        {
            if (Parent == null) return;
            int myIndex = Parent.Controls.GetChildIndex(this);
            var myBounds = new Rectangle(Left, Top, Width, Height);

            // index أعلى = خلفي في WinForms
            for (int i = Parent.Controls.Count - 1; i > myIndex; i--)
            {
                var sib = Parent.Controls[i];
                if (sib == this) continue;  // ✅ تجاهل نفسك
                if (!sib.Visible) continue;
                if (!sib.Bounds.IntersectsWith(myBounds)) continue;

                var st = g.Save();
                g.TranslateTransform(sib.Left, sib.Top);
                using var pe = new PaintEventArgs(g, new Rectangle(0, 0, sib.Width, sib.Height));
                InvokePaintBackground(sib, pe);
                InvokePaint(sib, pe);
                g.Restore(st);
            }
        }

        // ══════════════════════════════════════════════════════════
        //  Blur Cache Management
        // ══════════════════════════════════════════════════════════
        private void InvalidateBlurCache()
        {
            _blurCache?.Dispose();
            _blurCache = null;
            _blurCacheSize = Size.Empty;
            _blurCacheRadius = -1;
            _blurCacheLocation = new Point(-99999, -99999);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InvalidateBlurCache();
            UpdatePadding();
            Invalidate();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            InvalidateBlurCache();
            Invalidate();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            InvalidateBlurCache();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) InvalidateBlurCache();
            base.Dispose(disposing);
        }

        // ══════════════════════════════════════════════════════════
        //  Gaussian Blur (3 × Box Blur H+V = Gaussian تقريبي سريع)
        // ══════════════════════════════════════════════════════════
        private static Bitmap GaussianBlur(Bitmap src, int radius)
        {
            Bitmap a = BoxBlurH(src, radius);
            Bitmap b = BoxBlurV(a, radius); a.Dispose();
            Bitmap c = BoxBlurH(b, radius); b.Dispose();
            Bitmap d = BoxBlurV(c, radius); c.Dispose();
            Bitmap e = BoxBlurH(d, radius); d.Dispose();
            Bitmap f = BoxBlurV(e, radius); e.Dispose();
            return f;
        }

        private static unsafe Bitmap BoxBlurH(Bitmap src, int radius)
        {
            int w = src.Width, h = src.Height;
            var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            var srcData = src.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var dstData = dst.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int kernel = radius * 2 + 1;

            for (int y = 0; y < h; y++)
            {
                byte* sRow = (byte*)srcData.Scan0 + y * srcData.Stride;
                byte* dRow = (byte*)dstData.Scan0 + y * dstData.Stride;
                long b = 0, gr = 0, r = 0, a = 0;

                for (int kx = -radius; kx <= radius; kx++)
                {
                    int px = Math.Clamp(kx, 0, w - 1) * 4;
                    b += sRow[px]; gr += sRow[px + 1]; r += sRow[px + 2]; a += sRow[px + 3];
                }

                for (int x = 0; x < w; x++)
                {
                    dRow[x * 4] = (byte)(b / kernel);
                    dRow[x * 4 + 1] = (byte)(gr / kernel);
                    dRow[x * 4 + 2] = (byte)(r / kernel);
                    dRow[x * 4 + 3] = (byte)(a / kernel);
                    int ap = Math.Clamp(x + radius + 1, 0, w - 1) * 4;
                    int rp = Math.Clamp(x - radius, 0, w - 1) * 4;
                    b += sRow[ap] - sRow[rp];
                    gr += sRow[ap + 1] - sRow[rp + 1];
                    r += sRow[ap + 2] - sRow[rp + 2];
                    a += sRow[ap + 3] - sRow[rp + 3];
                }
            }
            src.UnlockBits(srcData);
            dst.UnlockBits(dstData);
            return dst;
        }

        private static unsafe Bitmap BoxBlurV(Bitmap src, int radius)
        {
            int w = src.Width, h = src.Height;
            var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            var srcData = src.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var dstData = dst.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int stride = srcData.Stride;
            int kernel = radius * 2 + 1;

            for (int x = 0; x < w; x++)
            {
                long b = 0, gr = 0, r = 0, a = 0;
                for (int ky = -radius; ky <= radius; ky++)
                {
                    byte* p = (byte*)srcData.Scan0 + Math.Clamp(ky, 0, h - 1) * stride + x * 4;
                    b += p[0]; gr += p[1]; r += p[2]; a += p[3];
                }

                for (int y = 0; y < h; y++)
                {
                    byte* dp = (byte*)dstData.Scan0 + y * stride + x * 4;
                    dp[0] = (byte)(b / kernel);
                    dp[1] = (byte)(gr / kernel);
                    dp[2] = (byte)(r / kernel);
                    dp[3] = (byte)(a / kernel);
                    byte* ap = (byte*)srcData.Scan0 + Math.Clamp(y + radius + 1, 0, h - 1) * stride + x * 4;
                    byte* rp = (byte*)srcData.Scan0 + Math.Clamp(y - radius, 0, h - 1) * stride + x * 4;
                    b += ap[0] - rp[0];
                    gr += ap[1] - rp[1];
                    r += ap[2] - rp[2];
                    a += ap[3] - rp[3];
                }
            }
            src.UnlockBits(srcData);
            dst.UnlockBits(dstData);
            return dst;
        }

        // ══════════════════════════════════════════════════════════
        //  Gradient Renderer
        // ══════════════════════════════════════════════════════════
        private void PaintGradient(Graphics g, Rectangle rc)
        {
            if (rc.Width <= 0 || rc.Height <= 0) return;
            var inf = Rectangle.Inflate(rc, 2, 2);

            if (_useThreeColors && _gradientMid != Color.Empty)
            {
                int h2 = Math.Max(1, rc.Height / 2);
                var top = new Rectangle(rc.X, rc.Y, rc.Width, h2 + 1);
                var bot = new Rectangle(rc.X, rc.Y + h2, rc.Width, Math.Max(1, rc.Height - h2));
                if (top.Height > 0)
                    using (var lg = new LinearGradientBrush(top, _gradientStart, _gradientMid, _gradientAngle))
                        g.FillRectangle(lg, top);
                if (bot.Height > 0)
                    using (var lg = new LinearGradientBrush(bot, _gradientMid, _gradientEnd, _gradientAngle))
                        g.FillRectangle(lg, bot);
            }
            else
            {
                try
                {
                    using var lg = new LinearGradientBrush(inf, _gradientStart, _gradientEnd, _gradientAngle);
                    lg.InterpolationColors = new ColorBlend(3)
                    {
                        Colors = new[] { _gradientStart, BlendColor(_gradientStart, _gradientEnd, 0.42f), _gradientEnd },
                        Positions = new[] { 0f, 0.5f, 1f }
                    };
                    g.FillRectangle(lg, rc);
                }
                catch
                {
                    using var lg = new LinearGradientBrush(inf, _gradientStart, _gradientEnd, _gradientAngle);
                    g.FillRectangle(lg, rc);
                }
            }
        }

        // ══════════════════════════════════════════════════════════
        //  Shimmer
        // ══════════════════════════════════════════════════════════
        private void PaintShimmer(Graphics g, Rectangle rc)
        {
            int shimH = Math.Max(1, rc.Height * 2 / 5);
            var shimR = new Rectangle(rc.X, rc.Y, rc.Width, shimH);
            using var lg = new LinearGradientBrush(
                new Rectangle(shimR.X, shimR.Y, shimR.Width, shimR.Height + 1),
                Color.FromArgb(Math.Min(255, _shimmerOpacity), 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                LinearGradientMode.Vertical);
            lg.SetSigmaBellShape(0.4f, 0.9f);
            g.FillRectangle(lg, shimR);
        }

        // ══════════════════════════════════════════════════════════
        //  Shadow / Glow
        // ══════════════════════════════════════════════════════════
        private void PaintShadow(Graphics g, Rectangle card, Color clr, int radius)
        {
            if (radius <= 0 || clr.A == 0) return;
            int ox = DesignMode ? 0 : _shadowOffsetX;
            int oy = DesignMode ? 0 : _shadowOffsetY;

            for (int i = radius; i >= 1; i--)
            {
                float t = (float)i / radius;
                int alpha = (int)(clr.A * (1f - t * t) * 0.75f);
                if (alpha <= 0) continue;

                var sr = new Rectangle(
                    card.Left - i + ox, card.Top - i + oy,
                    card.Width + i * 2, card.Height + i * 2);

                int grow = i;
                int maxHalf = Math.Min(sr.Width, sr.Height) / 2;
                var shadowCr = new CornerRadiusEx(
                    Math.Min(_cornerRadiusEx.TopLeft + grow, maxHalf),
                    Math.Min(_cornerRadiusEx.TopRight + grow, maxHalf),
                    Math.Min(_cornerRadiusEx.BottomRight + grow, maxHalf),
                    Math.Min(_cornerRadiusEx.BottomLeft + grow, maxHalf));

                using var sp = BuildPath(sr, shadowCr);
                using var sb = new SolidBrush(Color.FromArgb(Math.Min(255, alpha), clr.R, clr.G, clr.B));
                g.FillPath(sb, sp);
            }
        }

        // ══════════════════════════════════════════════════════════
        //  GetCardRect
        // ══════════════════════════════════════════════════════════
        private Rectangle GetCardRect()
        {
            int sr = DesignMode ? 4 : (_showShadow ? _shadowRadius : 4);
            int ox = DesignMode ? 0 : _shadowOffsetX;
            int oy = DesignMode ? 0 : _shadowOffsetY;
            int l = sr + Math.Max(0, -ox);
            int t = sr + Math.Max(0, -oy);
            int r = sr + Math.Max(0, ox);
            int b = sr + Math.Max(0, oy);
            return new Rectangle(l, t,
                Math.Max(1, Width - l - r - 1),
                Math.Max(1, Height - t - b - 1));
        }

        // ══════════════════════════════════════════════════════════
        //  UpdatePadding
        // ══════════════════════════════════════════════════════════
        private void UpdatePadding()
        {
            int sr = _showShadow ? _shadowRadius : 4;
            int ox = Math.Abs(_shadowOffsetX);
            int oy = Math.Abs(_shadowOffsetY);
            Padding = new Padding(
                Math.Max(sr + (_shadowOffsetX < 0 ? ox : 0), 6),
                Math.Max(sr + (_shadowOffsetY < 0 ? oy : 0), 6),
                Math.Max(sr + (_shadowOffsetX > 0 ? ox : 0), 6),
                Math.Max(sr + (_shadowOffsetY > 0 ? oy : 0), 6));
        }

        // ══════════════════════════════════════════════════════════
        //  Mouse Events
        // ══════════════════════════════════════════════════════════
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!DesignMode) { _isHovered = true; Invalidate(); }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!DesignMode && !ClientRectangle.Contains(PointToClient(MousePosition)))
            { _isHovered = false; Invalidate(); }
            base.OnMouseLeave(e);
        }

        // ══════════════════════════════════════════════════════════
        //  OnControlAdded
        // ══════════════════════════════════════════════════════════
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            ApplyTransparency(e.Control);
        }

        private void ApplyTransparency(Control ctrl)
        {
            if (ctrl is ButtonBase btn)
            {
                if (btn.FlatStyle != FlatStyle.Flat && btn.FlatStyle != FlatStyle.Popup)
                { btn.FlatStyle = FlatStyle.Flat; btn.FlatAppearance.BorderSize = 0; }
                if (btn.BackColor == SystemColors.Control || btn.BackColor == SystemColors.ButtonFace)
                    btn.BackColor = Color.Transparent;
            }
            else if (ctrl is Label lbl) { lbl.BackColor = Color.Transparent; }
            else if (ctrl is PictureBox pb) { pb.BackColor = Color.Transparent; }
            else
            {
                ctrl.SetStyle_IfSupported(ControlStyles.SupportsTransparentBackColor, true);
                if (ctrl.BackColor == SystemColors.Control)
                    ctrl.BackColor = Color.Transparent;
            }
            foreach (Control child in ctrl.Controls)
                ApplyTransparency(child);
        }

        // ══════════════════════════════════════════════════════════
        //  Path Builder — Per-Corner Radius
        // ══════════════════════════════════════════════════════════
        private static GraphicsPath BuildPath(Rectangle r, CornerRadiusEx cr)
        {
            int maxR = Math.Min(r.Width, r.Height) / 2;
            int tl = Math.Clamp(cr.TopLeft, 0, maxR) * 2;
            int tr = Math.Clamp(cr.TopRight, 0, maxR) * 2;
            int br = Math.Clamp(cr.BottomRight, 0, maxR) * 2;
            int bl = Math.Clamp(cr.BottomLeft, 0, maxR) * 2;

            var path = new GraphicsPath();
            if (tl > 0) path.AddArc(r.Left, r.Top, tl, tl, 180, 90);
            else path.AddLine(r.Left, r.Top, r.Left, r.Top);
            if (tr > 0) path.AddArc(r.Right - tr, r.Top, tr, tr, 270, 90);
            else path.AddLine(r.Right, r.Top, r.Right, r.Top);
            if (br > 0) path.AddArc(r.Right - br, r.Bottom - br, br, br, 0, 90);
            else path.AddLine(r.Right, r.Bottom, r.Right, r.Bottom);
            if (bl > 0) path.AddArc(r.Left, r.Bottom - bl, bl, bl, 90, 90);
            else path.AddLine(r.Left, r.Bottom, r.Left, r.Bottom);
            path.CloseFigure();
            return path;
        }

        // ══════════════════════════════════════════════════════════
        //  Color Helpers
        // ══════════════════════════════════════════════════════════
        private static Color BlendColor(Color a, Color b, float t) =>
            Color.FromArgb(
                Clamp255((int)(a.A + (b.A - a.A) * t)),
                Clamp255((int)(a.R + (b.R - a.R) * t)),
                Clamp255((int)(a.G + (b.G - a.G) * t)),
                Clamp255((int)(a.B + (b.B - a.B) * t)));

        private static int Clamp255(int v) => v < 0 ? 0 : v > 255 ? 255 : v;
    }

    // ══════════════════════════════════════════════════════════════
    //  ControlExtensions
    // ══════════════════════════════════════════════════════════════
    internal static class ControlExtensions
    {
        public static void SetStyle_IfSupported(this Control ctrl, ControlStyles flag, bool value)
        {
            try
            {
                var m = typeof(Control).GetMethod("SetStyle",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);
                m?.Invoke(ctrl, new object[] { flag, value });
            }
            catch { }
        }
    }
}