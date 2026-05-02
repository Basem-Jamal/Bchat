using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat.Report
{
    public enum CardTheme2 { Dark, Light }

    [ToolboxItem(true)]
    [DefaultProperty("TotalMessages")]
    [Description("Card that displays monthly message statistics with an area sparkline chart.")]
    public class MonthlyMessagesCard2 : Control
    {
        // ─────────────────────────────────────────────
        //  Fields
        // ─────────────────────────────────────────────
        private CardTheme2 _theme = CardTheme2.Dark;
        private Color _accentColor = Color.FromArgb(124, 111, 247);
        private Color _chartFillColor = Color.FromArgb(124, 111, 247);
        private int _borderRadius = 18;
        private int _dotGridOpacity = 18;
        private Color _cardBackColor = Color.FromArgb(26, 31, 60);

        private string _title = "الرسائل الشهرية";
        private string _subLabel = "إجمالي رسائل الشهر الحالي";
        private string _monthLabel = "";
        private int _totalMessages = 4_872;

        private int[] _chartData = { 120, 95, 210, 180, 330, 260, 410, 375, 490, 420, 560, 510,
                                          480, 600, 570, 650, 590, 710, 680, 740, 700, 780, 820, 760,
                                          830, 870, 910, 880, 950, 1020 };
        private int _fillOpacity = 28;
        private Color _iconColor = Color.FromArgb(124, 111, 247);
        private Image _iconImage = null;
        private bool _showIcon = true;

        private double _trendValue = 12.4;
        private bool _showTrend = true;
        private Color _trendPositiveColor = Color.FromArgb(16, 185, 129);
        private Color _trendNegativeColor = Color.FromArgb(239, 68, 68);
        private string _compareLabel = "مقارنةً بالشهر الماضي";

        private Font _titleFont;
        private Font _countFont;
        private Font _subFont;
        private Font _trendFont;
        private Font _monthFont;
        private bool _fontsOwned = false;

        // ─────────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────────
        public MonthlyMessagesCard2()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            RightToLeft = RightToLeft.Yes;
            BackColor = Color.Transparent;
            Size = new Size(420, 200);

            _monthLabel = DateTime.Now.ToString("MMMM yyyy");

            _titleFont = new Font("Cairo", 11f, FontStyle.Bold, GraphicsUnit.Point);
            _countFont = new Font("Cairo", 28f, FontStyle.Bold, GraphicsUnit.Point);
            _subFont = new Font("Cairo", 9f, FontStyle.Regular, GraphicsUnit.Point);
            _trendFont = new Font("Cairo", 9f, FontStyle.Regular, GraphicsUnit.Point);
            _monthFont = new Font("Cairo", 8f, FontStyle.Regular, GraphicsUnit.Point);
            _fontsOwned = true;

            ApplyTheme();
        }

        // ─────────────────────────────────────────────
        //  Properties — Appearance
        // ─────────────────────────────────────────────
        [Category("MessagesCard - Appearance")]
        [Description("Dark or Light theme.")]
        [DefaultValue(CardTheme2.Dark)]
        public CardTheme2 Theme
        {
            get => _theme;
            set { _theme = value; ApplyTheme(); }
        }

        [Category("MessagesCard - Appearance")]
        [Description("Main accent color used for chart line, icon, and highlights.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; _chartFillColor = value; Invalidate(); }
        }

        [Category("MessagesCard - Appearance")]
        [Description("Corner radius of the card.")]
        [DefaultValue(18)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Clamp(value, 0, 40); Invalidate(); }
        }

        [Category("MessagesCard - Appearance")]
        [Description("Opacity of the background dot grid texture (0 = off, 100 = full).")]
        [DefaultValue(18)]
        public int DotGridOpacity
        {
            get => _dotGridOpacity;
            set { _dotGridOpacity = Clamp(value, 0, 100); Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Content
        // ─────────────────────────────────────────────
        [Category("MessagesCard - Content")]
        [Description("Card title text.")]
        [DefaultValue("الرسائل الشهرية")]
        public string Title
        {
            get => _title;
            set { _title = value ?? ""; Invalidate(); }
        }

        [Category("MessagesCard - Content")]
        [Description("Sub-label below the main count.")]
        [DefaultValue("إجمالي رسائل الشهر الحالي")]
        public string SubLabel
        {
            get => _subLabel;
            set { _subLabel = value ?? ""; Invalidate(); }
        }

        [Category("MessagesCard - Content")]
        [Description("Month label (auto-filled from current date if empty).")]
        public string MonthLabel
        {
            get => _monthLabel;
            set { _monthLabel = value ?? ""; Invalidate(); }
        }

        [Category("MessagesCard - Content")]
        [Description("Total message count displayed as the primary metric.")]
        [DefaultValue(4872)]
        public int TotalMessages
        {
            get => _totalMessages;
            set { _totalMessages = value < 0 ? 0 : value; Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Chart
        // ─────────────────────────────────────────────
        [Category("MessagesCard - Chart")]
        [Description("Daily message counts array (up to 31 values for monthly view).")]
        public int[] ChartData
        {
            get => _chartData;
            set { _chartData = value ?? new int[30]; Invalidate(); }
        }

        [Category("MessagesCard - Chart")]
        [Description("Opacity of the filled area under the chart line (0–100).")]
        [DefaultValue(28)]
        public int FillOpacity
        {
            get => _fillOpacity;
            set { _fillOpacity = Clamp(value, 0, 100); Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Trend
        // ─────────────────────────────────────────────
        [Category("MessagesCard - Trend")]
        [Description("Trend percentage value. Positive = up, negative = down.")]
        [DefaultValue(12.4)]
        public double TrendValue
        {
            get => _trendValue;
            set { _trendValue = value; Invalidate(); }
        }

        [Category("MessagesCard - Trend")]
        [Description("Show or hide the trend indicator.")]
        [DefaultValue(true)]
        public bool ShowTrend
        {
            get => _showTrend;
            set { _showTrend = value; Invalidate(); }
        }

        [Category("MessagesCard - Trend")]
        [Description("Color shown when trend is positive.")]
        public Color TrendPositiveColor
        {
            get => _trendPositiveColor;
            set { _trendPositiveColor = value; Invalidate(); }
        }

        [Category("MessagesCard - Trend")]
        [Description("Color shown when trend is negative.")]
        public Color TrendNegativeColor
        {
            get => _trendNegativeColor;
            set { _trendNegativeColor = value; Invalidate(); }
        }

        [Category("MessagesCard - Trend")]
        [Description("Comparison label shown next to the trend value.")]
        [DefaultValue("مقارنةً بالشهر الماضي")]
        public string CompareLabel
        {
            get => _compareLabel;
            set { _compareLabel = value ?? ""; Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Icon
        // ─────────────────────────────────────────────
        [Category("MessagesCard - Icon")]
        [Description("Show the message icon.")]
        [DefaultValue(true)]
        public bool ShowIcon
        {
            get => _showIcon;
            set { _showIcon = value; Invalidate(); }
        }

        [Category("MessagesCard - Icon")]
        [Description("Custom icon image. When null the built-in envelope icon is drawn.")]
        [DefaultValue(null)]
        public Image IconImage
        {
            get => _iconImage;
            set { _iconImage = value; Invalidate(); }
        }

        [Category("MessagesCard - Icon")]
        [Description("Color of the built-in icon.")]
        public Color IconColor
        {
            get => _iconColor;
            set { _iconColor = value; Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Fonts
        // ─────────────────────────────────────────────
        [Category("MessagesCard - Fonts")]
        [Description("Font used for the card title.")]
        public Font TitleFont
        {
            get => _titleFont;
            set
            {
                if (_fontsOwned) _titleFont?.Dispose();
                _titleFont = value ?? new Font("Cairo", 11f, FontStyle.Bold, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        [Category("MessagesCard - Fonts")]
        [Description("Font used for the main message count.")]
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

        [Category("MessagesCard - Fonts")]
        [Description("Font used for sub-label text.")]
        public Font SubFont
        {
            get => _subFont;
            set
            {
                if (_fontsOwned) _subFont?.Dispose();
                _subFont = value ?? new Font("Cairo", 9f, FontStyle.Regular, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        [Category("MessagesCard - Fonts")]
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

        // ─────────────────────────────────────────────
        //  Theme
        // ─────────────────────────────────────────────
        private void ApplyTheme()
        {
            _cardBackColor = _theme == CardTheme2.Dark
                ? Color.FromArgb(26, 31, 60)
                : Color.FromArgb(248, 250, 252);
            Invalidate();
        }

        private Color TextColor => _theme == CardTheme2.Dark
            ? Color.White
            : Color.FromArgb(15, 23, 42);

        private Color MutedColor => _theme == CardTheme2.Dark
            ? Color.FromArgb(148, 163, 184)
            : Color.FromArgb(100, 116, 139);

        // ─────────────────────────────────────────────
        //  OnPaint
        // ─────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = _borderRadius;
            float pad = 18f;

            // ── Step 1: Card background ──
            using (GraphicsPath bgPath = RoundedRect(bounds, r))
            using (SolidBrush bgBrush = new SolidBrush(_cardBackColor))
                g.FillPath(bgBrush, bgPath);

            // ── Step 2: Gradient overlay ──
            using (GraphicsPath gradPath = RoundedRect(bounds, r))
            using (LinearGradientBrush grad = new LinearGradientBrush(
                bounds,
                Color.FromArgb(_theme == CardTheme2.Dark ? 22 : 10, Color.White),
                Color.FromArgb(0, Color.Black),
                LinearGradientMode.Vertical))
                g.FillPath(grad, gradPath);

            // ── Step 3: SetClip ──
            using (GraphicsPath clip = RoundedRect(bounds, r))
                g.SetClip(clip);

            // ── Step 4: Dot grid texture ──
            PaintDotGrid(g);

            // ── Step 5: Left accent border ──
            PaintAccentBorder(g, bounds, r);

            // ── Step 6: Title + month label (top-right) ──
            float contentRight = Width - pad - 6f;
            float contentLeft = pad + 8f;

            using (SolidBrush tb = new SolidBrush(TextColor))
            {
                SizeF titleSz = g.MeasureString(_title, _titleFont);
                g.DrawString(_title, _titleFont, tb, contentRight - titleSz.Width, pad);
            }

            if (!string.IsNullOrEmpty(_monthLabel))
            {
                using (SolidBrush mb = new SolidBrush(MutedColor))
                {
                    SizeF msz = g.MeasureString(_monthLabel, _monthFont);
                    g.DrawString(_monthLabel, _monthFont, mb, contentLeft, pad + 3f);
                }
            }

            // ── Step 7: Icon ──
            float iconY = pad + 2f;
            if (_showIcon)
            {
                SizeF titleSz2 = g.MeasureString(_title, _titleFont);
                float iconX = contentRight - titleSz2.Width - 30f;
                if (_iconImage != null)
                    g.DrawImage(_iconImage, new RectangleF(iconX, iconY, 22f, 22f));
                else
                    PaintEnvelopeIcon(g, iconX, iconY, 22f);
            }

            // ── Step 8 + 9: Count + sub-label ──
            float metricTop = pad + 34f;
            string countTxt = _totalMessages.ToString("N0");
            using (SolidBrush cb = new SolidBrush(TextColor))
            {
                SizeF csz = g.MeasureString(countTxt, _countFont);
                g.DrawString(countTxt, _countFont, cb, contentRight - csz.Width, metricTop);
            }

            float subY = metricTop + g.MeasureString(countTxt, _countFont).Height - 4f;
            using (SolidBrush sb2 = new SolidBrush(MutedColor))
            {
                SizeF ssz = g.MeasureString(_subLabel, _subFont);
                g.DrawString(_subLabel, _subFont, sb2, contentRight - ssz.Width, subY);
            }

            // ── Step 8: Area sparkline chart ──
            PaintAreaChart(g, pad, contentLeft);

            // ── Step 10: Trend ──
            if (_showTrend)
                PaintTrend(g, pad);

            // ── Step 11: ResetClip ──
            g.ResetClip();
        }

        // ─────────────────────────────────────────────
        //  Paint Helpers
        // ─────────────────────────────────────────────
        private void PaintAccentBorder(Graphics g, Rectangle bounds, int r)
        {
            int bw = 4;
            using (SolidBrush ab = new SolidBrush(_accentColor))
            {
                // RTL: accent on right side
                g.FillRectangle(ab, Width - bw, r, bw, Height - r * 2);
                g.FillRectangle(ab, Width - bw, 0, bw, r);
                g.FillRectangle(ab, Width - bw, Height - r, bw, r);
            }
        }

        private void PaintAreaChart(Graphics g, float pad, float contentLeft)
        {
            int[] data = _chartData ?? new int[30];
            int count = Math.Min(data.Length, 31);
            if (count < 2) return;

            // Find max
            int maxVal = 1;
            for (int i = 0; i < count; i++)
                if (data[i] > maxVal) maxVal = data[i];

            float chartLeft = contentLeft;
            float chartRight = Width * 0.52f;
            float chartBottom = Height - (pad + 26f);
            float chartTop = pad + 10f;
            float chartW = chartRight - chartLeft;
            float chartH = chartBottom - chartTop;

            // Build points
            var pts = new PointF[count];
            for (int i = 0; i < count; i++)
            {
                pts[i] = new PointF(
                    chartLeft + i * (chartW / (count - 1)),
                    chartBottom - chartH * ((float)data[i] / maxVal));
            }

            // Average reference line
            float sum = 0f;
            for (int i = 0; i < count; i++) sum += data[i];
            float avg = sum / count;
            float avgY = chartBottom - chartH * (avg / maxVal);
            using (Pen dp = new Pen(Color.FromArgb(60, MutedColor), 1f) { DashStyle = DashStyle.Dash })
                g.DrawLine(dp, chartLeft, avgY, chartRight, avgY);

            // Filled polygon area
            var fillPts = new PointF[count + 2];
            fillPts[0] = new PointF(chartLeft, chartBottom);
            for (int i = 0; i < count; i++) fillPts[i + 1] = pts[i];
            fillPts[count + 1] = new PointF(chartRight, chartBottom);

            int fillAlpha = (int)(255 * _fillOpacity / 100.0);
            using (SolidBrush fb = new SolidBrush(Color.FromArgb(fillAlpha, _chartFillColor)))
                g.FillPolygon(fb, fillPts);

            // Gradient on the fill
            using (LinearGradientBrush grad2 = new LinearGradientBrush(
                new PointF(chartLeft, chartTop),
                new PointF(chartLeft, chartBottom),
                Color.FromArgb(fillAlpha + 20, _accentColor),
                Color.FromArgb(0, _accentColor)))
            {
                var fillPts2 = (PointF[])fillPts.Clone();
                g.FillPolygon(grad2, fillPts2);
            }

            // Chart line
            using (Pen lp = new Pen(_accentColor, 2f) { LineJoin = LineJoin.Round })
                g.DrawLines(lp, pts);

            // Glow endpoint dot (last point = today)
            float hx = pts[count - 1].X;
            float hy = pts[count - 1].Y;
            using (SolidBrush gb = new SolidBrush(Color.FromArgb(45, _accentColor)))
                g.FillEllipse(gb, hx - 8f, hy - 8f, 16f, 16f);
            using (SolidBrush gb2 = new SolidBrush(Color.FromArgb(25, _accentColor)))
                g.FillEllipse(gb2, hx - 12f, hy - 12f, 24f, 24f);
            using (SolidBrush db = new SolidBrush(_accentColor))
                g.FillEllipse(db, hx - 4f, hy - 4f, 8f, 8f);
            using (SolidBrush wb = new SolidBrush(Color.White))
                g.FillEllipse(wb, hx - 1.8f, hy - 1.8f, 3.6f, 3.6f);

            // Day labels: first, mid, last
            string[] labels = { "1", "15", "30" };
            float[] lx = { chartLeft, chartLeft + chartW / 2f, chartRight };
            using (SolidBrush lb = new SolidBrush(MutedColor))
            {
                foreach (int idx in new[] { 0, 1, 2 })
                {
                    SizeF lsz = g.MeasureString(labels[idx], _monthFont);
                    g.DrawString(labels[idx], _monthFont, lb,
                        lx[idx] - lsz.Width / 2f, chartBottom + 4f);
                }
            }
        }

        private void PaintTrend(Graphics g, float pad)
        {
            bool pos = _trendValue >= 0;
            string arrow = pos ? "▲" : "▼";
            string sign = pos ? "+" : "-";
            string txt = $"{arrow} {sign}{Math.Abs(_trendValue):F1}%  {_compareLabel}";
            Color col = pos ? _trendPositiveColor : _trendNegativeColor;

            using (SolidBrush tb = new SolidBrush(col))
            {
                SizeF sz = g.MeasureString(txt, _trendFont);
                g.DrawString(txt, _trendFont, tb,
                    (Width - sz.Width) / 2f,
                    Height - pad - sz.Height - 2f);
            }
        }

        private void PaintDotGrid(Graphics g)
        {
            const int spacing = 22;
            const float dotR = 1.1f;
            int alpha = (int)(255.0 * _dotGridOpacity / 100.0);
            if (alpha <= 0) return;
            using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, MutedColor)))
            {
                for (int x = spacing; x < Width; x += spacing)
                    for (int y = spacing; y < Height; y += spacing)
                        g.FillEllipse(b, x - dotR, y - dotR, dotR * 2f, dotR * 2f);
            }
        }

        private void PaintEnvelopeIcon(Graphics g, float x, float y, float size)
        {
            // Draw a simple envelope icon
            using (Pen pen = new Pen(_iconColor, 1.5f) { LineJoin = LineJoin.Round })
            {
                // Envelope body
                g.DrawRectangle(pen, x, y + size * 0.2f, size, size * 0.7f);
                // Flap lines
                g.DrawLine(pen,
                    x, y + size * 0.2f,
                    x + size / 2f, y + size * 0.58f);
                g.DrawLine(pen,
                    x + size, y + size * 0.2f,
                    x + size / 2f, y + size * 0.58f);
            }
            // Accent dot badge on top-right corner
            using (SolidBrush dot = new SolidBrush(_accentColor))
                g.FillEllipse(dot, x + size * 0.72f, y, size * 0.28f, size * 0.28f);
        }

        // ─────────────────────────────────────────────
        //  GDI+ Helpers
        // ─────────────────────────────────────────────
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

        private static Color LightenColor(Color color, float amount)
        {
            float r = Math.Min(255f, color.R + (255f - color.R) * amount);
            float gg = Math.Min(255f, color.G + (255f - color.G) * amount);
            float b = Math.Min(255f, color.B + (255f - color.B) * amount);
            return Color.FromArgb(color.A, (int)r, (int)gg, (int)b);
        }

        private static int Clamp(int v, int lo, int hi) => v < lo ? lo : v > hi ? hi : v;

        // ─────────────────────────────────────────────
        //  Overrides
        // ─────────────────────────────────────────────
        protected override Size DefaultSize => new Size(420, 200);

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
                _subFont?.Dispose();
                _trendFont?.Dispose();
                _monthFont?.Dispose();
            }
            // Do NOT dispose _iconImage — the Designer owns it
            base.Dispose(disposing);
        }
    }
}