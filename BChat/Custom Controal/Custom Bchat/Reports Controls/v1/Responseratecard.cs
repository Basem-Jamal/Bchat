using BChat.Custom_Controal.Custom_Bchat.Report;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat.Report
{
    // ════════════════════════════════════════════════════════════════════════
    //  NOTE: If CardTheme is already declared in another file inside this
    //        namespace (e.g. DailyMessagesCard.cs), remove the enum below.
    // ════════════════════════════════════════════════════════════════════════
    // public enum CardTheme { Dark, Light }

    [ToolboxItem(true)]
    [DefaultProperty("PercentValue")]
    [Description("A circular ring progress card for displaying rates, scores and ratio metrics.")]
    public class ResponseRateCard : Control
    {
        // ════════════════════════════════════════════════════════════════════
        //  PRIVATE BACKING FIELDS
        // ════════════════════════════════════════════════════════════════════

        // ── Appearance ────────────────────────────────────────────────────
        private CardTheme _theme = CardTheme.Dark;
        private Color _cardBackColor = Color.FromArgb(20, 25, 55);
        private Color _accentColor = Color.FromArgb(82, 215, 175);   // mint-teal
        private Color _accentColorEnd = Color.FromArgb(56, 163, 255);   // sky-blue
        private int _borderRadius = 22;
        private bool _showDotGrid = true;
        private int _dotGridOpacity = 15;

        // ── Content ───────────────────────────────────────────────────────
        private string _title = "معدل الاستجابة";
        private double _percentValue = 76.0;
        private string _centerLabel = "معدل الرد";
        private string _subLabel = "من إجمالي الرسائل";

        // ── Ring ──────────────────────────────────────────────────────────
        private int _ringThickness = 14;
        private bool _showBackgroundRing = true;
        private int _backgroundRingOpacity = 18;
        private bool _showGlowDot = true;
        private int _glowIntensity = 3;     // 1–5
        private bool _showInnerShimmer = true;

        // ── Trend ─────────────────────────────────────────────────────────
        private double _trendValue = 5.2;
        private bool _showTrend = true;
        private Color _trendPositiveColor = Color.FromArgb(16, 185, 129);
        private Color _trendNegativeColor = Color.FromArgb(239, 68, 68);
        private string _trendCompareLabel = "من الأسبوع الماضي";

        // ── Icon ──────────────────────────────────────────────────────────
        private bool _showIcon = true;
        private Image _iconImage = null;
        private Color _iconColor = Color.FromArgb(82, 215, 175);

        // ── Fonts ─────────────────────────────────────────────────────────
        private Font _titleFont;
        private Font _percentFont;
        private Font _labelFont;
        private Font _subLabelFont;
        private Font _trendFont;
        private bool _fontsOwned = false;

        // ════════════════════════════════════════════════════════════════════
        //  CONSTRUCTOR
        // ════════════════════════════════════════════════════════════════════
        public ResponseRateCard()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            RightToLeft = RightToLeft.Yes;
            Size = new Size(260, 270);
            BackColor = Color.Transparent;

            _titleFont = new Font("Cairo", 10f, FontStyle.Regular, GraphicsUnit.Point);
            _percentFont = new Font("Cairo", 30f, FontStyle.Bold, GraphicsUnit.Point);
            _labelFont = new Font("Cairo", 9f, FontStyle.Bold, GraphicsUnit.Point);
            _subLabelFont = new Font("Cairo", 8f, FontStyle.Regular, GraphicsUnit.Point);
            _trendFont = new Font("Cairo", 9f, FontStyle.Regular, GraphicsUnit.Point);
            _fontsOwned = true;

            ApplyTheme();
        }

        // ════════════════════════════════════════════════════════════════════
        //  THEME HELPERS
        // ════════════════════════════════════════════════════════════════════
        private void ApplyTheme()
        {
            _cardBackColor = _theme == CardTheme.Dark
                ? Color.FromArgb(20, 25, 55)
                : Color.FromArgb(248, 250, 252);
            Invalidate();
        }

        private Color TextColor => _theme == CardTheme.Dark
            ? Color.White : Color.FromArgb(15, 23, 42);

        private Color MutedColor => _theme == CardTheme.Dark
            ? Color.FromArgb(148, 163, 184) : Color.FromArgb(100, 116, 139);

        // ════════════════════════════════════════════════════════════════════
        //  DESIGNER PROPERTIES
        // ════════════════════════════════════════════════════════════════════

        // ── Appearance ────────────────────────────────────────────────────

        [Category("ResponseRate - Appearance")]
        [Description("Full card color scheme: Dark or Light.")]
        [DefaultValue(CardTheme.Dark)]
        public CardTheme Theme
        {
            get => _theme;
            set { _theme = value; ApplyTheme(); }
        }

        [Category("ResponseRate - Appearance")]
        [Description("Card background fill color.")]
        public Color CardBackColor
        {
            get => _cardBackColor;
            set { _cardBackColor = value; Invalidate(); }
        }

        [Category("ResponseRate - Appearance")]
        [Description("Primary accent color: ring arc, glow dot, icon, and center label.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("ResponseRate - Appearance")]
        [Description("Secondary accent color used for the inner shimmer highlight on the arc.")]
        public Color AccentColorEnd
        {
            get => _accentColorEnd;
            set { _accentColorEnd = value; Invalidate(); }
        }

        [Category("ResponseRate - Appearance")]
        [Description("Corner radius of the card.")]
        [DefaultValue(22)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("ResponseRate - Appearance")]
        [Description("Show a subtle dot grid texture in the card background.")]
        [DefaultValue(true)]
        public bool ShowDotGrid
        {
            get => _showDotGrid;
            set { _showDotGrid = value; Invalidate(); }
        }

        [Category("ResponseRate - Appearance")]
        [Description("Opacity (0–100) of the background dot grid.")]
        [DefaultValue(15)]
        public int DotGridOpacity
        {
            get => _dotGridOpacity;
            set { _dotGridOpacity = Clamp(value, 0, 100); Invalidate(); }
        }

        // ── Content ───────────────────────────────────────────────────────

        [Category("ResponseRate - Content")]
        [Description("Card title shown at the top right.")]
        [DefaultValue("معدل الاستجابة")]
        public string Title
        {
            get => _title;
            set { _title = value ?? string.Empty; Invalidate(); }
        }

        [Category("ResponseRate - Content")]
        [Description("Percentage value (0.0 – 100.0) represented by the ring arc.")]
        [DefaultValue(76.0)]
        public double PercentValue
        {
            get => _percentValue;
            set { _percentValue = ClampD(value, 0.0, 100.0); Invalidate(); }
        }

        [Category("ResponseRate - Content")]
        [Description("Label shown below the large percentage number inside the ring.")]
        [DefaultValue("معدل الرد")]
        public string CenterLabel
        {
            get => _centerLabel;
            set { _centerLabel = value ?? string.Empty; Invalidate(); }
        }

        [Category("ResponseRate - Content")]
        [Description("Smaller muted sub-label beneath the center label.")]
        [DefaultValue("من إجمالي الرسائل")]
        public string SubLabel
        {
            get => _subLabel;
            set { _subLabel = value ?? string.Empty; Invalidate(); }
        }

        // ── Ring ──────────────────────────────────────────────────────────

        [Category("ResponseRate - Ring")]
        [Description("Stroke thickness in pixels of the progress ring.")]
        [DefaultValue(14)]
        public int RingThickness
        {
            get => _ringThickness;
            set { _ringThickness = Math.Max(2, value); Invalidate(); }
        }

        [Category("ResponseRate - Ring")]
        [Description("Draw a faint full-circle ring behind the progress arc.")]
        [DefaultValue(true)]
        public bool ShowBackgroundRing
        {
            get => _showBackgroundRing;
            set { _showBackgroundRing = value; Invalidate(); }
        }

        [Category("ResponseRate - Ring")]
        [Description("Opacity (0–100) of the full background ring.")]
        [DefaultValue(18)]
        public int BackgroundRingOpacity
        {
            get => _backgroundRingOpacity;
            set { _backgroundRingOpacity = Clamp(value, 0, 100); Invalidate(); }
        }

        [Category("ResponseRate - Ring")]
        [Description("Show a glowing dot at the tip of the progress arc.")]
        [DefaultValue(true)]
        public bool ShowGlowDot
        {
            get => _showGlowDot;
            set { _showGlowDot = value; Invalidate(); }
        }

        [Category("ResponseRate - Ring")]
        [Description("Glow halo intensity at the arc tip (1 = subtle … 5 = intense).")]
        [DefaultValue(3)]
        public int GlowIntensity
        {
            get => _glowIntensity;
            set { _glowIntensity = Clamp(value, 1, 5); Invalidate(); }
        }

        [Category("ResponseRate - Ring")]
        [Description("Show a thin lighter shimmer stroke inside the arc for a 3-D look.")]
        [DefaultValue(true)]
        public bool ShowInnerShimmer
        {
            get => _showInnerShimmer;
            set { _showInnerShimmer = value; Invalidate(); }
        }

        // ── Trend ─────────────────────────────────────────────────────────

        [Category("ResponseRate - Trend")]
        [Description("Trend percentage. Positive ▲ green / Negative ▼ red.")]
        [DefaultValue(5.2)]
        public double TrendValue
        {
            get => _trendValue;
            set { _trendValue = value; Invalidate(); }
        }

        [Category("ResponseRate - Trend")]
        [Description("Show or hide the trend indicator at the bottom.")]
        [DefaultValue(true)]
        public bool ShowTrend
        {
            get => _showTrend;
            set { _showTrend = value; Invalidate(); }
        }

        [Category("ResponseRate - Trend")]
        [Description("Color used when the trend is positive (upward).")]
        public Color TrendPositiveColor
        {
            get => _trendPositiveColor;
            set { _trendPositiveColor = value; Invalidate(); }
        }

        [Category("ResponseRate - Trend")]
        [Description("Color used when the trend is negative (downward).")]
        public Color TrendNegativeColor
        {
            get => _trendNegativeColor;
            set { _trendNegativeColor = value; Invalidate(); }
        }

        [Category("ResponseRate - Trend")]
        [Description("Comparison period label appended to the trend text.")]
        [DefaultValue("من الأسبوع الماضي")]
        public string TrendCompareLabel
        {
            get => _trendCompareLabel;
            set { _trendCompareLabel = value ?? string.Empty; Invalidate(); }
        }

        // ── Icon ──────────────────────────────────────────────────────────

        [Category("ResponseRate - Icon")]
        [Description("Show or hide the icon at the top left.")]
        [DefaultValue(true)]
        public bool ShowIcon
        {
            get => _showIcon;
            set { _showIcon = value; Invalidate(); }
        }

        [Category("ResponseRate - Icon")]
        [Description("Custom image for the top-left icon. Null = built-in check-badge icon.")]
        [DefaultValue(null)]
        public Image IconImage
        {
            get => _iconImage;
            set { _iconImage = value; Invalidate(); }
        }

        [Category("ResponseRate - Icon")]
        [Description("Color of the built-in check-badge icon.")]
        public Color IconColor
        {
            get => _iconColor;
            set { _iconColor = value; Invalidate(); }
        }

        // ── Fonts ─────────────────────────────────────────────────────────

        [Category("ResponseRate - Fonts")]
        [Description("Font used for the card title.")]
        public Font TitleFont
        {
            get => _titleFont;
            set
            {
                if (_fontsOwned) _titleFont?.Dispose();
                _titleFont = value ?? new Font("Cairo", 10f, FontStyle.Regular, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        [Category("ResponseRate - Fonts")]
        [Description("Font used for the large percentage number inside the ring.")]
        public Font PercentFont
        {
            get => _percentFont;
            set
            {
                if (_fontsOwned) _percentFont?.Dispose();
                _percentFont = value ?? new Font("Cairo", 30f, FontStyle.Bold, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        [Category("ResponseRate - Fonts")]
        [Description("Font used for the center label (accent-colored).")]
        public Font LabelFont
        {
            get => _labelFont;
            set
            {
                if (_fontsOwned) _labelFont?.Dispose();
                _labelFont = value ?? new Font("Cairo", 9f, FontStyle.Bold, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        [Category("ResponseRate - Fonts")]
        [Description("Font used for the muted sub-label inside the ring.")]
        public Font SubLabelFont
        {
            get => _subLabelFont;
            set
            {
                if (_fontsOwned) _subLabelFont?.Dispose();
                _subLabelFont = value ?? new Font("Cairo", 8f, FontStyle.Regular, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        [Category("ResponseRate - Fonts")]
        [Description("Font used for the bottom trend indicator.")]
        public Font TrendFont
        {
            get => _trendFont;
            set
            {
                if (_fontsOwned) _trendFont?.Dispose();
                _trendFont = value ?? new Font("Cairo", 9f, FontStyle.Regular, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  PAINTING
        // ════════════════════════════════════════════════════════════════════
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = _borderRadius;
            float pad = 16f;

            // ── 1. Card background ──────────────────────────────────────────
            using (GraphicsPath bgPath = RoundedRect(bounds, r))
            using (SolidBrush bgBrush = new SolidBrush(_cardBackColor))
                g.FillPath(bgBrush, bgPath);

            // ── 2. Soft gradient overlay ────────────────────────────────────
            using (GraphicsPath gradPath = RoundedRect(bounds, r))
            using (LinearGradientBrush grad = new LinearGradientBrush(
                bounds,
                Color.FromArgb(_theme == CardTheme.Dark ? 20 : 8, Color.White),
                Color.FromArgb(0, Color.Black),
                LinearGradientMode.Vertical))
                g.FillPath(grad, gradPath);

            // Clip everything inside the card from now on
            using (GraphicsPath cardClip = RoundedRect(bounds, r))
                g.SetClip(cardClip);

            // ── 3. Dot grid background ──────────────────────────────────────
            if (_showDotGrid)
                PaintDotGrid(g);

            // ── 4. Title (top-right) ────────────────────────────────────────
            using (SolidBrush mb = new SolidBrush(MutedColor))
            {
                SizeF titleSz = g.MeasureString(_title, _titleFont);
                g.DrawString(_title, _titleFont, mb,
                    Width - pad - titleSz.Width, pad);
            }

            // ── 5. Icon (top-left) ──────────────────────────────────────────
            if (_showIcon)
            {
                float ix = pad;
                float iy = pad;
                if (_iconImage != null)
                    g.DrawImage(_iconImage, new RectangleF(ix, iy, 22f, 22f));
                else
                    PaintCheckBadgeIcon(g, ix, iy, 21f);
            }

            // ── 6. Compute ring geometry ────────────────────────────────────
            float topReserve = pad + g.MeasureString(_title, _titleFont).Height + 4f;
            float bottomReserve = _showTrend
                ? g.MeasureString("Ag", _trendFont).Height + pad * 1.4f
                : pad;

            float availW = Width - pad * 2f;
            float availH = Height - topReserve - bottomReserve;
            float ringSize = Math.Min(availW, availH) * 0.84f;
            float ringX = (Width - ringSize) / 2f;
            float ringY = topReserve + (availH - ringSize) / 2f;

            RectangleF ringRect = new RectangleF(ringX, ringY, ringSize, ringSize);

            // ── 7. Draw the ring ────────────────────────────────────────────
            PaintRing(g, ringRect);

            // ── 8. Center text (inside ring) ────────────────────────────────
            PaintCenterText(g, ringRect);

            // ── 9. Trend bar (bottom center) ────────────────────────────────
            if (_showTrend)
                PaintTrend(g, pad);

            g.ResetClip();
        }

        // ════════════════════════════════════════════════════════════════════
        //  PAINT HELPERS
        // ════════════════════════════════════════════════════════════════════

        // ── Dot Grid ─────────────────────────────────────────────────────────
        private void PaintDotGrid(Graphics g)
        {
            const int spacing = 20;
            const float dotR = 1.2f;
            int alpha = (int)(255.0 * _dotGridOpacity / 100.0);
            if (alpha <= 0) return;

            using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, MutedColor)))
            {
                for (int x = spacing; x < Width; x += spacing)
                    for (int y = spacing; y < Height; y += spacing)
                        g.FillEllipse(b, x - dotR, y - dotR, dotR * 2f, dotR * 2f);
            }
        }

        // ── Ring ─────────────────────────────────────────────────────────────
        private void PaintRing(Graphics g, RectangleF rect)
        {
            float startAngle = -90f;
            float sweepAngle = 360f * (float)(_percentValue / 100.0);

            // Background full ring (faint)
            if (_showBackgroundRing)
            {
                int bgAlpha = (int)(255.0 * _backgroundRingOpacity / 100.0);
                using (Pen bgPen = new Pen(Color.FromArgb(bgAlpha, _accentColor), _ringThickness))
                {
                    bgPen.StartCap = LineCap.Round;
                    bgPen.EndCap = LineCap.Round;
                    g.DrawEllipse(bgPen, rect);
                }
            }

            if (sweepAngle <= 0f) return;

            // Outer ambient glow band (wide, very transparent)
            int ambientAlpha = Clamp(_glowIntensity * 9, 5, 55);
            using (Pen ambientPen = new Pen(
                Color.FromArgb(ambientAlpha, _accentColor),
                _ringThickness + 10))
            {
                ambientPen.StartCap = LineCap.Flat;
                ambientPen.EndCap = LineCap.Flat;
                RectangleF expanded = new RectangleF(
                    rect.X - 5, rect.Y - 5,
                    rect.Width + 10, rect.Height + 10);
                g.DrawArc(ambientPen, expanded, startAngle, sweepAngle);
            }

            // Main progress arc stroke
            using (Pen arcPen = new Pen(_accentColor, _ringThickness))
            {
                arcPen.StartCap = LineCap.Round;
                arcPen.EndCap = LineCap.Round;
                g.DrawArc(arcPen, rect, startAngle, sweepAngle);
            }

            // Inner shimmer highlight (thin, lighter color, first 55% of arc only)
            if (_showInnerShimmer && sweepAngle > 5f)
            {
                Color shimColor = LightenColor(_accentColor, 0.45f);
                float shimSweep = sweepAngle * 0.55f;
                float shimInset = _ringThickness * 0.18f;

                RectangleF shimRect = new RectangleF(
                    rect.X + shimInset, rect.Y + shimInset,
                    rect.Width - shimInset * 2,
                    rect.Height - shimInset * 2);

                using (Pen shimPen = new Pen(
                    Color.FromArgb(160, shimColor),
                    _ringThickness * 0.28f))
                {
                    shimPen.StartCap = LineCap.Flat;
                    shimPen.EndCap = LineCap.Flat;
                    g.DrawArc(shimPen, shimRect, startAngle, shimSweep);
                }
            }

            // Glow dot at the arc tip
            if (_showGlowDot)
            {
                double endRad = (startAngle + sweepAngle) * Math.PI / 180.0;
                float cx = rect.X + rect.Width / 2f;
                float cy = rect.Y + rect.Height / 2f;
                float tx = cx + (rect.Width / 2f) * (float)Math.Cos(endRad);
                float ty = cy + (rect.Height / 2f) * (float)Math.Sin(endRad);

                // Concentric glow halos
                int layers = Clamp(_glowIntensity, 1, 5);
                for (int i = layers; i >= 1; i--)
                {
                    float glowR = _ringThickness * 0.65f * i;
                    int alpha = (int)(180.0 / (i * 1.5));
                    using (SolidBrush hb = new SolidBrush(Color.FromArgb(alpha, _accentColor)))
                        g.FillEllipse(hb, tx - glowR, ty - glowR, glowR * 2, glowR * 2);
                }

                // Bright white center pinpoint
                float dotR = _ringThickness * 0.42f;
                using (SolidBrush wb = new SolidBrush(Color.White))
                    g.FillEllipse(wb, tx - dotR, ty - dotR, dotR * 2, dotR * 2);

                // Tiny accent core
                float coreR = dotR * 0.5f;
                using (SolidBrush cb = new SolidBrush(_accentColor))
                    g.FillEllipse(cb, tx - coreR, ty - coreR, coreR * 2, coreR * 2);
            }
        }

        // ── Center Text ───────────────────────────────────────────────────────
        private void PaintCenterText(Graphics g, RectangleF ringRect)
        {
            float cx = ringRect.X + ringRect.Width / 2f;
            float cy = ringRect.Y + ringRect.Height / 2f;

            string pctText = $"{_percentValue:F0}%";

            SizeF pctSz = g.MeasureString(pctText, _percentFont);
            SizeF lblSz = g.MeasureString(_centerLabel, _labelFont);
            SizeF subSz = g.MeasureString(_subLabel, _subLabelFont);

            // Vertical block: percent + label + sub-label
            float lineGap = 2f;
            float blockH = pctSz.Height + lblSz.Height + subSz.Height - lineGap * 2;
            float startY = cy - blockH / 2f;

            // Percentage number (white / dark)
            using (SolidBrush tb = new SolidBrush(TextColor))
                g.DrawString(pctText, _percentFont, tb,
                    cx - pctSz.Width / 2f, startY);

            // Center label (accent color)
            float lblY = startY + pctSz.Height - lineGap;
            using (SolidBrush ab = new SolidBrush(_accentColor))
                g.DrawString(_centerLabel, _labelFont, ab,
                    cx - lblSz.Width / 2f, lblY);

            // Sub-label (muted)
            float subY = lblY + lblSz.Height - lineGap;
            using (SolidBrush mb = new SolidBrush(MutedColor))
                g.DrawString(_subLabel, _subLabelFont, mb,
                    cx - subSz.Width / 2f, subY);
        }

        // ── Trend Bar ─────────────────────────────────────────────────────────
        private void PaintTrend(Graphics g, float pad)
        {
            bool positive = _trendValue >= 0;
            string arrow = positive ? "▲" : "▼";
            string sign = positive ? "+" : "-";
            string trendText = $"{arrow} {sign}{Math.Abs(_trendValue):F1}%  {_trendCompareLabel}";
            Color trendCol = positive ? _trendPositiveColor : _trendNegativeColor;

            using (SolidBrush tb = new SolidBrush(trendCol))
            {
                SizeF sz = g.MeasureString(trendText, _trendFont);
                g.DrawString(trendText, _trendFont, tb,
                    (Width - sz.Width) / 2f,
                    Height - pad * 0.9f - sz.Height);
            }
        }

        // ── Check-Badge Icon ──────────────────────────────────────────────────
        private void PaintCheckBadgeIcon(Graphics g, float x, float y, float size)
        {
            using (Pen pen = new Pen(_iconColor, 1.5f))
            {
                // Outer circle
                g.DrawEllipse(pen, x, y, size, size);

                // Checkmark (two segments)
                float m = size * 0.22f;
                float cy = y + size / 2f;

                PointF p1 = new PointF(x + m, cy + size * 0.05f);
                PointF p2 = new PointF(x + size * 0.43f, cy + size * 0.26f);
                PointF p3 = new PointF(x + size - m, cy - size * 0.16f);

                using (Pen checkPen = new Pen(_iconColor, 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    g.DrawLine(checkPen, p1, p2);
                    g.DrawLine(checkPen, p2, p3);
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  GDI+ HELPERS
        // ════════════════════════════════════════════════════════════════════

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            int d = radius * 2;
            int x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height;
            var gp = new GraphicsPath();
            gp.AddArc(x, y, d, d, 180, 90);
            gp.AddArc(x + w - d, y, d, d, 270, 90);
            gp.AddArc(x + w - d, y + h - d, d, d, 0, 90);
            gp.AddArc(x, y + h - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }

        /// <summary>Lightens a Color by blending toward white by the given amount (0.0–1.0).</summary>
        private static Color LightenColor(Color color, float amount)
        {
            float r = Math.Min(255f, color.R + (255f - color.R) * amount);
            float g = Math.Min(255f, color.G + (255f - color.G) * amount);
            float b = Math.Min(255f, color.B + (255f - color.B) * amount);
            return Color.FromArgb(color.A, (int)r, (int)g, (int)b);
        }

        private static int Clamp(int v, int lo, int hi) => v < lo ? lo : v > hi ? hi : v;
        private static double ClampD(double v, double lo, double hi) => v < lo ? lo : v > hi ? hi : v;

        // ════════════════════════════════════════════════════════════════════
        //  OVERRIDES & DISPOSE
        // ════════════════════════════════════════════════════════════════════
        protected override Size DefaultSize => new Size(260, 270);

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _fontsOwned)
            {
                _titleFont?.Dispose();
                _percentFont?.Dispose();
                _labelFont?.Dispose();
                _subLabelFont?.Dispose();
                _trendFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}