using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat.Report
{
    // NOTE: CardTheme enum is declared in MonthlyMessagesCard.cs / CampaignSuccessCard.cs

    [ToolboxItem(true)]
    [DefaultProperty("TotalMessages")]
    [Description("Monthly messages card with area sparkline chart. Supports Dark and Light themes.")]
    public class MessagesChartCard : Control
    {
        // ─────────────────────────────────────────────
        //  Fields
        // ─────────────────────────────────────────────
        private CardTheme _theme          = CardTheme.Light;
        private Color     _cardBackColor  = Color.FromArgb(255, 255, 255);
        private Color     _accentColor    = Color.FromArgb(82, 215, 175);
        private Color     _chartFillColor = Color.FromArgb(82, 215, 175);
        private int       _borderRadius   = 20;
        private int       _dotGridOpacity = 8;
        private bool      _showBorder     = true;
        private Color     _borderColor    = Color.FromArgb(226, 232, 240);

        private string _title         = "رسائل الشهر";
        private int    _totalMessages = 38_420;

        private int[]  _chartData   = { 80,110,95,130,160,140,180,170,200,185,
                                        210,230,215,250,240,270,255,280,265,300,
                                        285,310,295,330,315,350,340,370,355,390 };
        private int    _fillOpacity = 30;
        private Color  _iconColor   = Color.FromArgb(82, 215, 175);
        private Image  _iconImage   = null;
        private bool   _showIcon    = true;

        private double _trendValue         = 15.0;
        private bool   _showTrend          = true;
        private Color  _trendPositiveColor = Color.FromArgb(16, 185, 129);
        private Color  _trendNegativeColor = Color.FromArgb(239, 68, 68);
        private string _compareLabel       = "من الشهر الماضي";

        private Font _titleFont;
        private Font _countFont;
        private Font _trendFont;
        private bool _fontsOwned = false;

        // ─────────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────────
        public MessagesChartCard()
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
            Size        = new Size(320, 180);

            _titleFont = new Font("Cairo", 10f, FontStyle.Regular, GraphicsUnit.Point);
            _countFont = new Font("Cairo", 26f, FontStyle.Bold,    GraphicsUnit.Point);
            _trendFont = new Font("Cairo",  9f, FontStyle.Regular, GraphicsUnit.Point);
            _fontsOwned = true;

            ApplyTheme();
        }

        // ─────────────────────────────────────────────
        //  Properties — Appearance
        // ─────────────────────────────────────────────
        [Category("MessagesChartCard - Appearance")]
        [Description("Dark or Light card theme.")]
        [DefaultValue(CardTheme.Light)]
        public CardTheme Theme
        {
            get => _theme;
            set { _theme = value; ApplyTheme(); }
        }

        [Category("MessagesChartCard - Appearance")]
        [Description("Chart line and accent color.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; _chartFillColor = value; Invalidate(); }
        }

        [Category("MessagesChartCard - Appearance")]
        [Description("Card corner radius.")]
        [DefaultValue(20)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Clamp(value, 0, 50); Invalidate(); }
        }

        [Category("MessagesChartCard - Appearance")]
        [Description("Show a subtle card border.")]
        [DefaultValue(true)]
        public bool ShowBorder
        {
            get => _showBorder;
            set { _showBorder = value; Invalidate(); }
        }

        [Category("MessagesChartCard - Appearance")]
        [Description("Card border color.")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("MessagesChartCard - Appearance")]
        [Description("Dot grid texture opacity (0=off, 100=full).")]
        [DefaultValue(8)]
        public int DotGridOpacity
        {
            get => _dotGridOpacity;
            set { _dotGridOpacity = Clamp(value, 0, 100); Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Content
        // ─────────────────────────────────────────────
        [Category("MessagesChartCard - Content")]
        [Description("Card title text.")]
        [DefaultValue("رسائل الشهر")]
        public string Title
        {
            get => _title;
            set { _title = value ?? ""; Invalidate(); }
        }

        [Category("MessagesChartCard - Content")]
        [Description("Total messages count.")]
        [DefaultValue(38420)]
        public int TotalMessages
        {
            get => _totalMessages;
            set { _totalMessages = value < 0 ? 0 : value; Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Chart
        // ─────────────────────────────────────────────
        [Category("MessagesChartCard - Chart")]
        [Description("Daily message counts array (up to 31 values).")]
        public int[] ChartData
        {
            get => _chartData;
            set { _chartData = value ?? new int[30]; Invalidate(); }
        }

        [Category("MessagesChartCard - Chart")]
        [Description("Fill area opacity under the chart line (0–100).")]
        [DefaultValue(30)]
        public int FillOpacity
        {
            get => _fillOpacity;
            set { _fillOpacity = Clamp(value, 0, 100); Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Trend
        // ─────────────────────────────────────────────
        [Category("MessagesChartCard - Trend")]
        [Description("Trend percentage value.")]
        [DefaultValue(15.0)]
        public double TrendValue
        {
            get => _trendValue;
            set { _trendValue = value; Invalidate(); }
        }

        [Category("MessagesChartCard - Trend")]
        [Description("Show or hide the trend indicator.")]
        [DefaultValue(true)]
        public bool ShowTrend
        {
            get => _showTrend;
            set { _showTrend = value; Invalidate(); }
        }

        [Category("MessagesChartCard - Trend")]
        [Description("Positive trend color.")]
        public Color TrendPositiveColor
        {
            get => _trendPositiveColor;
            set { _trendPositiveColor = value; Invalidate(); }
        }

        [Category("MessagesChartCard - Trend")]
        [Description("Negative trend color.")]
        public Color TrendNegativeColor
        {
            get => _trendNegativeColor;
            set { _trendNegativeColor = value; Invalidate(); }
        }

        [Category("MessagesChartCard - Trend")]
        [Description("Comparison label shown next to trend value.")]
        [DefaultValue("من الشهر الماضي")]
        public string CompareLabel
        {
            get => _compareLabel;
            set { _compareLabel = value ?? ""; Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Icon
        // ─────────────────────────────────────────────
        [Category("MessagesChartCard - Icon")]
        [Description("Show the icon.")]
        [DefaultValue(true)]
        public bool ShowIcon
        {
            get => _showIcon;
            set { _showIcon = value; Invalidate(); }
        }

        [Category("MessagesChartCard - Icon")]
        [Description("Custom icon image. When null, built-in icon is drawn.")]
        [DefaultValue(null)]
        public Image IconImage
        {
            get => _iconImage;
            set { _iconImage = value; Invalidate(); }
        }

        [Category("MessagesChartCard - Icon")]
        [Description("Built-in icon color.")]
        public Color IconColor
        {
            get => _iconColor;
            set { _iconColor = value; Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Fonts
        // ─────────────────────────────────────────────
        [Category("MessagesChartCard - Fonts")]
        [Description("Font for the title.")]
        public Font TitleFont
        {
            get => _titleFont;
            set
            {
                if (_fontsOwned) _titleFont?.Dispose();
                _titleFont  = value ?? new Font("Cairo", 10f, FontStyle.Regular, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        [Category("MessagesChartCard - Fonts")]
        [Description("Font for the main count number.")]
        public Font CountFont
        {
            get => _countFont;
            set
            {
                if (_fontsOwned) _countFont?.Dispose();
                _countFont  = value ?? new Font("Cairo", 26f, FontStyle.Bold, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        [Category("MessagesChartCard - Fonts")]
        [Description("Font for the trend text.")]
        public Font TrendFont
        {
            get => _trendFont;
            set
            {
                if (_fontsOwned) _trendFont?.Dispose();
                _trendFont  = value ?? new Font("Cairo", 9f, FontStyle.Regular, GraphicsUnit.Point);
                _fontsOwned = false;
                Invalidate();
            }
        }

        // ─────────────────────────────────────────────
        //  Theme
        // ─────────────────────────────────────────────
        private void ApplyTheme()
        {
            if (_theme == CardTheme.Dark)
            {
                _cardBackColor = Color.FromArgb(26, 31, 60);
                _borderColor   = Color.FromArgb(40, 50, 90);
            }
            else
            {
                _cardBackColor = Color.FromArgb(255, 255, 255);
                _borderColor   = Color.FromArgb(226, 232, 240);
            }
            Invalidate();
        }

        private Color TextColor  => _theme == CardTheme.Dark ? Color.White : Color.FromArgb(15, 23, 42);
        private Color MutedColor => _theme == CardTheme.Dark
            ? Color.FromArgb(148, 163, 184)
            : Color.FromArgb(100, 116, 139);

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

            // Step 2: Border
            if (_showBorder)
            {
                using (GraphicsPath borderPath = RoundedRect(bounds, r))
                using (Pen borderPen = new Pen(_borderColor, 1.2f))
                    g.DrawPath(borderPen, borderPath);
            }

            // Step 3: SetClip
            using (GraphicsPath clip = RoundedRect(bounds, r))
                g.SetClip(clip);

            // Step 4: Dot grid
            PaintDotGrid(g);

            // Step 5: Icon (top-left in RTL = top-right visually)
            float iconSize = 26f;
            float iconX    = Width - pad - iconSize;
            float iconY    = pad;
            if (_showIcon)
            {
                // Icon background bubble
                using (SolidBrush ibg = new SolidBrush(Color.FromArgb(30, _accentColor)))
                    g.FillEllipse(ibg, iconX - 4f, iconY - 4f, iconSize + 8f, iconSize + 8f);
                if (_iconImage != null)
                    g.DrawImage(_iconImage, new RectangleF(iconX, iconY, iconSize, iconSize));
                else
                    PaintMessageIcon(g, iconX, iconY, iconSize);
            }

            // Step 6: Title
            float titleX = Width - pad - (_showIcon ? iconSize + 14f : 0f);
            using (SolidBrush tb = new SolidBrush(MutedColor))
            {
                SizeF tsz = g.MeasureString(_title, _titleFont);
                g.DrawString(_title, _titleFont, tb, titleX - tsz.Width, pad + 4f);
            }

            // Step 7: Count
            string countTxt = _totalMessages.ToString("N0");
            SizeF  cntSz    = g.MeasureString(countTxt, _countFont);
            float  countY   = pad + g.MeasureString(_title, _titleFont).Height + 2f;
            using (SolidBrush cb = new SolidBrush(TextColor))
                g.DrawString(countTxt, _countFont, cb, Width - pad - cntSz.Width, countY);

            // Step 8: Area chart (bottom half)
            float chartTop    = countY + cntSz.Height - 5f;
            float trendH      = _showTrend ? 24f : 0f;
            float chartBottom = Height - pad * 0.5f - trendH;
            PaintAreaChart(g, pad, chartTop, chartBottom);

            // Step 9: Trend
            if (_showTrend)
            {
                bool   pos   = _trendValue >= 0;
                string arrow = pos ? "▲" : "▼";
                string txt   = $"{arrow} {(pos ? "+" : "-")}{Math.Abs(_trendValue):F0}%  {_compareLabel}";
                Color  col   = pos ? _trendPositiveColor : _trendNegativeColor;
                using (SolidBrush trendBrush = new SolidBrush(col))
                {
                    SizeF tsz2 = g.MeasureString(txt, _trendFont);
                    g.DrawString(txt, _trendFont, trendBrush,
                        Width - pad - tsz2.Width,
                        Height - pad * 0.3f - tsz2.Height);
                }
            }

            g.ResetClip();
        }

        private void PaintAreaChart(Graphics g, float pad, float top, float bottom)
        {
            int[] data  = _chartData ?? new int[30];
            int   count = Math.Min(data.Length, 31);
            if (count < 2) return;

            int maxVal = 1;
            for (int i = 0; i < count; i++)
                if (data[i] > maxVal) maxVal = data[i];

            float left  = pad;
            float right = Width - pad;
            float chartW = right - left;
            float chartH = bottom - top;

            var pts = new PointF[count];
            for (int i = 0; i < count; i++)
            {
                pts[i] = new PointF(
                    left + i * (chartW / (count - 1)),
                    bottom - chartH * ((float)data[i] / maxVal));
            }

            // Fill
            var fillPts = new PointF[count + 2];
            fillPts[0] = new PointF(left, bottom);
            for (int i = 0; i < count; i++) fillPts[i + 1] = pts[i];
            fillPts[count + 1] = new PointF(right, bottom);

            using (LinearGradientBrush fillGrad = new LinearGradientBrush(
                new PointF(left, top), new PointF(left, bottom),
                Color.FromArgb((int)(255 * _fillOpacity / 100.0), _chartFillColor),
                Color.FromArgb(0, _chartFillColor)))
                g.FillPolygon(fillGrad, fillPts);

            // Line
            using (Pen lp = new Pen(_accentColor, 2f) { LineJoin = LineJoin.Round })
                g.DrawLines(lp, pts);

            // Glow endpoint
            float hx = pts[count - 1].X;
            float hy = pts[count - 1].Y;
            using (SolidBrush gb = new SolidBrush(Color.FromArgb(40, _accentColor)))
                g.FillEllipse(gb, hx - 8f, hy - 8f, 16f, 16f);
            using (SolidBrush db = new SolidBrush(_accentColor))
                g.FillEllipse(db, hx - 4f, hy - 4f, 8f, 8f);
            using (SolidBrush wb = new SolidBrush(Color.White))
                g.FillEllipse(wb, hx - 1.8f, hy - 1.8f, 3.6f, 3.6f);
        }

        private void PaintMessageIcon(Graphics g, float x, float y, float size)
        {
            using (Pen p = new Pen(_iconColor, 1.5f) { LineJoin = LineJoin.Round })
            {
                g.DrawRectangle(p, x + 1f, y + size * 0.15f, size - 2f, size * 0.72f);
                g.DrawLine(p, x + 1f,      y + size * 0.15f, x + size / 2f, y + size * 0.52f);
                g.DrawLine(p, x + size - 1f, y + size * 0.15f, x + size / 2f, y + size * 0.52f);
            }
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

        private static int Clamp(int v, int lo, int hi) => v < lo ? lo : v > hi ? hi : v;

        protected override Size DefaultSize => new Size(320, 180);
        protected override void OnResize(EventArgs e) { base.OnResize(e); Invalidate(); }

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
