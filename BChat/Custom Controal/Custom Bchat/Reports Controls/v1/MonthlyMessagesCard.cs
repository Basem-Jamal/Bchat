using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat.Report
{
    public enum CardTheme { Dark, Light }

    [ToolboxItem(true)]
    [DefaultProperty("MessageCount")]
    [Description("Analytics card showing monthly message stats with a 30-day area chart.")]
    public class MonthlyMessagesCard : Control
    {
        // ── Private backing fields ─────────────────────────────────────────────
        private CardTheme _theme = CardTheme.Dark;
        private Color _cardBackColor = Color.FromArgb(26, 31, 60);
        private Color _accentColor = Color.FromArgb(124, 111, 247);
        private int _borderRadius = 20;
        private bool _showLeftAccentBorder = true;
        private int _leftAccentBorderWidth = 4;

        private string _title = "رسائل الشهر";
        private int _messageCount = 38450;
        private double _trendValue = 8.5;
        private Color _trendPositiveColor = Color.FromArgb(16, 185, 129);
        private Color _trendNegativeColor = Color.FromArgb(239, 68, 68);
        private bool _showTrend = true;

        // 30 قيمة — واحدة لكل يوم في الشهر
        private int[] _chartData = {
            120, 200, 180, 250, 300, 270, 310,
            280, 320, 290, 350, 400, 380, 420,
            390, 430, 410, 460, 440, 480, 500,
            470, 510, 490, 530, 520, 560, 540, 580, 600
        };

        private bool _showChart = true;
        private Color _chartFillColor = Color.FromArgb(124, 111, 247);
        private int _chartFillOpacity = 25;
        private bool _showAverageLine = true;
        private int _highlightDay = -1;   // -1 = آخر يوم

        private bool _showIcon = true;
        private Image _iconImage = null;
        private Color _iconColor = Color.FromArgb(124, 111, 247);

        private Font _titleFont;
        private Font _countFont;
        private Font _trendFont;
        private bool _fontsOwned = false;

        // ── Constructor ───────────────────────────────────────────────────────
        public MonthlyMessagesCard()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            RightToLeft = RightToLeft.Yes;
            Size = new Size(320, 180);
            BackColor = Color.Transparent;

            _titleFont = new Font("Cairo", 10f, FontStyle.Regular, GraphicsUnit.Point);
            _countFont = new Font("Cairo", 28f, FontStyle.Bold, GraphicsUnit.Point);
            _trendFont = new Font("Cairo", 9f, FontStyle.Regular, GraphicsUnit.Point);
            _fontsOwned = true;

            ApplyTheme();
        }

        // ── Theme helper ──────────────────────────────────────────────────────
        private void ApplyTheme()
        {
            _cardBackColor = _theme == CardTheme.Dark
                ? Color.FromArgb(26, 31, 60)
                : Color.White;
            Invalidate();
        }

        private Color TextColor => _theme == CardTheme.Dark
            ? Color.White
            : Color.FromArgb(15, 23, 42);

        private Color MutedColor => _theme == CardTheme.Dark
            ? Color.FromArgb(148, 163, 184)
            : Color.FromArgb(100, 116, 139);

        // ════════════════════════════════════════════════════════════════════
        //  DESIGNER PROPERTIES
        // ════════════════════════════════════════════════════════════════════

        // ── Appearance ────────────────────────────────────────────────────────
        [Category("Monthly - Appearance")]
        [Description("Switches the full color scheme between Dark and Light.")]
        [DefaultValue(CardTheme.Dark)]
        public CardTheme Theme
        {
            get => _theme;
            set { _theme = value; ApplyTheme(); }
        }

        [Category("Monthly - Appearance")]
        [Description("Background color of the card.")]
        public Color CardBackColor
        {
            get => _cardBackColor;
            set { _cardBackColor = value; Invalidate(); }
        }

        [Category("Monthly - Appearance")]
        [Description("Accent color used for the left border, chart line and icon.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Monthly - Appearance")]
        [Description("Corner radius of the card background.")]
        [DefaultValue(20)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("Monthly - Appearance")]
        [Description("Show the accent border on the left edge.")]
        [DefaultValue(true)]
        public bool ShowLeftAccentBorder
        {
            get => _showLeftAccentBorder;
            set { _showLeftAccentBorder = value; Invalidate(); }
        }

        [Category("Monthly - Appearance")]
        [Description("Width (px) of the left accent border.")]
        [DefaultValue(4)]
        public int LeftAccentBorderWidth
        {
            get => _leftAccentBorderWidth;
            set { _leftAccentBorderWidth = Math.Max(1, value); Invalidate(); }
        }

        // ── Content ───────────────────────────────────────────────────────────
        [Category("Monthly - Content")]
        [Description("Card title displayed at top-right.")]
        [DefaultValue("رسائل الشهر")]
        public string Title
        {
            get => _title;
            set { _title = value ?? string.Empty; Invalidate(); }
        }

        [Category("Monthly - Content")]
        [Description("Total monthly message count (displayed with comma separator).")]
        [DefaultValue(38450)]
        public int MessageCount
        {
            get => _messageCount;
            set { _messageCount = value; Invalidate(); }
        }

        [Category("Monthly - Content")]
        [Description("Percentage trend vs last month. Positive = ▲ green, Negative = ▼ red.")]
        [DefaultValue(8.5)]
        public double TrendValue
        {
            get => _trendValue;
            set { _trendValue = value; Invalidate(); }
        }

        [Category("Monthly - Content")]
        [Description("Color used for positive trend indicator.")]
        public Color TrendPositiveColor
        {
            get => _trendPositiveColor;
            set { _trendPositiveColor = value; Invalidate(); }
        }

        [Category("Monthly - Content")]
        [Description("Color used for negative trend indicator.")]
        public Color TrendNegativeColor
        {
            get => _trendNegativeColor;
            set { _trendNegativeColor = value; Invalidate(); }
        }

        [Category("Monthly - Content")]
        [Description("Show or hide the trend indicator row.")]
        [DefaultValue(true)]
        public bool ShowTrend
        {
            get => _showTrend;
            set { _showTrend = value; Invalidate(); }
        }

        // ── Chart ─────────────────────────────────────────────────────────────
        [Category("Monthly - Chart")]
        [Description("Array of daily values (30 recommended). Drives the area chart.")]
        public int[] ChartData
        {
            get => _chartData;
            set
            {
                _chartData = value ?? new int[] {
                    120,200,180,250,300,270,310,
                    280,320,290,350,400,380,420,
                    390,430,410,460,440,480,500,
                    470,510,490,530,520,560,540,580,600
                };
                Invalidate();
            }
        }

        [Category("Monthly - Chart")]
        [Description("Show or hide the area chart.")]
        [DefaultValue(true)]
        public bool ShowChart
        {
            get => _showChart;
            set { _showChart = value; Invalidate(); }
        }

        [Category("Monthly - Chart")]
        [Description("Fill color under the area chart line.")]
        public Color ChartFillColor
        {
            get => _chartFillColor;
            set { _chartFillColor = value; Invalidate(); }
        }

        [Category("Monthly - Chart")]
        [Description("Alpha opacity (0-100) for the area fill under the chart line.")]
        [DefaultValue(25)]
        public int ChartFillOpacity
        {
            get => _chartFillOpacity;
            set { _chartFillOpacity = Math.Clamp(value, 0, 100); Invalidate(); }
        }

        [Category("Monthly - Chart")]
        [Description("Draw a dashed horizontal line at the daily average value.")]
        [DefaultValue(true)]
        public bool ShowAverageLine
        {
            get => _showAverageLine;
            set { _showAverageLine = value; Invalidate(); }
        }

        [Category("Monthly - Chart")]
        [Description("Day index (0-based) to highlight with a glow dot. -1 = last day.")]
        [DefaultValue(-1)]
        public int HighlightDay
        {
            get => _highlightDay;
            set { _highlightDay = value; Invalidate(); }
        }

        // ── Icon ──────────────────────────────────────────────────────────────
        [Category("Monthly - Icon")]
        [Description("Show or hide the icon at bottom-left.")]
        [DefaultValue(true)]
        public bool ShowIcon
        {
            get => _showIcon;
            set { _showIcon = value; Invalidate(); }
        }

        [Category("Monthly - Icon")]
        [Description("Custom image shown at bottom-left. When null the built-in calendar icon is drawn.")]
        [DefaultValue(null)]
        public Image IconImage
        {
            get => _iconImage;
            set { _iconImage = value; Invalidate(); }
        }

        [Category("Monthly - Icon")]
        [Description("Color of the built-in calendar icon.")]
        public Color IconColor
        {
            get => _iconColor;
            set { _iconColor = value; Invalidate(); }
        }

        // ── Fonts ─────────────────────────────────────────────────────────────
        [Category("Monthly - Fonts")]
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

        [Category("Monthly - Fonts")]
        [Description("Font used for the large message count number.")]
        public Font CountFont
        {
            get => _countFont;
            set
            {
                if (_fontsOwned) _countFont?.Dispose();
                _countFont = value ?? new Font("Cairo", 28f, FontStyle.Bold, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        [Category("Monthly - Fonts")]
        [Description("Font used for the trend indicator text.")]
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

            // 1. Rounded background ───────────────────────────────────────────
            using (GraphicsPath bgPath = RoundedRect(bounds, r))
            using (SolidBrush bgBrush = new SolidBrush(_cardBackColor))
                g.FillPath(bgBrush, bgPath);

            // 2. Left accent border ───────────────────────────────────────────
            if (_showLeftAccentBorder)
            {
                int bw = _leftAccentBorderWidth;
                using (GraphicsPath clipPath = RoundedRect(bounds, r))
                {
                    g.SetClip(clipPath);
                    using (SolidBrush accentBrush = new SolidBrush(_accentColor))
                    {
                        g.FillRectangle(accentBrush, 0, r, bw, Height - r * 2);
                        g.FillPie(accentBrush, 0, 0, r * 2, r * 2, 180, 90);
                        g.FillRectangle(accentBrush, 0, 0, bw, r);
                        g.FillRectangle(accentBrush, 0, Height - r, bw, r);
                    }
                    g.ResetClip();
                }
            }

            // 3. Subtle gradient overlay ──────────────────────────────────────
            using (GraphicsPath gradPath = RoundedRect(bounds, r))
            {
                Color topColor = _theme == CardTheme.Dark
                    ? Color.FromArgb(20, Color.White)
                    : Color.FromArgb(8, Color.White);
                Color botColor = Color.FromArgb(0, Color.Black);

                using (LinearGradientBrush gradBrush = new LinearGradientBrush(
                    bounds, topColor, botColor, LinearGradientMode.Vertical))
                    g.FillPath(gradBrush, gradPath);
            }

            // Clip everything inside the card
            using (GraphicsPath clipCard = RoundedRect(bounds, r))
                g.SetClip(clipCard);

            // Layout: respect left accent border
            float contentRight = Width - pad;

            // 4. Title (top-right, muted) ─────────────────────────────────────
            using (SolidBrush mutedBrush = new SolidBrush(MutedColor))
            {
                SizeF titleSize = g.MeasureString(_title, _titleFont);
                g.DrawString(_title, _titleFont, mutedBrush,
                    contentRight - titleSize.Width, pad);
            }

            // 5. Large count number ───────────────────────────────────────────
            string countText = _messageCount.ToString("N0");
            using (SolidBrush textBrush = new SolidBrush(TextColor))
            {
                SizeF countSize = g.MeasureString(countText, _countFont);
                float cy = Height * 0.26f;
                g.DrawString(countText, _countFont, textBrush,
                    contentRight - countSize.Width, cy);

                // 6. Trend indicator ──────────────────────────────────────────
                if (_showTrend)
                {
                    bool positive = _trendValue >= 0;
                    string trendText = positive
                        ? $"▲ +{Math.Abs(_trendValue):F0}% من الشهر الماضي"
                        : $"▼ -{Math.Abs(_trendValue):F0}% من الشهر الماضي";

                    Color trendColor = positive ? _trendPositiveColor : _trendNegativeColor;
                    using (SolidBrush trendBrush = new SolidBrush(trendColor))
                    {
                        SizeF trendSize = g.MeasureString(trendText, _trendFont);
                        g.DrawString(trendText, _trendFont, trendBrush,
                            contentRight - trendSize.Width,
                            cy + countSize.Height - 2f);
                    }
                }
            }

            // 7. Area chart ───────────────────────────────────────────────────
            if (_showChart && _chartData != null && _chartData.Length > 1)
                DrawAreaChart(g, pad);

            // 8. Icon (bottom-left) ───────────────────────────────────────────
            if (_showIcon)
            {
                float ix = (_showLeftAccentBorder ? _leftAccentBorderWidth : 0) + pad;
                float iy = pad + 2f;

                if (_iconImage != null)
                    g.DrawImage(_iconImage, new RectangleF(ix, iy, 24f, 24f));
                else
                    DrawCalendarIcon(g, ix, iy, 22f);
            }

            g.ResetClip();
        }

        // ── Area Chart ────────────────────────────────────────────────────────
        private void DrawAreaChart(Graphics g, float pad)
        {
            int count = _chartData.Length;
            float left = (_showLeftAccentBorder ? _leftAccentBorderWidth : 0) + pad;
            float right = Width - pad;
            float top = Height * 0.60f;
            float bottom = Height - pad * 0.55f;
            float chartH = bottom - top;
            float chartW = right - left;

            // Calculate max & average
            int maxVal = 1, sumVal = 0;
            foreach (int v in _chartData) { if (v > maxVal) maxVal = v; sumVal += v; }
            float avg = (float)sumVal / count;

            // Build point array
            var pts = new PointF[count];
            for (int i = 0; i < count; i++)
            {
                float x = left + i * (chartW / (count - 1));
                float y = bottom - chartH * ((float)_chartData[i] / maxVal);
                pts[i] = new PointF(x, y);
            }

            // Average line (dashed)
            if (_showAverageLine)
            {
                float avgY = bottom - chartH * (avg / maxVal);
                using (Pen pen = new Pen(Color.FromArgb(90, MutedColor), 1f))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawLine(pen, left, avgY, right, avgY);
                }
            }

            // Filled area under the line
            var fillPts = new PointF[count + 2];
            fillPts[0] = new PointF(left, bottom);
            for (int i = 0; i < count; i++) fillPts[i + 1] = pts[i];
            fillPts[count + 1] = new PointF(right, bottom);

            int fillAlpha = (int)(255 * _chartFillOpacity / 100.0);
            using (SolidBrush fillBrush = new SolidBrush(
                Color.FromArgb(fillAlpha, _chartFillColor.R, _chartFillColor.G, _chartFillColor.B)))
            {
                g.FillPolygon(fillBrush, fillPts);
            }

            // Line stroke
            using (Pen linePen = new Pen(_accentColor, 1.8f))
            {
                linePen.LineJoin = LineJoin.Round;
                g.DrawLines(linePen, pts);
            }

            // Highlight dot
            int hlIdx = (_highlightDay < 0 || _highlightDay >= count) ? count - 1 : _highlightDay;
            float hx = pts[hlIdx].X;
            float hy = pts[hlIdx].Y;

            // Outer glow ring
            using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(55, _accentColor)))
                g.FillEllipse(glowBrush, hx - 7f, hy - 7f, 14f, 14f);

            // Inner filled dot
            using (SolidBrush dotBrush = new SolidBrush(_accentColor))
                g.FillEllipse(dotBrush, hx - 3.5f, hy - 3.5f, 7f, 7f);

            // White center pinpoint
            using (SolidBrush centerBrush = new SolidBrush(Color.White))
                g.FillEllipse(centerBrush, hx - 1.5f, hy - 1.5f, 3f, 3f);
        }

        // ── Calendar icon ─────────────────────────────────────────────────────
        private void DrawCalendarIcon(Graphics g, float x, float y, float size)
        {
            using (Pen pen = new Pen(_iconColor, 1.5f))
            {
                // Card body
                g.DrawRectangle(pen, x + 1f, y + 3f, size - 2f, size - 4f);

                // Header separator line
                g.DrawLine(pen, x + 1f, y + 9f, x + size - 1f, y + 9f);

                // Binding rings
                g.DrawLine(pen, x + 6f, y + 1f, x + 6f, y + 6f);
                g.DrawLine(pen, x + size - 6f, y + 1f, x + size - 6f, y + 6f);

                // Day dots (2 rows × 3 columns)
                float dotR = 1.5f;
                for (int row = 0; row < 2; row++)
                    for (int col = 0; col < 3; col++)
                    {
                        float dx = x + 5f + col * 5.5f;
                        float dy = y + 12f + row * 5f;
                        using (SolidBrush dotBrush = new SolidBrush(_iconColor))
                            g.FillEllipse(dotBrush, dx, dy, dotR * 2, dotR * 2);
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

        // ════════════════════════════════════════════════════════════════════
        //  OVERRIDES & DISPOSE
        // ════════════════════════════════════════════════════════════════════
        protected override Size DefaultSize => new Size(320, 180);

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
                _countFont?.Dispose();
                _trendFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}