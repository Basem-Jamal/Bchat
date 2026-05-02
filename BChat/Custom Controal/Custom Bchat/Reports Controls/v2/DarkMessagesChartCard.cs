using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat.Report
{
    // NOTE: CardTheme enum is declared in CampaignSuccessCard.cs

    [ToolboxItem(true)]
    [DefaultProperty("TotalMessages")]
    [Description("Dark-themed monthly messages card with area chart, wave gradient, and trend indicator.")]
    public class DarkMessagesChartCard : Control
    {
        // ─────────────────────────────────────────────
        //  Fields
        // ─────────────────────────────────────────────
        private CardTheme _theme          = CardTheme.Dark;
        private Color     _cardBackColor  = Color.FromArgb(18, 22, 48);
        private Color     _cardBackColor2 = Color.FromArgb(28, 34, 68);
        private Color     _accentColor    = Color.FromArgb(82, 215, 175);
        private Color     _chartFillColor = Color.FromArgb(82, 215, 175);
        private int       _borderRadius   = 20;
        private int       _dotGridOpacity = 15;
        private bool      _useGradientBg  = true;

        private string _title         = "رسائل الشهر";
        private int    _totalMessages = 38_420;

        private int[]  _chartData   = { 80,110,95,130,160,140,180,170,200,185,
                                        210,230,215,250,240,270,255,280,265,300,
                                        285,310,295,330,315,350,340,370,355,390 };
        private int    _fillOpacity = 25;
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
        public DarkMessagesChartCard()
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
        }

        // ─────────────────────────────────────────────
        //  Properties — Appearance
        // ─────────────────────────────────────────────
        [Category("DarkMessagesCard - Appearance")]
        [Description("Primary background color.")]
        public Color CardBackColor
        {
            get => _cardBackColor;
            set { _cardBackColor = value; Invalidate(); }
        }

        [Category("DarkMessagesCard - Appearance")]
        [Description("Secondary background gradient color (used when UseGradientBg = true).")]
        public Color CardBackColor2
        {
            get => _cardBackColor2;
            set { _cardBackColor2 = value; Invalidate(); }
        }

        [Category("DarkMessagesCard - Appearance")]
        [Description("Use diagonal gradient background.")]
        [DefaultValue(true)]
        public bool UseGradientBg
        {
            get => _useGradientBg;
            set { _useGradientBg = value; Invalidate(); }
        }

        [Category("DarkMessagesCard - Appearance")]
        [Description("Chart line and accent color.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; _chartFillColor = value; Invalidate(); }
        }

        [Category("DarkMessagesCard - Appearance")]
        [Description("Card corner radius.")]
        [DefaultValue(20)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Clamp(value, 0, 50); Invalidate(); }
        }

        [Category("DarkMessagesCard - Appearance")]
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
        [Category("DarkMessagesCard - Content")]
        [Description("Card title text.")]
        [DefaultValue("رسائل الشهر")]
        public string Title
        {
            get => _title;
            set { _title = value ?? ""; Invalidate(); }
        }

        [Category("DarkMessagesCard - Content")]
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
        [Category("DarkMessagesCard - Chart")]
        [Description("Daily message counts array (up to 31 values).")]
        public int[] ChartData
        {
            get => _chartData;
            set { _chartData = value ?? new int[30]; Invalidate(); }
        }

        [Category("DarkMessagesCard - Chart")]
        [Description("Fill area opacity under the chart line (0–100).")]
        [DefaultValue(25)]
        public int FillOpacity
        {
            get => _fillOpacity;
            set { _fillOpacity = Clamp(value, 0, 100); Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Trend
        // ─────────────────────────────────────────────
        [Category("DarkMessagesCard - Trend")]
        [Description("Trend percentage value.")]
        [DefaultValue(15.0)]
        public double TrendValue
        {
            get => _trendValue;
            set { _trendValue = value; Invalidate(); }
        }

        [Category("DarkMessagesCard - Trend")]
        [Description("Show or hide the trend indicator.")]
        [DefaultValue(true)]
        public bool ShowTrend
        {
            get => _showTrend;
            set { _showTrend = value; Invalidate(); }
        }

        [Category("DarkMessagesCard - Trend")]
        [Description("Positive trend color.")]
        public Color TrendPositiveColor
        {
            get => _trendPositiveColor;
            set { _trendPositiveColor = value; Invalidate(); }
        }

        [Category("DarkMessagesCard - Trend")]
        [Description("Negative trend color.")]
        public Color TrendNegativeColor
        {
            get => _trendNegativeColor;
            set { _trendNegativeColor = value; Invalidate(); }
        }

        [Category("DarkMessagesCard - Trend")]
        [Description("Comparison label text.")]
        [DefaultValue("من الشهر الماضي")]
        public string CompareLabel
        {
            get => _compareLabel;
            set { _compareLabel = value ?? ""; Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Icon
        // ─────────────────────────────────────────────
        [Category("DarkMessagesCard - Icon")]
        [Description("Show icon.")]
        [DefaultValue(true)]
        public bool ShowIcon
        {
            get => _showIcon;
            set { _showIcon = value; Invalidate(); }
        }

        [Category("DarkMessagesCard - Icon")]
        [Description("Custom icon image (null = built-in).")]
        [DefaultValue(null)]
        public Image IconImage
        {
            get => _iconImage;
            set { _iconImage = value; Invalidate(); }
        }

        [Category("DarkMessagesCard - Icon")]
        [Description("Built-in icon color.")]
        public Color IconColor
        {
            get => _iconColor;
            set { _iconColor = value; Invalidate(); }
        }

        // ─────────────────────────────────────────────
        //  Properties — Fonts
        // ─────────────────────────────────────────────
        [Category("DarkMessagesCard - Fonts")]
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

        [Category("DarkMessagesCard - Fonts")]
        [Description("Font for the count.")]
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

        [Category("DarkMessagesCard - Fonts")]
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
        //  Color shortcuts
        // ─────────────────────────────────────────────
        private Color MutedColor => Color.FromArgb(148, 163, 184);

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

            // Step 1: Background (gradient or solid)
            using (GraphicsPath bgPath = RoundedRect(bounds, r))
            {
                if (_useGradientBg)
                {
                    using (LinearGradientBrush grad = new LinearGradientBrush(
                        new Point(0, 0), new Point(Width, Height),
                        _cardBackColor, _cardBackColor2))
                        g.FillPath(grad, bgPath);
                }
                else
                {
                    using (SolidBrush bgBrush = new SolidBrush(_cardBackColor))
                        g.FillPath(bgBrush, bgPath);
                }
            }

            // Step 2: Subtle top-light overlay
            using (GraphicsPath gradPath = RoundedRect(bounds, r))
            using (LinearGradientBrush topLight = new LinearGradientBrush(
                bounds,
                Color.FromArgb(18, Color.White),
                Color.FromArgb(0, Color.Black),
                LinearGradientMode.Vertical))
                g.FillPath(topLight, gradPath);

            // Step 3: SetClip
            using (GraphicsPath clip = RoundedRect(bounds, r))
                g.SetClip(clip);

            // Step 4: Dot grid
            PaintDotGrid(g);

            // Step 5: Icon
            float iconSize = 26f;
            float iconX    = Width - pad - iconSize;
            float iconY    = pad;
            if (_showIcon)
            {
                using (SolidBrush ibg = new SolidBrush(Color.FromArgb(35, _accentColor)))
                    g.FillEllipse(ibg, iconX - 5f, iconY - 5f, iconSize + 10f, iconSize + 10f);
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
            using (SolidBrush cb = new SolidBrush(Color.White))
                g.DrawString(countTxt, _countFont, cb, Width - pad - cntSz.Width, countY);

            // Step 8: Area chart
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

            float left   = pad;
            float right  = Width - pad;
            float chartW = right - left;
            float chartH = bottom - top;

            var pts = new PointF[count];
            for (int i = 0; i < count; i++)
            {
                pts[i] = new PointF(
                    left + i * (chartW / (count - 1)),
                    bottom - chartH * ((float)data[i] / maxVal));
            }

            // Fill with gradient
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
            using (SolidBrush g1 = new SolidBrush(Color.FromArgb(20, _accentColor)))
                g.FillEllipse(g1, hx - 12f, hy - 12f, 24f, 24f);
            using (SolidBrush g2 = new SolidBrush(Color.FromArgb(40, _accentColor)))
                g.FillEllipse(g2, hx - 7f, hy - 7f, 14f, 14f);
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
                g.DrawLine(p, x + 1f,        y + size * 0.15f, x + size / 2f, y + size * 0.52f);
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
