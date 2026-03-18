using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BChat
{
    // ═══════════════════════════════════════════════════════════════
    //   AdvancedDatePicker  –  Custom WinForms Control
    //   • Default value : DateTime.Today  (always today's date)
    //   • Popup         : Full calendar with month navigation
    //   • Themes        : Light / Dark
    // ═══════════════════════════════════════════════════════════════

    [ToolboxItem(true)]
    [DefaultEvent("ValueChanged")]
    public class AdvancedDatePicker : Control
    {
        // ── Enums ────────────────────────────────────────────────────
        public enum ThemeMode { Light, Dark }

        // ── Private Fields ───────────────────────────────────────────
        private ThemeMode _theme = ThemeMode.Light;
        private Color _accent = Color.FromArgb(66, 133, 244);
        private DateTime _value = DateTime.Today;   // ✅ always today
        private bool _hover;
        private bool _focused;

        private Image _iconImage = null;
        private Size _iconSize = new Size(18, 18);
        private int _iconPad = 10;
        private Rectangle _iconRect;
        private int _radius = 10;
        private Padding _innerPad = new Padding(12, 8, 12, 8);
        private string _dateFormat = "dd MMM yyyy";

        private CalendarPopup _popup;

        // ── Event ────────────────────────────────────────────────────
        public event EventHandler ValueChanged;

        // ── Constructor ──────────────────────────────────────────────
        public AdvancedDatePicker()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor, true);

            Font = new Font("Segoe UI", 10f);
            Size = new Size(210, 40);
            BackColor = Color.White;
            Cursor = Cursors.Hand;
            TabStop = true;
        }

        // ════════════════════════════════════════════════════════════
        //  Public Properties
        // ════════════════════════════════════════════════════════════

        [Category("Behavior")]
        [Description("The selected date. Always starts from today's date.")]
        public DateTime Value
        {
            get => _value;
            set
            {
                if (_value.Date != value.Date)
                {
                    _value = value.Date;
                    Invalidate();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        public ThemeMode Theme
        {
            get => _theme;
            set { _theme = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color AccentColor
        {
            get => _accent;
            set { _accent = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Date format string. Default: dd MMM yyyy")]
        public string DateFormat
        {
            get => _dateFormat;
            set { _dateFormat = string.IsNullOrWhiteSpace(value) ? "dd MMM yyyy" : value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Optional icon on the right side of the control.")]
        [Browsable(true)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image IconImage
        {
            get => _iconImage;
            set
            {
                try { _iconImage = value; Invalidate(); }
                catch { _iconImage = null; if (!DesignMode) throw; }
            }
        }

        [Category("Appearance")]
        public Size IconSize
        {
            get => _iconSize;
            set { _iconSize = value; Invalidate(); }
        }

        [Category("Layout")]
        public int IconPadding
        {
            get => _iconPad;
            set { _iconPad = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout")]
        public int BorderRadius
        {
            get => _radius;
            set { _radius = Math.Max(4, value); Invalidate(); }
        }

        [Category("Layout")]
        public new Padding Padding
        {
            get => _innerPad;
            set { _innerPad = value; Invalidate(); }
        }

        // ════════════════════════════════════════════════════════════
        //  Painting
        // ════════════════════════════════════════════════════════════

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // ── Theme colors ─────────────────────────────────────────
            Color surface, border, textColor;
            if (_theme == ThemeMode.Light)
            {
                surface = Color.White;
                border = _focused ? _accent : Color.FromArgb(210, 214, 220);
                textColor = Color.FromArgb(35, 35, 35);
            }
            else
            {
                surface = Color.FromArgb(45, 45, 48);
                border = _focused ? _accent : Color.FromArgb(76, 76, 76);
                textColor = Color.WhiteSmoke;
            }

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);

            // ── Background ───────────────────────────────────────────
            using (GraphicsPath path = BuildRoundedPath(bounds, _radius))
            using (SolidBrush sb = new SolidBrush(surface))
            using (Pen pen = new Pen(border, 1.6f))
            {
                g.FillPath(sb, path);
                g.DrawPath(pen, path);
            }

            // ── Hover / focus tint ───────────────────────────────────
            if (_hover || _focused)
            {
                int alpha = _theme == ThemeMode.Light ? 12 : 18;
                using (SolidBrush tint = new SolidBrush(Color.FromArgb(alpha, _accent)))
                using (GraphicsPath ph = BuildRoundedPath(bounds, _radius))
                    g.FillPath(tint, ph);
            }

            // ── Icon column ──────────────────────────────────────────
            int iconW = Math.Max(_iconSize.Width, 16) + _iconPad * 2;
            _iconRect = new Rectangle(Width - iconW, 0, iconW, Height);

            // Separator line
            using (Pen sep = new Pen(Color.FromArgb(_theme == ThemeMode.Light ? 28 : 40, Color.Black), 1))
                g.DrawLine(sep, _iconRect.Left, 8, _iconRect.Left, Height - 8);

            // Icon drawing
            int icX = _iconRect.Left + (iconW - _iconSize.Width) / 2;
            int icY = (Height - _iconSize.Height) / 2;

            if (_iconImage != null)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(_iconImage, new Rectangle(icX, icY, _iconSize.Width, _iconSize.Height));
            }
            else
            {
                Color iconColor = (_hover || _focused) ? _accent : textColor;
                DrawCalendarIcon(g, new Rectangle(icX, icY, _iconSize.Width, _iconSize.Height), iconColor);
            }

            // ── Date text ────────────────────────────────────────────
            Rectangle textRect = new Rectangle(
                _innerPad.Left,
                _innerPad.Top,
                Width - _innerPad.Horizontal - iconW,
                Height - _innerPad.Vertical);

            using (SolidBrush tb = new SolidBrush(textColor))
            {
                string label = _value.ToString(_dateFormat, CultureInfo.InvariantCulture);
                StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(label, Font, tb, textRect, sf);
            }
        }

        // ════════════════════════════════════════════════════════════
        //  Drawing Helpers
        // ════════════════════════════════════════════════════════════

        private static GraphicsPath BuildRoundedPath(Rectangle r, int radius)
        {
            int d = radius * 2;
            GraphicsPath p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static void DrawCalendarIcon(Graphics g, Rectangle r, Color color)
        {
            using (Pen p = new Pen(color, 1.6f))
            using (SolidBrush b = new SolidBrush(color))
            {
                // Outer box
                Rectangle box = new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1);
                g.DrawRectangle(p, box);

                // Header fill
                int headerH = Math.Max(2, r.Height / 4);
                g.FillRectangle(b, box.X, box.Y, box.Width, headerH);

                // Two dots representing days
                int dot = Math.Max(2, r.Width / 8);
                g.FillEllipse(b, box.X + dot, box.Y + r.Height / 2, dot, dot);
                g.FillEllipse(b, box.Right - dot * 3, box.Y + r.Height / 2, dot, dot);
            }
        }

        // ════════════════════════════════════════════════════════════
        //  Mouse & Focus Events
        // ════════════════════════════════════════════════════════════

        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool h = ClientRectangle.Contains(e.Location);
            if (h != _hover) { _hover = h; Invalidate(); }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnGotFocus(EventArgs e)
        { _focused = true; Invalidate(); base.OnGotFocus(e); }

        protected override void OnLostFocus(EventArgs e)
        { _focused = false; Invalidate(); base.OnLostFocus(e); }

        protected override void OnClick(EventArgs e)
        { base.OnClick(e); Focus(); ShowPopup(); }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up: Value = _value.AddDays(-1); e.Handled = true; break;
                case Keys.Down: Value = _value.AddDays(1); e.Handled = true; break;
                case Keys.Left: Value = _value.AddDays(-1); e.Handled = true; break;
                case Keys.Right: Value = _value.AddDays(1); e.Handled = true; break;
                case Keys.Enter: ShowPopup(); e.Handled = true; break;
            }
            base.OnKeyDown(e);
        }

        // ════════════════════════════════════════════════════════════
        //  Popup
        // ════════════════════════════════════════════════════════════

        private void ShowPopup()
        {
            if (_popup != null && !_popup.IsDisposed && _popup.Visible)
            {
                _popup.Close();
                return;
            }

            _popup = new CalendarPopup(this, _theme, _accent, _value);
            Point screenPt = PointToScreen(new Point(0, Height + 4));
            _popup.StartPosition = FormStartPosition.Manual;
            _popup.Location = screenPt;
            _popup.DatePicked += (s, d) => Value = d;
            _popup.Show(this);
            _popup.AnimateIn();
        }

        // ════════════════════════════════════════════════════════════
        //  Inner Class: CalendarPopup
        // ════════════════════════════════════════════════════════════

        private sealed class CalendarPopup : Form
        {
            // ── Layout constants ─────────────────────────────────────
            private const int CELL_W = 40;
            private const int CELL_H = 34;
            private const int HEADER_H = 50;
            private const int WEEK_H = 24;
            private const int SIDE_PAD = 12;

            // ── State ────────────────────────────────────────────────
            private readonly AdvancedDatePicker _owner;
            private readonly ThemeMode _theme;
            private readonly Color _accent;
            private DateTime _displayMonth;

            private Rectangle _leftArrow;
            private Rectangle _rightArrow;
            private int _hoverDay = -1;

            public event EventHandler<DateTime> DatePicked;

            // ── Constructor ──────────────────────────────────────────
            public CalendarPopup(AdvancedDatePicker owner,
                                 ThemeMode theme, Color accent,
                                 DateTime selectedDate)
            {
                _owner = owner;
                _theme = theme;
                _accent = accent;
                _displayMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);

                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                DoubleBuffered = true;
                AutoSize = false;

                Width = CELL_W * 7 + SIDE_PAD * 2;
                Height = HEADER_H + WEEK_H + CELL_H * 6 + SIDE_PAD;
                BackColor = theme == ThemeMode.Light ? Color.White : Color.FromArgb(42, 42, 45);

                // ✅ Drops a subtle shadow effect via region + custom paint
                Deactivate += (s, e) => Close();

                // ✅ Clip the Form window to the rounded shape → transparent corners
                this.Load += (s, e) => ApplyRoundedRegion();
                this.Resize += (s, e) => ApplyRoundedRegion();
            }

            private void ApplyRoundedRegion()
            {
                Region?.Dispose();
                using (GraphicsPath path = RoundedPath(new Rectangle(0, 0, Width, Height), 12))
                    Region = new Region(path);
            }

            // ── Paint ────────────────────────────────────────────────
            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Rounded popup background
                using (SolidBrush sb = new SolidBrush(BackColor))
                using (GraphicsPath path = RoundedPath(new Rectangle(0, 0, Width - 1, Height - 1), 12))
                {
                    g.FillPath(sb, path);

                    // Subtle border
                    Color borderColor = _theme == ThemeMode.Light
                        ? Color.FromArgb(220, 222, 226)
                        : Color.FromArgb(70, 70, 75);
                    using (Pen pen = new Pen(borderColor, 1))
                        g.DrawPath(pen, path);
                }

                DrawHeader(g);
                DrawWeekDayLabels(g);
                DrawDayCells(g);
            }

            // ── Header (month/year + arrows) ─────────────────────────
            private void DrawHeader(Graphics g)
            {
                Rectangle headerRect = new Rectangle(0, 0, Width, HEADER_H);

                // Header background tint
                using (SolidBrush hb = new SolidBrush(
                    Color.FromArgb(_theme == ThemeMode.Light ? 240 : 55, _accent)))
                    g.FillRectangle(hb, headerRect);

                // Month + year label
                string title = _displayMonth.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
                using (Font f = new Font("Segoe UI", 11f, FontStyle.Bold))
                using (SolidBrush tb = new SolidBrush(
                    _theme == ThemeMode.Light ? Color.FromArgb(30, 30, 30) : Color.WhiteSmoke))
                {
                    StringFormat sf = new StringFormat
                    { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(title, f, tb, headerRect, sf);
                }

                // Navigation arrows
                _leftArrow = new Rectangle(12, (HEADER_H - 22) / 2, 22, 22);
                _rightArrow = new Rectangle(Width - 34, (HEADER_H - 22) / 2, 22, 22);

                DrawArrow(g, _leftArrow, false);
                DrawArrow(g, _rightArrow, true);
            }

            private void DrawArrow(Graphics g, Rectangle r, bool pointRight)
            {
                // Hover background
                bool hover = r.Contains(PointToClient(MousePosition));
                if (hover)
                {
                    using (SolidBrush hb = new SolidBrush(Color.FromArgb(30, _accent)))
                    using (GraphicsPath hp = RoundedPath(r, 5))
                        g.FillPath(hb, hp);
                }

                Color arrowColor = _theme == ThemeMode.Light
                    ? Color.FromArgb(50, 50, 50)
                    : Color.WhiteSmoke;

                using (Pen p = new Pen(arrowColor, 2f) { LineJoin = LineJoin.Round })
                {
                    int cx = r.X + r.Width / 2;
                    int cy = r.Y + r.Height / 2;

                    if (!pointRight)
                    {
                        // Left chevron ‹
                        g.DrawLines(p, new[] {
                            new Point(cx + 4, cy - 6),
                            new Point(cx - 3, cy),
                            new Point(cx + 4, cy + 6)
                        });
                    }
                    else
                    {
                        // Right chevron ›
                        g.DrawLines(p, new[] {
                            new Point(cx - 4, cy - 6),
                            new Point(cx + 3, cy),
                            new Point(cx - 4, cy + 6)
                        });
                    }
                }
            }

            // ── Week day labels (Su Mo Tu …) ─────────────────────────
            private void DrawWeekDayLabels(Graphics g)
            {
                string[] days = { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };
                Color labelColor = _theme == ThemeMode.Light
                    ? Color.FromArgb(120, 120, 125)
                    : Color.FromArgb(150, 150, 155);

                using (Font f = new Font("Segoe UI", 8f, FontStyle.Bold))
                using (SolidBrush b = new SolidBrush(labelColor))
                {
                    StringFormat sf = new StringFormat
                    { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                    for (int i = 0; i < 7; i++)
                    {
                        Rectangle r = new Rectangle(SIDE_PAD + i * CELL_W, HEADER_H, CELL_W, WEEK_H);
                        g.DrawString(days[i], f, b, r, sf);
                    }
                }

                // Separator under day labels
                int sepY = HEADER_H + WEEK_H;
                using (Pen sep = new Pen(Color.FromArgb(
                    _theme == ThemeMode.Light ? 20 : 35, Color.Black), 1))
                    g.DrawLine(sep, SIDE_PAD, sepY, Width - SIDE_PAD, sepY);
            }

            // ── Day cells ────────────────────────────────────────────
            private void DrawDayCells(Graphics g)
            {
                DateTime firstDay = new DateTime(_displayMonth.Year, _displayMonth.Month, 1);
                int startCol = (int)firstDay.DayOfWeek;
                int daysInMonth = DateTime.DaysInMonth(_displayMonth.Year, _displayMonth.Month);
                DateTime today = DateTime.Today;

                int xOff = SIDE_PAD;
                int yOff = HEADER_H + WEEK_H + 4;

                Color normalText = _theme == ThemeMode.Light ? Color.FromArgb(30, 30, 30) : Color.WhiteSmoke;
                Color todayText = _accent;
                Color selectedText = Color.White;
                Color sundayColor = Color.FromArgb(220, 60, 60);   // Sunday highlight

                for (int d = 1; d <= daysInMonth; d++)
                {
                    int index = startCol + d - 1;
                    int row = index / 7;
                    int col = index % 7;

                    Rectangle cell = new Rectangle(xOff + col * CELL_W, yOff + row * CELL_H, CELL_W, CELL_H);
                    DateTime thisDay = new DateTime(_displayMonth.Year, _displayMonth.Month, d);

                    bool isSelected = thisDay.Date == _owner.Value.Date;
                    bool isToday = thisDay.Date == today;
                    bool isHover = d == _hoverDay;
                    bool isSunday = col == 0;

                    // ── Cell background ──────────────────────────────
                    if (isSelected)
                    {
                        // Filled circle for selected day
                        Rectangle circle = GetCellCircle(cell);
                        using (SolidBrush sb = new SolidBrush(_accent))
                        using (GraphicsPath cp = RoundedPath(circle, circle.Width / 2))
                            g.FillPath(sb, cp);
                    }
                    else if (isToday)
                    {
                        // Outlined ring for today
                        Rectangle circle = GetCellCircle(cell);
                        using (Pen p = new Pen(_accent, 1.8f))
                        using (GraphicsPath cp = RoundedPath(circle, circle.Width / 2))
                            g.DrawPath(p, cp);
                    }
                    else if (isHover)
                    {
                        // Subtle fill on hover
                        Rectangle circle = GetCellCircle(cell);
                        using (SolidBrush sb = new SolidBrush(Color.FromArgb(30, _accent)))
                        using (GraphicsPath cp = RoundedPath(circle, circle.Width / 2))
                            g.FillPath(sb, cp);
                    }

                    // ── Day number ───────────────────────────────────
                    Color textColor = isSelected
                        ? selectedText
                        : (isToday
                            ? todayText
                            : (isSunday ? sundayColor : normalText));

                    FontStyle fs = (isSelected || isToday) ? FontStyle.Bold : FontStyle.Regular;

                    using (Font f = new Font("Segoe UI", 9.5f, fs))
                    using (SolidBrush tb = new SolidBrush(textColor))
                    {
                        StringFormat sf = new StringFormat
                        { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString(d.ToString(), f, tb, cell, sf);
                    }
                }
            }

            // ── Get a centred circle/square inside a cell ────────────
            private static Rectangle GetCellCircle(Rectangle cell)
            {
                int size = Math.Min(cell.Width, cell.Height) - 6;
                return new Rectangle(
                    cell.X + (cell.Width - size) / 2,
                    cell.Y + (cell.Height - size) / 2,
                    size, size);
            }

            // ── Mouse Events ─────────────────────────────────────────
            protected override void OnMouseMove(MouseEventArgs e)
            {
                int old = _hoverDay;
                _hoverDay = GetDayAt(e.Location);
                Cursor = (_hoverDay > 0 ||
                             _leftArrow.Contains(e.Location) ||
                             _rightArrow.Contains(e.Location))
                            ? Cursors.Hand : Cursors.Default;
                if (_hoverDay != old) Invalidate();
                base.OnMouseMove(e);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                _hoverDay = -1;
                Cursor = Cursors.Default;
                Invalidate();
                base.OnMouseLeave(e);
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                if (_leftArrow.Contains(e.Location))
                {
                    _displayMonth = _displayMonth.AddMonths(-1);
                    Invalidate();
                }
                else if (_rightArrow.Contains(e.Location))
                {
                    _displayMonth = _displayMonth.AddMonths(1);
                    Invalidate();
                }
                else
                {
                    int day = GetDayAt(e.Location);
                    if (day > 0)
                    {
                        DatePicked?.Invoke(this,
                            new DateTime(_displayMonth.Year, _displayMonth.Month, day));
                        Close();
                    }
                }
            }

            // ── Hit-test helper ──────────────────────────────────────
            private int GetDayAt(Point pt)
            {
                DateTime firstDay = new DateTime(_displayMonth.Year, _displayMonth.Month, 1);
                int startCol = (int)firstDay.DayOfWeek;
                int daysInMonth = DateTime.DaysInMonth(_displayMonth.Year, _displayMonth.Month);

                int xOff = SIDE_PAD;
                int yOff = HEADER_H + WEEK_H + 4;

                for (int d = 1; d <= daysInMonth; d++)
                {
                    int index = startCol + d - 1;
                    int row = index / 7;
                    int col = index % 7;

                    Rectangle cell = new Rectangle(
                        xOff + col * CELL_W, yOff + row * CELL_H, CELL_W, CELL_H);

                    if (cell.Contains(pt)) return d;
                }
                return -1;
            }

            // ── Rounded path helper ──────────────────────────────────
            private static GraphicsPath RoundedPath(Rectangle r, int radius)
            {
                int d = radius * 2;
                GraphicsPath p = new GraphicsPath();
                p.AddArc(r.X, r.Y, d, d, 180, 90);
                p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
                p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
                p.CloseFigure();
                return p;
            }

            // ── Fade-in animation ────────────────────────────────────
            public void AnimateIn()
            {
                Opacity = 0;
                Timer t = new Timer { Interval = 12 };
                t.Tick += delegate
                {
                    Opacity += 0.12;
                    if (Opacity >= 1) { Opacity = 1; t.Stop(); t.Dispose(); }
                };
                t.Start();
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  GraphicsExtensions  –  Shared drawing utilities
    // ═══════════════════════════════════════════════════════════════

    internal static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (GraphicsPath p = RoundedRect(rect, radius))
                g.FillPath(brush, p);
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle rect, int radius)
        {
            using (GraphicsPath p = RoundedRect(rect, radius))
                g.DrawPath(pen, p);
        }

        private static GraphicsPath RoundedRect(Rectangle b, int radius)
        {
            int d = radius * 2;
            GraphicsPath p = new GraphicsPath();
            p.AddArc(b.X, b.Y, d, d, 180, 90);
            p.AddArc(b.Right - d, b.Y, d, d, 270, 90);
            p.AddArc(b.Right - d, b.Bottom - d, d, d, 0, 90);
            p.AddArc(b.X, b.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}