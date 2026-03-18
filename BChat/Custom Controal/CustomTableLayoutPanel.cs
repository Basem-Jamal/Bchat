using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat
{
    [ToolboxItem(true)]
    [DefaultProperty("BackColorEx")]
    public class CustomTableLayoutPanel : TableLayoutPanel
    {
        // Panel
        private Color _backColorEx = Color.White;
        private int _borderRadius = 14;
        private Color _borderColor = Color.FromArgb(210, 218, 240);
        private int _borderThickness = 1;

        // Shadow
        private bool _showShadow = true;
        private int _shadowDepth = 8;
        private Color _shadowColor = Color.FromArgb(45, 80, 100, 180);

        // Header
        private bool _showHeaderStyle = true;
        private Color _headerStart = Color.FromArgb(90, 115, 255);
        private Color _headerEnd = Color.FromArgb(60, 80, 220);
        private int _headerHeight = 44;
        private bool _headerGradientHorizontal = true;

        // Rows
        private Color _rowEven = Color.White;
        private Color _rowOdd = Color.FromArgb(246, 248, 255);
        private Color _rowHover = Color.FromArgb(228, 235, 255);
        private Color _rowSelected = Color.FromArgb(210, 222, 255);
        private Color _rowSeparator = Color.FromArgb(230, 233, 245);
        private bool _showSeparators = true;
        private bool _alternateColor = true;

        // Accent
        private bool _showAccent = true;
        private Color _accentColor = Color.FromArgb(90, 115, 255);
        private int _accentWidth = 3;

        // ✅ State — يُعيَّن من الخارج عبر SetHoveredRow / SetSelectedRow
        private int _hoveredRow = -1;
        private int _selectedRow = -1;

        public event EventHandler<int> RowClicked;

        public CustomTableLayoutPanel()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            BackColor = Color.Transparent;
            AutoScroll = true;
        }

        // ── API عامة يستدعيها CreateCustomerRow ───────────────────────

        /// <summary>استدعِها من MouseEnter لكل rowPanel</summary>
        public void SetHoveredRow(int rowIndex)
        {
            if (_hoveredRow == rowIndex) return;
            _hoveredRow = rowIndex;
            Invalidate();
        }

        /// <summary>استدعِها من MouseLeave على آخر صف</summary>
        public void ClearHover()
        {
            if (_hoveredRow == -1) return;
            _hoveredRow = -1;
            Invalidate();
        }

        /// <summary>استدعِها من Click لكل rowPanel</summary>
        public void SetSelectedRow(int rowIndex)
        {
            _selectedRow = _selectedRow == rowIndex ? -1 : rowIndex;
            Invalidate();
            RowClicked?.Invoke(this, _selectedRow);
        }

        [Browsable(false)]
        public int SelectedRowIndex => _selectedRow;

        // ════════════════════════════════════════════════════════════════
        //  Properties
        // ════════════════════════════════════════════════════════════════

        [Category("🎨 Panel"), DisplayName("Background Color")]
        public Color BackColorEx
        { get => _backColorEx; set { _backColorEx = value; Invalidate(); } }

        [Category("🎨 Panel"), DisplayName("Border Radius")]
        public int BorderRadius
        { get => _borderRadius; set { _borderRadius = Math.Max(0, value); Invalidate(); } }

        [Category("🎨 Panel"), DisplayName("Border Color")]
        public Color BorderColor
        { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        [Category("🎨 Panel"), DisplayName("Border Thickness")]
        public int BorderThickness
        { get => _borderThickness; set { _borderThickness = Math.Max(0, value); Invalidate(); } }

        [Category("🌑 Shadow"), DisplayName("Show Shadow")]
        public bool ShowShadow
        { get => _showShadow; set { _showShadow = value; Invalidate(); } }

        [Category("🌑 Shadow"), DisplayName("Shadow Depth")]
        public int ShadowDepth
        { get => _shadowDepth; set { _shadowDepth = Math.Max(0, Math.Min(20, value)); Invalidate(); } }

        [Category("🌑 Shadow"), DisplayName("Shadow Color")]
        public Color ShadowColor
        { get => _shadowColor; set { _shadowColor = value; Invalidate(); } }

        [Category("📌 Header"), DisplayName("Show Header Style")]
        public bool ShowHeaderStyle
        { get => _showHeaderStyle; set { _showHeaderStyle = value; Invalidate(); } }

        [Category("📌 Header"), DisplayName("Header Gradient Start")]
        public Color HeaderGradientStart
        { get => _headerStart; set { _headerStart = value; Invalidate(); } }

        [Category("📌 Header"), DisplayName("Header Gradient End")]
        public Color HeaderGradientEnd
        { get => _headerEnd; set { _headerEnd = value; Invalidate(); } }

        [Category("📌 Header"), DisplayName("Header Height")]
        [Description("يجب أن يطابق RowStyles[0]")]
        public int HeaderHeight
        { get => _headerHeight; set { _headerHeight = Math.Max(20, value); Invalidate(); } }

        [Category("📌 Header"), DisplayName("Header Gradient Horizontal")]
        public bool HeaderGradientHorizontal
        { get => _headerGradientHorizontal; set { _headerGradientHorizontal = value; Invalidate(); } }

        [Category("📋 Rows"), DisplayName("Row Even Color")]
        public Color RowEvenColor
        { get => _rowEven; set { _rowEven = value; Invalidate(); } }

        [Category("📋 Rows"), DisplayName("Row Odd Color")]
        public Color RowOddColor
        { get => _rowOdd; set { _rowOdd = value; Invalidate(); } }

        [Category("📋 Rows"), DisplayName("Row Hover Color")]
        public Color RowHoverColor
        { get => _rowHover; set { _rowHover = value; Invalidate(); } }

        [Category("📋 Rows"), DisplayName("Row Selected Color")]
        public Color RowSelectedColor
        { get => _rowSelected; set { _rowSelected = value; Invalidate(); } }

        [Category("📋 Rows"), DisplayName("Row Separator Color")]
        public Color RowSeparatorColor
        { get => _rowSeparator; set { _rowSeparator = value; Invalidate(); } }

        [Category("📋 Rows"), DisplayName("Show Row Separators")]
        public bool ShowRowSeparators
        { get => _showSeparators; set { _showSeparators = value; Invalidate(); } }

        [Category("📋 Rows"), DisplayName("Alternate Row Colors")]
        public bool AlternateRowColors
        { get => _alternateColor; set { _alternateColor = value; Invalidate(); } }

        [Category("✨ Accent"), DisplayName("Show Accent Bar")]
        public bool ShowAccentBar
        { get => _showAccent; set { _showAccent = value; Invalidate(); } }

        [Category("✨ Accent"), DisplayName("Accent Color")]
        public Color AccentBarColor
        { get => _accentColor; set { _accentColor = value; Invalidate(); } }

        [Category("✨ Accent"), DisplayName("Accent Width")]
        public int AccentBarWidth
        { get => _accentWidth; set { _accentWidth = Math.Max(1, value); Invalidate(); } }

        // ════════════════════════════════════════════════════════════════
        //  OnPaintBackground
        // ════════════════════════════════════════════════════════════════

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? SystemColors.Control);

            int s = _showShadow ? _shadowDepth : 0;
            var panelRect = new Rectangle(0, 0, Width - 1 - s, Height - 1 - s);

            // ── Shadow ────────────────────────────────────────────────
            if (_showShadow && s > 0)
            {
                for (int i = s; i > 0; i--)
                {
                    double t = (double)i / s;
                    int alpha = (int)(_shadowColor.A * t * t);
                    int off = s - i;
                    var sr = new Rectangle(s + off / 2, s + off / 2,
                                       panelRect.Width - off, panelRect.Height - off);
                    if (sr.Width < 4 || sr.Height < 4) continue;
                    using (var path = Rounded(sr, _borderRadius))
                    using (var pen = new Pen(Color.FromArgb(Math.Min(255, alpha),
                                   _shadowColor.R, _shadowColor.G, _shadowColor.B), 1.8f))
                        g.DrawPath(pen, path);
                }
            }

            using (var clip = Rounded(panelRect, _borderRadius))
            {
                g.SetClip(clip);

                // خلفية
                using (var bg = new SolidBrush(_backColorEx))
                    g.FillPath(bg, clip);

                // ── ألوان الصفوف — تقرأ من RowStyles الحقيقية ─────────
                int headerRow = _showHeaderStyle ? 1 : 0;          // أول صف بيانات
                int totalRows = RowCount;
                int dataCount = Math.Max(0, totalRows - headerRow);

                if (dataCount > 0)
                {
                    // احسب Y لكل صف بيانات من الـ RowStyles الفعلية
                    int currentY = _showHeaderStyle ? _headerHeight : 0;

                    for (int r = 0; r < dataCount; r++)
                    {
                        int styleIdx = r + headerRow;
                        int rh = 50; // default

                        if (styleIdx < RowStyles.Count)
                        {
                            var rs = RowStyles[styleIdx];
                            if (rs.SizeType == SizeType.Absolute)
                                rh = (int)rs.Height;
                            else
                            {
                                // Percent أو AutoSize — اقسّم المساحة المتبقية
                                rh = (int)((Height - s - (_showHeaderStyle ? _headerHeight : 0)) / (float)dataCount);
                            }
                        }

                        var rr = new Rectangle(0, currentY, panelRect.Width, rh);

                        // اختر اللون
                        Color fill = (_alternateColor && r % 2 == 1) ? _rowOdd : _rowEven;
                        if (r == _hoveredRow && r != _selectedRow) fill = _rowHover;
                        if (r == _selectedRow) fill = _rowSelected;

                        using (var rb = new SolidBrush(fill))
                            g.FillRectangle(rb, rr);

                        // Accent bar
                        if (r == _selectedRow && _showAccent)
                            using (var ab = new SolidBrush(_accentColor))
                                g.FillRectangle(ab, new Rectangle(0, currentY, _accentWidth, rh));

                        // Separator
                        if (_showSeparators && r < dataCount - 1)
                            using (var sp = new Pen(_rowSeparator, 1f))
                                g.DrawLine(sp, 4, currentY + rh, panelRect.Width - 4, currentY + rh);

                        currentY += rh;
                    }
                }

                // ── Header Gradient ───────────────────────────────────
                if (_showHeaderStyle && _headerHeight > 0)
                {
                    var hr = new Rectangle(0, 0, panelRect.Width, _headerHeight);
                    var dir = _headerGradientHorizontal
                        ? LinearGradientMode.Horizontal
                        : LinearGradientMode.Vertical;

                    using (var hb = new LinearGradientBrush(hr, _headerStart, _headerEnd, dir))
                    {
                        hb.SetBlendTriangularShape(0.5f, 1f);
                        g.FillRectangle(hb, hr);
                    }

                    var shadowH = new Rectangle(0, _headerHeight, panelRect.Width, 5);
                    if (shadowH.Bottom <= panelRect.Height)
                        using (var hs = new LinearGradientBrush(shadowH,
                            Color.FromArgb(20, 0, 0, 0), Color.Transparent, LinearGradientMode.Vertical))
                            g.FillRectangle(hs, shadowH);
                }

                g.ResetClip();
            }

            // Border
            if (_borderThickness > 0)
                using (var path = Rounded(panelRect, _borderRadius))
                using (var pen = new Pen(_borderColor, _borderThickness))
                    g.DrawPath(pen, path);
        }

        protected override void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        private static GraphicsPath Rounded(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            int d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
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