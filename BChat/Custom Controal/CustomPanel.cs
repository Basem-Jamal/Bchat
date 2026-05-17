using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Car_Rental_System.CustomControls
{
    [TypeConverter(typeof(CornerRadiusConverter))]
    public class CornerRadius
    {
        public int TopLeft { get; set; }
        public int TopRight { get; set; }
        public int BottomRight { get; set; }
        public int BottomLeft { get; set; }

        public CornerRadius() { }
        public CornerRadius(int all)
        { TopLeft = TopRight = BottomRight = BottomLeft = all; }
        public CornerRadius(int tl, int tr, int br, int bl)
        { TopLeft = tl; TopRight = tr; BottomRight = br; BottomLeft = bl; }

        public override string ToString() => $"{TopLeft}, {TopRight}, {BottomRight}, {BottomLeft}";
    }

    public class CornerRadiusConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext ctx, Type t)
            => t == typeof(string) || base.CanConvertFrom(ctx, t);

        public override object ConvertFrom(ITypeDescriptorContext ctx, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string s)
            {
                var parts = s.Split(',');
                if (parts.Length == 4 &&
                    int.TryParse(parts[0].Trim(), out int tl) &&
                    int.TryParse(parts[1].Trim(), out int tr) &&
                    int.TryParse(parts[2].Trim(), out int br) &&
                    int.TryParse(parts[3].Trim(), out int bl))
                    return new CornerRadius(tl, tr, br, bl);

                if (int.TryParse(s.Trim(), out int all))
                    return new CornerRadius(all);
            }
            return base.ConvertFrom(ctx, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext ctx, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is CornerRadius cr)
                return cr.ToString();
            return base.ConvertTo(ctx, culture, value, destType);
        }
    }

    [ToolboxItem(true)]
    [DefaultProperty("BackColorEx")]
    public class CustomPanel : Panel
    {
        // ─── Fields ───────────────────────────────────────────
        private Color _backColorEx = Color.FromArgb(40, 255, 255, 255);
        private Color _borderColor = Color.FromArgb(60, 255, 255, 255);
        private int _borderThickness = 1;
        private int _borderRadius = 15;
        private CornerRadius _cornerRadius = new CornerRadius(15);

        // Shadow
        private bool _useShadow = true;
        private int _shadowSize = 6;
        private Color _shadowColor = Color.FromArgb(80, 0, 0, 0);

        // Blur / Glass
        private bool _useBlur = false;
        private int _blurRadius = 10;

        // ─── Blur Cache ───────────────────────────────────────
        // نُعيد استخدام الـ Bitmap المضبّب بدل إعادة حسابه كل frame
        private Bitmap? _blurCache;
        private Size _blurCacheSize = Size.Empty;
        private int _blurCacheRadius = -1;

        // ─── Constructor ──────────────────────────────────────
        public CustomPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Size = new Size(200, 120);
            Font = new Font("Segoe UI", 10f);
            ForeColor = Color.White;
            BackColor = Color.Transparent;
        }

        // ════════════════════════════════════════════════════
        //  Properties
        // ════════════════════════════════════════════════════

        [Category("Appearance")]
        [Description("لون التراكب فوق الـ Blur — يدعم الـ Alpha مثل Color.FromArgb(40, 255, 255, 255)")]
        public Color BackColorEx
        {
            get => _backColorEx;
            set { _backColorEx = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(15)]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = Math.Max(0, value);
                _cornerRadius = new CornerRadius(_borderRadius);
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadius(0); Invalidate(); }
        }

        // ─── Shadow ───────────────────────────────────────────
        [Category("Shadow")]
        public bool UseShadow
        {
            get => _useShadow;
            set { _useShadow = value; Invalidate(); }
        }

        [Category("Shadow")]
        public int ShadowSize
        {
            get => _shadowSize;
            set { _shadowSize = Math.Max(0, value); Invalidate(); }
        }

        [Category("Shadow")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        // ─── Glass / Blur ─────────────────────────────────────
        [Category("Glass")]
        [DefaultValue(false)]
        [Description("تفعيل تأثير الزجاج الضبابي (Glassmorphism Blur)")]
        public bool UseBlur
        {
            get => _useBlur;
            set { _useBlur = value; InvalidateBlurCache(); Invalidate(); }
        }

        [Category("Glass")]
        [DefaultValue(10)]
        [Description("شدة الضبابية: 1 = خفيف جداً  |  40 = ضبابي كامل")]
        public int BlurRadius
        {
            get => _blurRadius;
            set { _blurRadius = Math.Clamp(value, 1, 40); InvalidateBlurCache(); Invalidate(); }
        }

        // ════════════════════════════════════════════════════
        //  Suppress default Panel background
        // ════════════════════════════════════════════════════
        protected override void OnPaintBackground(PaintEventArgs e) { }

        // ════════════════════════════════════════════════════
        //  Paint
        // ════════════════════════════════════════════════════
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int shadow = _useShadow ? _shadowSize : 0;

            var rectPanel = new Rectangle(0, 0, Width - 1 - shadow, Height - 1 - shadow);
            var rectShadow = new Rectangle(shadow, shadow, Width - 1 - shadow, Height - 1 - shadow);

            using var pathPanel = GetRoundedPath(rectPanel, _cornerRadius);
            using var pathShadow = GetRoundedPath(rectShadow, _cornerRadius);

            // ── 1: دائماً ارسم خلفية الـ Parent أولاً ──────────
            // هذا يُنظّف الزوايا خارج الـ Rounded Path (المنطقة المدورة)
            // بدونه تبقى الزوايا بها بكسلات قديمة أو داكنة
            PaintSceneBackground(g);

            // ── 2: فوقها ارسم الـ Blur داخل الـ Path فقط ───────
            if (_useBlur)
                DrawBlurredBackground(g, pathPanel);

            // ── 2: الظل ───────────────────────────────────────
            if (_useShadow)
            {
                using var sb = new PathGradientBrush(pathShadow);
                sb.CenterColor = _shadowColor;
                sb.SurroundColors = new[] { Color.Transparent };
                g.FillPath(sb, pathShadow);
            }

            // ── 3: لون التراكب (Glass tint overlay) ───────────
            using (var overlayBrush = new SolidBrush(_backColorEx))
                g.FillPath(overlayBrush, pathPanel);

            // ── 4: البوردر ────────────────────────────────────
            if (_borderThickness > 0)
            {
                using var pen = new Pen(_borderColor, _borderThickness);
                g.DrawPath(pen, pathPanel);
            }
        }

        // ════════════════════════════════════════════════════
        //  Glassmorphism Engine
        // ════════════════════════════════════════════════════

        private void DrawBlurredBackground(Graphics g, GraphicsPath clipPath)
        {
            if (Parent == null) { PaintSceneBackground(g); return; }

            // أعد البناء فقط إذا تغيّر الحجم أو الـ BlurRadius
            if (_blurCache == null ||
                _blurCacheSize != Size ||
                _blurCacheRadius != _blurRadius)
            {
                _blurCache?.Dispose();
                _blurCache = BuildBlurredBitmap();
                _blurCacheSize = Size;
                _blurCacheRadius = _blurRadius;
            }

            // TextureBrush + FillPath = حواف ناعمة مع Anti-Aliasing
            using var textureBrush = new TextureBrush(_blurCache);
            g.FillPath(textureBrush, clipPath);
        }

        /// <summary>
        /// الخوارزمية:
        ///   1. التقط المشهد بالحجم الكامل
        ///   2. صغّره إلى 25%  ← هذا يجعله أسرع 16x
        ///   3. طبّق الـ Blur على الحجم الصغير (بسيط وسريع)
        ///   4. كبّره مرة أخرى ← HighQualityBicubic يُعطي حواف ناعمة تلقائياً
        ///
        ///   النتيجة: مطابقة بصرياً للـ Blur الكامل، بسرعة 16x.
        /// </summary>
        private Bitmap BuildBlurredBitmap()
        {
            const float scale = 0.25f; // 25% — غيّره لـ 0.5f لو أردت دقة أعلى

            int smallW = Math.Max(1, (int)(Width * scale));
            int smallH = Math.Max(1, (int)(Height * scale));

            // الخطوة 1: التقط المشهد الكامل
            using var full = CaptureSceneBehind();

            // الخطوة 2: صغّر إلى 25%
            using var small = new Bitmap(smallW, smallH, PixelFormat.Format32bppArgb);
            using (var sg = Graphics.FromImage(small))
            {
                sg.InterpolationMode = InterpolationMode.Bilinear;
                sg.DrawImage(full, 0, 0, smallW, smallH);
            }

            // الخطوة 3: Blur على الحجم الصغير
            // radius × scale لأن كل بكسل يمثّل الآن 4 بكسلات أصلية
            int smallRadius = Math.Max(1, (int)(_blurRadius * scale));
            using var blurredSmall = GaussianBlur(small, smallRadius);

            // الخطوة 4: كبّر للحجم الأصلي
            var result = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using (var rg = Graphics.FromImage(result))
            {
                rg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                rg.DrawImage(blurredSmall, 0, 0, Width, Height);
            }

            return result;
        }

        // ── يُفرغ الـ Cache عند تغيير الحجم ─────────────────
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InvalidateBlurCache();
        }

        private void InvalidateBlurCache()
        {
            _blurCache?.Dispose();
            _blurCache = null;
            _blurCacheSize = Size.Empty;
            _blurCacheRadius = -1;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) InvalidateBlurCache();
            base.Dispose(disposing);
        }

        // ════════════════════════════════════════════════════
        //  Scene Capture (Parent + Siblings behind us)
        // ════════════════════════════════════════════════════

        private Bitmap CaptureSceneBehind()
        {
            var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bmp);

            g.TranslateTransform(-Left, -Top);

            // الـ Parent
            using (var pe = new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height)))
            {
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);
            }

            // الـ Siblings التي خلفنا في الـ Z-Order
            int myIndex = Parent.Controls.GetChildIndex(this);
            var myBounds = new Rectangle(Left, Top, Width, Height);

            for (int i = Parent.Controls.Count - 1; i > myIndex; i--)
            {
                var sib = Parent.Controls[i];
                if (!sib.Visible) continue;
                if (!sib.Bounds.IntersectsWith(myBounds)) continue;

                var state = g.Save();
                g.TranslateTransform(sib.Left, sib.Top);
                using (var sibPe = new PaintEventArgs(g, new Rectangle(0, 0, sib.Width, sib.Height)))
                {
                    InvokePaintBackground(sib, sibPe);
                    InvokePaint(sib, sibPe);
                }
                g.Restore(state);
            }

            return bmp;
        }

        private void PaintSceneBackground(Graphics g)
        {
            if (Parent == null) return;

            var state = g.Save();
            g.TranslateTransform(-Left, -Top);

            using (var pe = new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height)))
            {
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);
            }

            int myIndex = Parent.Controls.GetChildIndex(this);
            var myBounds = new Rectangle(Left, Top, Width, Height);

            for (int i = Parent.Controls.Count - 1; i > myIndex; i--)
            {
                var sib = Parent.Controls[i];
                if (!sib.Visible) continue;
                if (!sib.Bounds.IntersectsWith(myBounds)) continue;

                var sibState = g.Save();
                g.TranslateTransform(sib.Left, sib.Top);
                using (var sibPe = new PaintEventArgs(g, new Rectangle(0, 0, sib.Width, sib.Height)))
                {
                    InvokePaintBackground(sib, sibPe);
                    InvokePaint(sib, sibPe);
                }
                g.Restore(sibState);
            }

            g.Restore(state);
        }

        // ════════════════════════════════════════════════════
        //  Gaussian Blur (3 × Box Blur H+V)
        // ════════════════════════════════════════════════════

        private static Bitmap GaussianBlur(Bitmap source, int radius)
        {
            Bitmap a = BoxBlurH(source, radius);
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

                    int addPx = Math.Clamp(x + radius + 1, 0, w - 1) * 4;
                    int remPx = Math.Clamp(x - radius, 0, w - 1) * 4;
                    b += sRow[addPx] - sRow[remPx];
                    gr += sRow[addPx + 1] - sRow[remPx + 1];
                    r += sRow[addPx + 2] - sRow[remPx + 2];
                    a += sRow[addPx + 3] - sRow[remPx + 3];
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
                    b += ap[0] - rp[0]; gr += ap[1] - rp[1];
                    r += ap[2] - rp[2]; a += ap[3] - rp[3];
                }
            }
            src.UnlockBits(srcData);
            dst.UnlockBits(dstData);
            return dst;
        }

        // ════════════════════════════════════════════════════
        //  Rounded Path Helper
        // ════════════════════════════════════════════════════
        private static GraphicsPath GetRoundedPath(Rectangle r, CornerRadius cr)
        {
            var path = new GraphicsPath();
            int tl = cr.TopLeft * 2, tr = cr.TopRight * 2;
            int br = cr.BottomRight * 2, bl = cr.BottomLeft * 2;

            if (tl > 0) path.AddArc(r.X, r.Y, tl, tl, 180, 90);
            else path.AddLine(r.X, r.Y, r.X, r.Y);
            if (tr > 0) path.AddArc(r.Right - tr, r.Y, tr, tr, 270, 90);
            else path.AddLine(r.Right, r.Y, r.Right, r.Y);
            if (br > 0) path.AddArc(r.Right - br, r.Bottom - br, br, br, 0, 90);
            else path.AddLine(r.Right, r.Bottom, r.Right, r.Bottom);
            if (bl > 0) path.AddArc(r.X, r.Bottom - bl, bl, bl, 90, 90);
            else path.AddLine(r.X, r.Bottom, r.X, r.Bottom);

            path.CloseFigure();
            return path;
        }
    }
}