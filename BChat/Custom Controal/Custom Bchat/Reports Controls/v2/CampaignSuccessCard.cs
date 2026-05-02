using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat.Report
{
    // NOTE: CardTheme enum is declared in MonthlyMessagesCard.cs
    // If using standalone, uncomment:
    // public enum CardTheme { Dark, Light }

    [ToolboxItem(true)]
    [DefaultProperty("PercentValue")]
    [Description("Circular ring progress card showing campaign success rate.")]
    public class CampaignSuccessCard : Control
    {
        // ─────────────────────────────────────────────
        //  Fields
        // ─────────────────────────────────────────────
        private CardTheme _theme         = CardTheme.Dark;
        private Color     _cardBackColor = Color.FromArgb(20, 25, 55);
        private Color     _accentColor   = Color.FromArgb(124, 111, 247);
        private Color     _ringBgColor   = Color.FromArgb(40, 45, 80);
        private int       _borderRadius  = 20;
        private int       _dotGridOpacity = 15;

        private string _title        = "نسبة نجاح الحملات";
        private double _percentValue = 94.0;
        private int    _ringThickness = 14;

        private string _badgeText  = "تم تسليم 12,400 رسالة بنجاح";
        private bool   _showBadge  = true;
        private Color  _badgeColor = Color.FromArgb(16, 185, 129);

        private Font _titleFont;
        private Font _percentFont;
        private Font _badgeFont;
        private bool _fontsOwned = false;

        // ─────────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────────
        public CampaignSuccessCard()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint  |
                ControlStyles.UserPaint             |
                ControlStyles.ResizeRedraw          |
                ControlStyles.SupportsTransparentBackColor,
                true);

            RightToLeft = RightToLeft.Yes;
            BackColor   = Color.Transparent;
            Size        = new Size(260, 260);

            _titleFont   = new Font("Cairo", 11f, FontStyle.Bold,    GraphicsUnit.Point);
            _percentFont = new Font("Cairo", 32f, FontStyle.Bold,    GraphicsUnit.Point);
            _badgeFont   = new Font("Cairo",  9f, FontStyle.Regular, GraphicsUnit.Point);
            _fontsOwned  = true;

            ApplyTheme();
        }

        // ─────────────────────────────────────────────
        //  Properties — Appearance
        // ─────────────────────────────────────────────
        [Category("CampaignCard - Appearance")]
        [Description("Dark or Light card theme.")]
        [DefaultValue(CardTheme.Dark)]
        public CardTheme Theme
        {
            get => _theme;
            set { _theme = value; ApplyTheme(); }
        }

        [Category("CampaignCard - Appearance")]
        [Description("Ring accent color.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("CampaignCard - Appearance")]
        [Description("Ring track (background) color.")]
        public Color RingBgColor
        {
            get => _ringBgColor;
            set { _ringBgColor = value; Invalidate(); }
        }

        [Category("CampaignCard - Appearance")]
        [Description("Card corner radius.")]
        [DefaultValue(20)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Clamp(value, 0, 60); Invalidate(); }
        }

        [Category("CampaignCard - Appearance")]
        [Description("Ring stroke thickness in pixels.")]
        [DefaultValue(14)]
        public int RingThickness
        {
            get => _ringThickness;
            set { _ringThickness = Clamp(value, 4, 40); Invalidate(); }
        }

        [Category("CampaignCard - Appearance")]
        [Description("Dot grid texture opacity (0=off, 100=full).")]
        [DefaultValue(15)]
        public int DotGridOpacity
        {
            get => _dotGridOpacity;
            set { _dotGridOpacity = Clamp(value, 0, 100); Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Content
        // ─────────────────────────────────────────────
        [Category("CampaignCard - Content")]
        [Description("Card title displayed at the top.")]
        [DefaultValue("نسبة نجاح الحملات")]
        public string Title
        {
            get => _title;
            set { _title = value ?? ""; Invalidate(); }
        }

        [Category("CampaignCard - Content")]
        [Description("Percentage value (0–100).")]
        [DefaultValue(94.0)]
        public double PercentValue
        {
            get => _percentValue;
            set { _percentValue = ClampD(value, 0.0, 100.0); Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Badge
        // ─────────────────────────────────────────────
        [Category("CampaignCard - Badge")]
        [Description("Show the success badge at the bottom.")]
        [DefaultValue(true)]
        public bool ShowBadge
        {
            get => _showBadge;
            set { _showBadge = value; Invalidate(); }
        }

        [Category("CampaignCard - Badge")]
        [Description("Badge label text.")]
        [DefaultValue("تم تسليم 12,400 رسالة بنجاح")]
        public string BadgeText
        {
            get => _badgeText;
            set { _badgeText = value ?? ""; Invalidate(); }
        }

        [Category("CampaignCard - Badge")]
        [Description("Badge text and icon color.")]
        public Color BadgeColor
        {
            get => _badgeColor;
            set { _badgeColor = value; Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Fonts
        // ─────────────────────────────────────────────
        [Category("CampaignCard - Fonts")]
        [Description("Font for the card title.")]
        public Font TitleFont
        {
            get => _titleFont;
            set
            {
                if (_fontsOwned) _titleFont?.Dispose();
                _titleFont  = value ?? new Font("Cairo", 11f, FontStyle.Bold, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        [Category("CampaignCard - Fonts")]
        [Description("Font for the percentage number.")]
        public Font PercentFont
        {
            get => _percentFont;
            set
            {
                if (_fontsOwned) _percentFont?.Dispose();
                _percentFont = value ?? new Font("Cairo", 32f, FontStyle.Bold, GraphicsUnit.Point);
                _fontsOwned  = false;
                Invalidate();
            }
        }

        [Category("CampaignCard - Fonts")]
        [Description("Font for the badge text.")]
        public Font BadgeFont
        {
            get => _badgeFont;
            set
            {
                if (_fontsOwned) _badgeFont?.Dispose();
                _badgeFont  = value ?? new Font("Cairo", 9f, FontStyle.Regular, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        // ─────────────────────────────────────────────
        //  Theme
        // ─────────────────────────────────────────────
        private void ApplyTheme()
        {
            _cardBackColor = _theme == CardTheme.Dark
                ? Color.FromArgb(20, 25, 55)
                : Color.FromArgb(248, 250, 252);
            _ringBgColor = _theme == CardTheme.Dark
                ? Color.FromArgb(40, 45, 80)
                : Color.FromArgb(220, 220, 240);
            Invalidate();
        }

        private Color TextColor  => _theme == CardTheme.Dark ? Color.White : Color.FromArgb(15, 23, 42);
        private Color MutedColor => _theme == CardTheme.Dark ? Color.FromArgb(148, 163, 184) : Color.FromArgb(100, 116, 139);

        // ─────────────────────────────────────────────
        //  OnPaint
        // ─────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode   = PixelOffsetMode.HighQuality;

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            int   r   = _borderRadius;
            float pad = 16f;

            // Step 1: Background
            using (GraphicsPath bgPath = RoundedRect(bounds, r))
            using (SolidBrush bgBrush = new SolidBrush(_cardBackColor))
                g.FillPath(bgBrush, bgPath);

            // Step 2: Gradient overlay
            using (GraphicsPath gradPath = RoundedRect(bounds, r))
            using (LinearGradientBrush grad = new LinearGradientBrush(
                bounds,
                Color.FromArgb(_theme == CardTheme.Dark ? 22 : 10, Color.White),
                Color.FromArgb(0, Color.Black),
                LinearGradientMode.Vertical))
                g.FillPath(grad, gradPath);

            // Step 3: SetClip
            using (GraphicsPath clip = RoundedRect(bounds, r))
                g.SetClip(clip);

            // Step 4: Dot grid
            PaintDotGrid(g);

            // Step 5: Title (centered top)
            using (SolidBrush tb = new SolidBrush(TextColor))
            {
                SizeF tsz = g.MeasureString(_title, _titleFont);
                g.DrawString(_title, _titleFont, tb,
                    (Width - tsz.Width) / 2f, pad);
            }

            // Step 6: Ring
            float titleH  = g.MeasureString(_title, _titleFont).Height;
            float ringTop = pad + titleH + 10f;
            float badgeH  = _showBadge ? 36f : 0f;
            float ringSize = Math.Min(Width - pad * 4f, Height - ringTop - badgeH - pad * 2f);
            float ringLeft = (Width  - ringSize) / 2f;
            RectangleF ringRect = new RectangleF(
                ringLeft + _ringThickness / 2f,
                ringTop  + _ringThickness / 2f,
                ringSize - _ringThickness,
                ringSize - _ringThickness);

            // Background ring
            using (Pen bgPen = new Pen(_ringBgColor, _ringThickness))
                g.DrawEllipse(bgPen, ringRect);

            // Progress arc
            float sweep = 360f * (float)(_percentValue / 100.0);
            using (Pen arcPen = new Pen(_accentColor, _ringThickness)
                { StartCap = LineCap.Round, EndCap = LineCap.Round })
                g.DrawArc(arcPen, ringRect, -90f, sweep);

            // Glow halo on arc tip
            double tipRad = (-90.0 + sweep) * Math.PI / 180.0;
            float  cx     = ringRect.X + ringRect.Width  / 2f;
            float  cy2    = ringRect.Y + ringRect.Height / 2f;
            float  rx     = ringRect.Width  / 2f;
            float  ry     = ringRect.Height / 2f;
            float  tx     = cx + rx * (float)Math.Cos(tipRad);
            float  ty2    = cy2 + ry * (float)Math.Sin(tipRad);
            for (int i = 3; i >= 1; i--)
            {
                int a = i == 3 ? 25 : i == 2 ? 45 : 80;
                float s = _ringThickness * 0.5f * i;
                using (SolidBrush halo = new SolidBrush(Color.FromArgb(a, _accentColor)))
                    g.FillEllipse(halo, tx - s, ty2 - s, s * 2f, s * 2f);
            }

            // Percent text centered in ring
            string pctTxt = $"{_percentValue:F0}%";
            SizeF  pctSz  = g.MeasureString(pctTxt, _percentFont);
            float  pcx    = (Width  - pctSz.Width)  / 2f;
            float  pcy    = ringTop + ringSize / 2f - pctSz.Height / 2f;
            using (SolidBrush pb = new SolidBrush(TextColor))
                g.DrawString(pctTxt, _percentFont, pb, pcx, pcy);

            // Step 7: Badge
            if (_showBadge)
            {
                float badgeY = ringTop + ringSize + pad * 0.5f;
                PaintCheckBadge(g, badgeY, pad);
            }

            g.ResetClip();
        }

        private void PaintCheckBadge(Graphics g, float badgeY, float pad)
        {
            string txt = _badgeText;
            SizeF  tsz = g.MeasureString(txt, _badgeFont);
            float  iconSize = 16f;
            float  totalW   = iconSize + 6f + tsz.Width;
            float  startX   = (Width - totalW) / 2f;
            float  textX    = startX;
            float  iconX    = startX + tsz.Width + 6f;
            float  baseY    = badgeY + 4f;

            // Check icon (circle + checkmark)
            using (Pen cp = new Pen(_badgeColor, 1.4f))
                g.DrawEllipse(cp, iconX, baseY, iconSize, iconSize);
            float m   = iconSize * 0.22f;
            float mcy = baseY + iconSize / 2f;
            PointF p1 = new PointF(iconX + m,               mcy + iconSize * 0.05f);
            PointF p2 = new PointF(iconX + iconSize * 0.43f, mcy + iconSize * 0.26f);
            PointF p3 = new PointF(iconX + iconSize - m,    mcy - iconSize * 0.16f);
            using (Pen cp2 = new Pen(_badgeColor, 1.6f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            { g.DrawLine(cp2, p1, p2); g.DrawLine(cp2, p2, p3); }

            // Badge text
            using (SolidBrush tb = new SolidBrush(_badgeColor))
                g.DrawString(txt, _badgeFont, tb, textX, baseY + (iconSize - tsz.Height) / 2f);
        }

        private void PaintDotGrid(Graphics g)
        {
            const int   spacing = 22;
            const float dotR    = 1.1f;
            int alpha = (int)(255.0 * _dotGridOpacity / 100.0);
            if (alpha <= 0) return;
            using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, MutedColor)))
            {
                for (int x = spacing; x < Width;  x += spacing)
                for (int y = spacing; y < Height; y += spacing)
                    g.FillEllipse(b, x - dotR, y - dotR, dotR * 2f, dotR * 2f);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            int d = radius * 2;
            int x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height;
            var gp = new GraphicsPath();
            gp.AddArc(x,         y,         d, d, 180, 90);
            gp.AddArc(x + w - d, y,         d, d, 270, 90);
            gp.AddArc(x + w - d, y + h - d, d, d,   0, 90);
            gp.AddArc(x,         y + h - d, d, d,  90, 90);
            gp.CloseFigure();
            return gp;
        }

        private static int    Clamp (int    v, int    lo, int    hi) => v < lo ? lo : v > hi ? hi : v;
        private static double ClampD(double v, double lo, double hi) => v < lo ? lo : v > hi ? hi : v;

        protected override Size DefaultSize => new Size(260, 260);

        protected override void OnResize(EventArgs e) { base.OnResize(e); Invalidate(); }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _fontsOwned)
            {
                _titleFont?.Dispose();
                _percentFont?.Dispose();
                _badgeFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
