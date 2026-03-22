// Controls/StatCard.cs
// ✅ بطاقة إحصائية مع شريط لون جانبي
// ✅ FontAwesome.Sharp 6.x
// ✅ كل الخصائص في Designer

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace BChat.Controls
{
    [ToolboxItem(true)]
    [DefaultEvent("Click")]
    [Description("بطاقة إحصائية مع شريط لون جانبي وأيقونة")]
    public class StatCard : Control
    {
        // ─── Fields 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        private Color _accentColor = Color.FromArgb(32, 201, 151);
        private int _accentWidth = 5;
        private bool _accentOnRight = true;
        private Color _cardColor = Color.White;
        private int _borderRadius = 18;
        private int _shadowDepth = 12;
        private Color _shadowColor = Color.FromArgb(30, 0, 0, 0);
        private IconChar _iconChar = IconChar.Bolt;
        private int _iconSize = 22;
        private Color _iconColor = Color.FromArgb(32, 201, 151);
        private Color _iconBgColor = Color.FromArgb(220, 245, 235);
        private int _iconBgSize = 48;
        private int _iconBgRadius = 999;
        private string _title = "العملاء النشطون";
        private string _value = "842";
        private Color _titleColor = Color.FromArgb(150, 160, 175);
        private Color _valueColor = Color.FromArgb(25, 35, 60);
        private float _titleFontSize = 10f;
        private float _valueFontSize = 22f;

        // ─── ✦ Accent Strip ───────────────────────────────────
        [Category("✦ Accent Strip"), Description("لون الشريط الجانبي")]
        public Color AccentColor
        { get => _accentColor; set { _accentColor = value; Invalidate(); } }

        [Category("✦ Accent Strip"), DefaultValue(5), Description("عرض الشريط بالبكسل")]
        public int AccentWidth
        { get => _accentWidth; set { _accentWidth = Math.Max(0, value); Invalidate(); } }

        [Category("✦ Accent Strip"), DefaultValue(true), Description("true=يمين | false=يسار")]
        public bool AccentOnRight
        { get => _accentOnRight; set { _accentOnRight = value; Invalidate(); } }

        // ─── ✦ Card 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        [Category("✦ Card"), Description("لون خلفية البطاقة")]
        public Color CardColor
        { get => _cardColor; set { _cardColor = value; Invalidate(); } }

        [Category("✦ Card"), DefaultValue(18), Description("نصف قطر الزوايا")]
        public int BorderRadius
        { get => _borderRadius; set { _borderRadius = Math.Max(0, value); Invalidate(); } }

        [Category("✦ Card"), DefaultValue(12), Description("عمق الظل")]
        public int ShadowDepth
        { get => _shadowDepth; set { _shadowDepth = Math.Max(0, value); Invalidate(); } }

        [Category("✦ Card"), Description("لون الظل")]
        public Color ShadowColor
        { get => _shadowColor; set { _shadowColor = value; Invalidate(); } }

        // ─── ✦ Icon 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        [Category("✦ Icon"), DefaultValue(IconChar.Bolt), Description("أيقونة Font Awesome")]
        public IconChar IconChar
        { get => _iconChar; set { _iconChar = value; Invalidate(); } }

        [Category("✦ Icon"), DefaultValue(22), Description("حجم الأيقونة")]
        public int IconSize
        { get => _iconSize; set { _iconSize = Math.Max(8, value); Invalidate(); } }

        [Category("✦ Icon"), Description("لون الأيقونة")]
        public Color IconColor
        { get => _iconColor; set { _iconColor = value; Invalidate(); } }

        [Category("✦ Icon"), Description("لون خلفية دائرة الأيقونة")]
        public Color IconBgColor
        { get => _iconBgColor; set { _iconBgColor = value; Invalidate(); } }

        [Category("✦ Icon"), DefaultValue(48), Description("حجم دائرة الأيقونة")]
        public int IconBgSize
        { get => _iconBgSize; set { _iconBgSize = Math.Max(16, value); Invalidate(); } }

        [Category("✦ Icon"), DefaultValue(999), Description("نصف قطر دائرة الأيقونة — 999=دائرة كاملة")]
        public int IconBgRadius
        { get => _iconBgRadius; set { _iconBgRadius = Math.Max(0, value); Invalidate(); } }

        // ─── ✦ Text 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        [Category("✦ Text"), Description("العنوان (السطر الأول)")]
        public string Title
        { get => _title; set { _title = value; Invalidate(); } }

        [Category("✦ Text"), Description("القيمة أو الرقم")]
        public string Value
        { get => _value; set { _value = value; Invalidate(); } }

        [Category("✦ Text"), Description("لون العنوان")]
        public Color TitleColor
        { get => _titleColor; set { _titleColor = value; Invalidate(); } }

        [Category("✦ Text"), Description("لون القيمة")]
        public Color ValueColor
        { get => _valueColor; set { _valueColor = value; Invalidate(); } }

        [Category("✦ Text"), DefaultValue(10f), Description("حجم خط العنوان")]
        public float TitleFontSize
        { get => _titleFontSize; set { _titleFontSize = Math.Max(6f, value); Invalidate(); } }

        [Category("✦ Text"), DefaultValue(22f), Description("حجم خط القيمة")]
        public float ValueFontSize
        { get => _valueFontSize; set { _valueFontSize = Math.Max(8f, value); Invalidate(); } }

        // ─── Constructor ──────────────────────────────────────
        public StatCard()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);

            Size = new Size(300, 90);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        // ─── Paint 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // 1. خلفية الـ Parent
            if (Parent != null)
            {
                g.TranslateTransform(-Left, -Top);
                var pa = new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height));
                InvokePaintBackground(Parent, pa);
                InvokePaint(Parent, pa);
                g.TranslateTransform(Left, Top);
            }

            int sd = _shadowDepth;
            var card = new Rectangle(sd, sd, Width - sd * 2 - 1, Height - sd * 2 - 1);
            if (card.Width <= 0 || card.Height <= 0) return;

            // 2. الظل
            DrawShadow(g, card);

            // 3. البطاقة
            using var cardPath = RoundedRect(card, _borderRadius);
            using (var br = new SolidBrush(_cardColor))
                g.FillPath(br, cardPath);

            // 4. الشريط الجانبي
            if (_accentWidth > 0)
            {
                int aw = Math.Min(_accentWidth, card.Width / 2);
                var strip = _accentOnRight
                    ? new Rectangle(card.Right - aw, card.Top, aw, card.Height)
                    : new Rectangle(card.Left, card.Top, aw, card.Height);

                g.SetClip(cardPath);
                using (var br = new SolidBrush(_accentColor))
                    g.FillRectangle(br, strip);
                g.ResetClip();
            }

            // 5. حافة خفيفة
            using (var pen = new Pen(Color.FromArgb(12, 0, 0, 0), 1f))
                g.DrawPath(pen, cardPath);

            // 6. المحتوى
            DrawContent(g, card);
        }

        // ─── Content ──────────────────────────────────────────
        private void DrawContent(Graphics g, Rectangle card)
        {
            int pad = 16;
            int awR = _accentOnRight ? _accentWidth + 4 : 0;
            int awL = _accentOnRight ? 0 : _accentWidth + 4;

            // ── دائرة الأيقونة ─────────────────────────────────
            int bgS = _iconBgSize;
            int bgR = Math.Min(_iconBgRadius, bgS / 2);
            float ix = card.Left + pad + awL;
            float iy = card.Top + (card.Height - bgS) / 2f;

            using (var path = RoundedRectF(new RectangleF(ix, iy, bgS, bgS), bgR))
            using (var br = new SolidBrush(_iconBgColor))
                g.FillPath(br, path);

            try
            {
                using var bmp = _iconChar.ToBitmap(_iconColor, _iconSize);
                g.DrawImage(bmp,
                    ix + (bgS - _iconSize) / 2f,
                    iy + (bgS - _iconSize) / 2f,
                    _iconSize, _iconSize);
            }
            catch { }

            // ── النصوص (محاذاة يمين) ───────────────────────────
            float textRight = card.Right - pad - awR;
            float midY = card.Top + card.Height / 2f;

            using var titleFont = new Font("Segoe UI", _titleFontSize, FontStyle.Regular);
            using var titleBr = new SolidBrush(_titleColor);
            var tSz = g.MeasureString(_title, titleFont);
            g.DrawString(_title, titleFont, titleBr,
                new PointF(textRight - tSz.Width, midY - tSz.Height - 1));

            using var valueFont = new Font("Segoe UI", _valueFontSize, FontStyle.Bold);
            using var valueBr = new SolidBrush(_valueColor);
            var vSz = g.MeasureString(_value, valueFont);
            g.DrawString(_value, valueFont, valueBr,
                new PointF(textRight - vSz.Width, midY + 2));
        }

        // ─── Shadow 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        private void DrawShadow(Graphics g, Rectangle card)
        {
            int depth = _shadowDepth;
            if (depth <= 0) return;
            for (int i = depth; i >= 1; i--)
            {
                var sr = new Rectangle(card.Left - i, card.Top + i,
                                          card.Width + i * 2, card.Height + i * 2);
                float t = 1f - (i / (float)depth);
                int alpha = (int)(_shadowColor.A * t * t * 0.4f);
                alpha = Math.Max(0, Math.Min(255, alpha));
                if (alpha == 0) continue;
                using var path = RoundedRect(sr, _borderRadius + i);
                using var br = new SolidBrush(Color.FromArgb(alpha,
                                    _shadowColor.R, _shadowColor.G, _shadowColor.B));
                g.FillPath(br, path);
            }
        }

        // ─── Helpers ──────────────────────────────────────────
        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            int rad = Math.Max(0, Math.Min(radius, Math.Min(r.Width, r.Height) / 2));
            int d = rad * 2;
            if (d <= 0) { p.AddRectangle(r); return p; }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static GraphicsPath RoundedRectF(RectangleF r, int radius)
        {
            var p = new GraphicsPath();
            int rad = Math.Max(0, Math.Min(radius, (int)Math.Min(r.Width, r.Height) / 2));
            float d = rad * 2;
            if (d <= 0) { p.AddRectangle(r); return p; }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}