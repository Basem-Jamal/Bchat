using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BChat.Custom_Controal.Custom_Bchat.Animated
{
    // ═══════════════════════════════════════════════════════════════
    // CORNER RADIUS
    // ═══════════════════════════════════════════════════════════════

    [TypeConverter(typeof(CornerRadiusExConverter))]
    public class CornerRadiusEx
    {
        public int TopLeft { get; set; }
        public int TopRight { get; set; }
        public int BottomRight { get; set; }
        public int BottomLeft { get; set; }

        public CornerRadiusEx() { }
        public CornerRadiusEx(int all)
            => TopLeft = TopRight = BottomRight = BottomLeft = all;
        public CornerRadiusEx(int tl, int tr, int br, int bl)
        { TopLeft = tl; TopRight = tr; BottomRight = br; BottomLeft = bl; }

        public override string ToString()
            => $"{TopLeft}, {TopRight}, {BottomRight}, {BottomLeft}";
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

    // ═══════════════════════════════════════════════════════════════
    // ENUMS
    // ═══════════════════════════════════════════════════════════════

    public enum PageTransitionType
    {
        Fade,
        SlideFromRight,
        SlideFromLeft,
        SlideFromBottom,
        SlideFromTop,
        ZoomIn,
        ZoomOut,
        SlideFade,
        ScaleFade,
        None
    }

    public enum EasingType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        EaseInOutCubic,
        EaseOutBack,
        EaseOutExpo,
        EaseOutCirc,
        Spring,
        Bounce,
        Elastic
    }

    // ═══════════════════════════════════════════════════════════════
    // EVENTS
    // ═══════════════════════════════════════════════════════════════

    public class PageTransitionEventArgs : EventArgs
    {
        public Control OldPage { get; }
        public Control NewPage { get; }
        public PageTransitionType TransitionType { get; }

        public PageTransitionEventArgs(Control old, Control next, PageTransitionType type)
        { OldPage = old; NewPage = next; TransitionType = type; }
    }

    // ═══════════════════════════════════════════════════════════════
    // MAIN CONTROL
    // ═══════════════════════════════════════════════════════════════

    [ToolboxItem(true)]
    [Description("لوحة تنقل بين الصفحات مع انميشن سلس + تصميم زجاجي احترافي")]
    public class AnimatedPagePanel : Panel
    {
        // ─────────────────────────────────────────────
        // Animation Fields
        // ─────────────────────────────────────────────

        private readonly Timer _timer;

        private Bitmap _bmpOld;
        private Bitmap _bmpNew;

        private float _rawProgress;
        private float _easedProgress;
        private bool _isAnimating;

        private Control _currentPage;
        private Control _pendingPage;

        private long _animStartTick;
        private long _animDurationTick;

        private readonly Queue<Control> _navQueue = new();

        // ─────────────────────────────────────────────
        // Appearance Fields (CustomPanel style)
        // ─────────────────────────────────────────────

        private Color _backColorEx = Color.FromArgb(40, 255, 255, 255);
        private Color _borderColor = Color.FromArgb(60, 255, 255, 255);
        private int _borderThickness = 1;
        private CornerRadiusEx _cornerRadius = new CornerRadiusEx(15);

        // Shadow
        private bool _useShadow = true;
        private int _shadowSize = 6;
        private Color _shadowColor = Color.FromArgb(80, 0, 0, 0);

        // Glass / Blur
        private bool _useBlur = false;
        private int _blurRadius = 10;

        // Blur cache
        private Bitmap? _blurCache;
        private Size _blurCacheSize = Size.Empty;
        private int _blurCacheRadius = -1;

        // ─────────────────────────────────────────────
        // Animation Properties
        // ─────────────────────────────────────────────

        private PageTransitionType _transitionType = PageTransitionType.SlideFade;
        private EasingType _easingType = EasingType.EaseOutExpo;
        private int _duration = 280;
        private bool _animationsEnabled = true;
        private int _fps = 60;
        private bool _queueTransitions = false;

        [Category("✦ الانميشن")]
        [Description("نوع حركة الانتقال بين الصفحات")]
        public PageTransitionType TransitionType
        {
            get => _transitionType;
            set => _transitionType = value;
        }

        [Category("✦ الانميشن")]
        [Description("نوع منحنى الحركة (Easing)")]
        public EasingType EasingFunction
        {
            get => _easingType;
            set => _easingType = value;
        }

        [Category("✦ الانميشن")]
        [Description("مدة الانتقال بالميللي ثانية (50 - 2000)")]
        public int TransitionDuration
        {
            get => _duration;
            set => _duration = Math.Max(50, Math.Min(2000, value));
        }

        [Category("✦ الانميشن")]
        [Description("تفعيل أو إيقاف الانميشن كلياً")]
        [DefaultValue(true)]
        public bool AnimationsEnabled
        {
            get => _animationsEnabled;
            set => _animationsEnabled = value;
        }

        [Category("✦ الانميشن")]
        [Description("عدد الإطارات في الثانية (10 - 144)")]
        [DefaultValue(60)]
        public int FramesPerSecond
        {
            get => _fps;
            set
            {
                _fps = Math.Max(10, Math.Min(144, value));
                if (_timer != null)
                    _timer.Interval = 1000 / _fps;
            }
        }

        [Category("✦ الانميشن")]
        [Description("ترتيب طلبات التنقل في قائمة انتظار")]
        [DefaultValue(false)]
        public bool QueueTransitions
        {
            get => _queueTransitions;
            set => _queueTransitions = value;
        }

        [Browsable(false)]
        public bool IsAnimating => _isAnimating;

        [Browsable(false)]
        public Control CurrentPage => _currentPage;

        // ─────────────────────────────────────────────
        // Appearance Properties
        // ─────────────────────────────────────────────

        [Category("✦ المظهر")]
        [Description("لون التراكب — يدعم الـ Alpha مثل Color.FromArgb(40, 255, 255, 255)")]
        public Color BackColorEx
        {
            get => _backColorEx;
            set { _backColorEx = value; Invalidate(); }
        }

        [Category("✦ المظهر")]
        [Description("لون الحدود")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("✦ المظهر")]
        [Description("سماكة الحدود (0 = بدون حدود)")]
        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = Math.Max(0, value); Invalidate(); }
        }

        [Category("✦ المظهر")]
        [Description("نصف قطر الزوايا المدورة لكل زاوية بشكل منفصل")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadiusEx CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadiusEx(0); Invalidate(); }
        }

        [Category("✦ الظل")]
        [Description("تفعيل ظل تحت البنل")]
        [DefaultValue(true)]
        public bool UseShadow
        {
            get => _useShadow;
            set { _useShadow = value; Invalidate(); }
        }

        [Category("✦ الظل")]
        [Description("حجم الظل بالبكسل")]
        public int ShadowSize
        {
            get => _shadowSize;
            set { _shadowSize = Math.Max(0, value); Invalidate(); }
        }

        [Category("✦ الظل")]
        [Description("لون الظل — يدعم الـ Alpha")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("✦ الزجاج")]
        [Description("تفعيل تأثير Glassmorphism Blur خلف البنل")]
        [DefaultValue(false)]
        public bool UseBlur
        {
            get => _useBlur;
            set { _useBlur = value; InvalidateBlurCache(); Invalidate(); }
        }

        [Category("✦ الزجاج")]
        [Description("شدة الضبابية: 1 = خفيف  |  40 = كامل")]
        [DefaultValue(10)]
        public int BlurRadius
        {
            get => _blurRadius;
            set { _blurRadius = Math.Clamp(value, 1, 40); InvalidateBlurCache(); Invalidate(); }
        }

        // ─────────────────────────────────────────────
        // Events
        // ─────────────────────────────────────────────

        public event EventHandler<PageTransitionEventArgs> TransitionStarted;
        public event EventHandler<PageTransitionEventArgs> TransitionCompleted;

        // ─────────────────────────────────────────────
        // Constructor
        // ─────────────────────────────────────────────

        public AnimatedPagePanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true
            );

            DoubleBuffered = true;
            BackColor = Color.Transparent;
            UpdateStyles();

            _timer = new Timer { Interval = 1000 / _fps };
            _timer.Tick += OnTimerTick;
        }

        // ═══════════════════════════════════════════════════════════════
        // PUBLIC API — Navigation
        // ═══════════════════════════════════════════════════════════════

        public void RegisterPage(Control page, string name = null)
        {
            if (page == null || Controls.Contains(page)) return;

            if (!string.IsNullOrWhiteSpace(name))
                page.Name = name;

            page.Dock = DockStyle.Fill;
            page.Visible = false;

            EnableDoubleBuffering(page);
            Controls.Add(page);
        }

        public Control GetPage(string name) => Controls[name];

        public void NavigateTo(Control page)
        {
            if (page == null) return;

            if (_isAnimating)
            {
                if (_queueTransitions) { _navQueue.Enqueue(page); return; }
                ForceCompleteAnimation();
            }

            if (page == _currentPage) return;

            if (!Controls.Contains(page))
            {
                page.Dock = DockStyle.Fill;
                page.Visible = false;
                EnableDoubleBuffering(page);
                Controls.Add(page);
            }

            // ── أول صفحة ──
            if (_currentPage == null)
            {
                page.Visible = true;
                page.BringToFront();
                _currentPage = page;
                return;
            }

            // ── الانميشن مُعطَّل ──
            if (!_animationsEnabled || _transitionType == PageTransitionType.None)
            {
                _currentPage.Visible = false;
                page.Visible = true;
                page.BringToFront();

                var prev = _currentPage;
                _currentPage = page;

                TransitionCompleted?.Invoke(this,
                    new PageTransitionEventArgs(prev, page, PageTransitionType.None));
                return;
            }

            // ── تجهيز الانميشن ──
            Control oldPage = _currentPage;

            page.Visible = true;
            page.Bounds = ClientRectangle;
            page.BringToFront();
            page.Refresh();

            DisposeBitmaps();
            _bmpOld = CaptureControl(oldPage);
            _bmpNew = CaptureControl(page);

            oldPage.Visible = false;
            page.Visible = false;

            _pendingPage = page;
            _rawProgress = 0f;
            _easedProgress = 0f;
            _isAnimating = true;
            _animStartTick = Environment.TickCount64;
            _animDurationTick = _duration;

            TransitionStarted?.Invoke(this,
                new PageTransitionEventArgs(oldPage, page, _transitionType));

            _timer.Start();
        }

        public void ForceCompleteAnimation()
        {
            if (!_isAnimating) return;
            _timer.Stop();
            CompleteTransition();
        }

        // ═══════════════════════════════════════════════════════════════
        // ANIMATION LOOP
        // ═══════════════════════════════════════════════════════════════

        private void OnTimerTick(object sender, EventArgs e)
        {
            long elapsed = Environment.TickCount64 - _animStartTick;
            _rawProgress = Math.Min((float)elapsed / _animDurationTick, 1f);
            _easedProgress = Ease(_rawProgress);

            Invalidate();

            if (_rawProgress >= 1f)
            {
                _timer.Stop();
                CompleteTransition();
            }
        }

        private void CompleteTransition()
        {
            _isAnimating = false;

            Control old = _currentPage;
            _currentPage = _pendingPage;
            _pendingPage = null;

            if (old != null) old.Visible = false;
            if (_currentPage != null) { _currentPage.Visible = true; _currentPage.BringToFront(); }

            DisposeBitmaps();
            Invalidate();

            TransitionCompleted?.Invoke(this,
                new PageTransitionEventArgs(old, _currentPage, _transitionType));

            if (_queueTransitions && _navQueue.Count > 0)
                NavigateTo(_navQueue.Dequeue());
        }

        // ═══════════════════════════════════════════════════════════════
        // PAINT — طبقات مرتبة: Parent Scene → Blur → Shadow → Tint → Border → Animation
        // ═══════════════════════════════════════════════════════════════

        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int shadow = _useShadow ? _shadowSize : 0;

            var rectPanel = new Rectangle(0, 0, Width - 1 - shadow, Height - 1 - shadow);
            var rectShadow = new Rectangle(shadow, shadow, Width - 1 - shadow, Height - 1 - shadow);

            using var pathPanel = GetRoundedPath(rectPanel, _cornerRadius);
            using var pathShadow = GetRoundedPath(rectShadow, _cornerRadius);

            // ── 1: خلفية الـ Parent لتنظيف الزوايا الخارجية ──
            PaintSceneBackground(g);

            // ── 2: Blur زجاجي داخل الـ Path فقط ──────────────
            if (_useBlur)
                DrawBlurredBackground(g, pathPanel);

            // ── 3: الظل ───────────────────────────────────────
            if (_useShadow && shadow > 0)
            {
                using var sb = new PathGradientBrush(pathShadow);
                sb.CenterColor = _shadowColor;
                sb.SurroundColors = new[] { Color.Transparent };
                g.FillPath(sb, pathShadow);
            }

            // ── 4: Clip على شكل البنل لرسم محتوى الانميشن ──
            var clipState = g.Save();
            g.SetClip(pathPanel);

            if (_isAnimating && _bmpOld != null && _bmpNew != null)
            {
                // ── 4a: رسم الانميشن مقيّداً بالـ Rounded Path ──
                g.CompositingMode = CompositingMode.SourceOver;
                DrawTransition(g, _easedProgress);
            }

            g.Restore(clipState);

            // ── 5: لون التراكب (Glass tint) فوق كل شيء ───────
            using (var overlayBrush = new SolidBrush(_backColorEx))
                g.FillPath(overlayBrush, pathPanel);

            // ── 6: الحدود ──────────────────────────────────────
            if (_borderThickness > 0)
            {
                using var pen = new Pen(_borderColor, _borderThickness);
                g.DrawPath(pen, pathPanel);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // TRANSITION EFFECTS
        // ═══════════════════════════════════════════════════════════════

        private void DrawTransition(Graphics g, float t)
        {
            switch (_transitionType)
            {
                case PageTransitionType.Fade: DrawFade(g, t); break;
                case PageTransitionType.SlideFromRight: DrawSlide(g, t, 1, 0); break;
                case PageTransitionType.SlideFromLeft: DrawSlide(g, t, -1, 0); break;
                case PageTransitionType.SlideFromBottom: DrawSlide(g, t, 0, 1); break;
                case PageTransitionType.SlideFromTop: DrawSlide(g, t, 0, -1); break;
                case PageTransitionType.ZoomIn: DrawZoom(g, t, true); break;
                case PageTransitionType.ZoomOut: DrawZoom(g, t, false); break;
                case PageTransitionType.SlideFade: DrawSlideFade(g, t); break;
                case PageTransitionType.ScaleFade: DrawScaleFade(g, t); break;
            }
        }

        private void DrawFade(Graphics g, float t)
        {
            DrawWithOpacity(g, _bmpOld, ClientRectangle, 1f - t);
            DrawWithOpacity(g, _bmpNew, ClientRectangle, t);
        }

        private void DrawSlide(Graphics g, float t, int dirX, int dirY)
        {
            float pullBack = 0.30f;
            int oldX = -(int)(dirX * Width * pullBack * t);
            int oldY = -(int)(dirY * Height * pullBack * t);
            int newX = (int)(dirX * Width * (1f - t));
            int newY = (int)(dirY * Height * (1f - t));

            DrawWithOpacity(g, _bmpOld,
                new Rectangle(oldX, oldY, Width, Height), 1f - t * 0.5f);
            DrawWithOpacity(g, _bmpNew,
                new Rectangle(newX, newY, Width, Height), 1f);
        }

        private void DrawSlideFade(Graphics g, float t)
        {
            float drift = 0.18f;

            int oldX = -(int)(Width * drift * t);
            DrawWithOpacity(g, _bmpOld,
                new Rectangle(oldX, 0, Width, Height), 1f - SmoothStep(t));

            int newX = (int)(Width * drift * (1f - t));
            DrawWithOpacity(g, _bmpNew,
                new Rectangle(newX, 0, Width, Height), SmoothStep(t));
        }

        private void DrawScaleFade(Graphics g, float t)
        {
            DrawScaled(g, _bmpOld, 1f - 0.04f * t, opacity: 1f - t);
            DrawScaled(g, _bmpNew, 0.96f + 0.04f * t, opacity: t);
        }

        private void DrawZoom(Graphics g, float t, bool zoomIn)
        {
            DrawWithOpacity(g, _bmpOld, ClientRectangle, 1f - t);
            float scale = zoomIn ? 0.88f + 0.12f * t : 1.12f - 0.12f * t;
            DrawScaled(g, _bmpNew, scale, opacity: t);
        }

        // ═══════════════════════════════════════════════════════════════
        // GLASSMORPHISM ENGINE  (مستورد من CustomPanel)
        // ═══════════════════════════════════════════════════════════════

        private void DrawBlurredBackground(Graphics g, GraphicsPath clipPath)
        {
            if (Parent == null) { PaintSceneBackground(g); return; }

            if (_blurCache == null ||
                _blurCacheSize != Size ||
                _blurCacheRadius != _blurRadius)
            {
                _blurCache?.Dispose();
                _blurCache = BuildBlurredBitmap();
                _blurCacheSize = Size;
                _blurCacheRadius = _blurRadius;
            }

            using var textureBrush = new TextureBrush(_blurCache);
            g.FillPath(textureBrush, clipPath);
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

        // ─── Scene Capture ────────────────────────────────────────────

        private Bitmap CaptureSceneBehind()
        {
            var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bmp);
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
                if (!sib.Visible || !sib.Bounds.IntersectsWith(myBounds)) continue;

                var state = g.Save();
                g.TranslateTransform(sib.Left, sib.Top);
                using var sibPe = new PaintEventArgs(g, new Rectangle(0, 0, sib.Width, sib.Height));
                InvokePaintBackground(sib, sibPe);
                InvokePaint(sib, sibPe);
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
                if (!sib.Visible || !sib.Bounds.IntersectsWith(myBounds)) continue;

                var sibState = g.Save();
                g.TranslateTransform(sib.Left, sib.Top);
                using var sibPe = new PaintEventArgs(g, new Rectangle(0, 0, sib.Width, sib.Height));
                InvokePaintBackground(sib, sibPe);
                InvokePaint(sib, sibPe);
                g.Restore(sibState);
            }

            g.Restore(state);
        }

        // ─── Gaussian Blur (3× Box H+V) ──────────────────────────────

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

        // ═══════════════════════════════════════════════════════════════
        // DRAWING HELPERS
        // ═══════════════════════════════════════════════════════════════

        private void DrawScaled(Graphics g, Bitmap bmp, float scale, float opacity)
        {
            if (bmp == null || opacity <= 0f) return;
            int w = (int)(Width * scale);
            int h = (int)(Height * scale);
            int x = (Width - w) / 2;
            int y = (Height - h) / 2;
            DrawWithOpacity(g, bmp, new Rectangle(x, y, w, h), opacity);
        }

        private static void DrawWithOpacity(Graphics g, Bitmap bmp, Rectangle dest, float opacity)
        {
            if (bmp == null || opacity <= 0f) return;
            opacity = Math.Min(1f, opacity);

            using var ia = new ImageAttributes();
            var cm = new ColorMatrix { Matrix33 = opacity };
            ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            g.DrawImage(bmp, dest,
                0, 0, bmp.Width, bmp.Height,
                GraphicsUnit.Pixel, ia);
        }

        private static float SmoothStep(float t) => t * t * (3f - 2f * t);

        // ═══════════════════════════════════════════════════════════════
        // EASING
        // ═══════════════════════════════════════════════════════════════

        private float Ease(float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return _easingType switch
            {
                EasingType.Linear => t,
                EasingType.EaseIn => t * t * t,
                EasingType.EaseOut => 1f - (float)Math.Pow(1f - t, 3),
                EasingType.EaseInOut => t < 0.5f
                                             ? 4f * t * t * t
                                             : 1f - (float)Math.Pow(-2f * t + 2f, 3) / 2f,
                EasingType.EaseInOutCubic => t < 0.5f
                                             ? 4f * t * t * t
                                             : 1f - (float)Math.Pow(-2f * t + 2f, 3) / 2f,
                EasingType.EaseOutExpo => t >= 1f ? 1f : 1f - (float)Math.Pow(2f, -10f * t),
                EasingType.EaseOutCirc => (float)Math.Sqrt(1f - Math.Pow(t - 1f, 2)),
                EasingType.EaseOutBack => EaseOutBack(t),
                EasingType.Spring => EaseSpring(t),
                EasingType.Bounce => EaseBounce(t),
                EasingType.Elastic => EaseElastic(t),
                _ => t
            };
        }

        private static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f, c3 = c1 + 1f;
            float u = t - 1f;
            return 1f + c3 * u * u * u + c1 * u * u;
        }

        private static float EaseSpring(float t)
            => 1f - (float)(Math.Cos(t * Math.PI * 4.5f) * Math.Exp(-t * 6f));

        private static float EaseBounce(float t)
        {
            const float n1 = 7.5625f, d1 = 2.75f;
            if (t < 1f / d1) return n1 * t * t;
            if (t < 2f / d1) { t -= 1.5f / d1; return n1 * t * t + 0.75f; }
            if (t < 2.5f / d1) { t -= 2.25f / d1; return n1 * t * t + 0.9375f; }
            t -= 2.625f / d1;
            return n1 * t * t + 0.984375f;
        }

        private static float EaseElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            const float c4 = (float)(2 * Math.PI / 3);
            return (float)(Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1);
        }

        // ═══════════════════════════════════════════════════════════════
        // ROUNDED PATH
        // ═══════════════════════════════════════════════════════════════

        private static GraphicsPath GetRoundedPath(Rectangle r, CornerRadiusEx cr)
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

        // ═══════════════════════════════════════════════════════════════
        // MISC HELPERS
        // ═══════════════════════════════════════════════════════════════

        private Bitmap CaptureControl(Control ctrl)
        {
            int w = Math.Max(1, ctrl.Width);
            int h = Math.Max(1, ctrl.Height);
            var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);

            if (ctrl.InvokeRequired)
                ctrl.Invoke(new Action(() =>
                    ctrl.DrawToBitmap(bmp, new Rectangle(0, 0, w, h))));
            else
                ctrl.DrawToBitmap(bmp, new Rectangle(0, 0, w, h));

            return bmp;
        }

        private static void EnableDoubleBuffering(Control control)
        {
            typeof(Control)
                .GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                ?.SetValue(control, true, null);

            foreach (Control child in control.Controls)
                EnableDoubleBuffering(child);
        }

        private void DisposeBitmaps()
        {
            _bmpOld?.Dispose(); _bmpOld = null;
            _bmpNew?.Dispose(); _bmpNew = null;
        }

        // ═══════════════════════════════════════════════════════════════
        // DISPOSE
        // ═══════════════════════════════════════════════════════════════

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Stop();
                _timer?.Dispose();
                DisposeBitmaps();
                InvalidateBlurCache();
            }
            base.Dispose(disposing);
        }
    }
}