using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace BChat
{
    public class SegmentedControl : UserControl
    {
        private int _selected = 0;
        private string[] _options = { "الخيار 1", "الخيار 2" };
        private string[] _subtitles = { "وصف 1", "وصف 2" };
        private int _hovered = -1;

        public event EventHandler<int> SelectionChanged;

        // ── Properties ───────────────────────────────────────

        [Category("BChat")]
        [Description("الخيارات — افصل بينها بفاصلة")]
        public string Options
        {
            get => string.Join(",", _options);
            set { _options = value.Split(','); Invalidate(); }
        }

        [Category("BChat")]
        [Description("النصوص الفرعية — افصل بينها بفاصلة")]
        public string Subtitles
        {
            get => string.Join(",", _subtitles);
            set { _subtitles = value.Split(','); Invalidate(); }
        }

        [Category("BChat")]
        public int SelectedIndex
        {
            get => _selected;
            set { _selected = value; Invalidate(); }
        }

        [Category("BChat")]
        [Description("حجم النص الرئيسي")]
        public float OptionFontSize { get; set; } = 10f;

        [Category("BChat")]
        [Description("حجم النص الفرعي")]
        public float SubtitleFontSize { get; set; } = 8.5f;

        [Category("BChat")]
        [Description("المسافة بين النص الرئيسي والفرعي")]
        public int LineSpacing { get; set; } = 22;

        // ── ألوان BChat ───────────────────────────────────────

        [Category("BChat - Colors")]
        [Description("لون الـ Accent الرئيسي — للحدود")]
        public Color AccentColor { get; set; } = Color.FromArgb(124, 111, 247);

        [Category("BChat - Colors")]
        [Description("خلفية العنصر المحدد")]
        public Color SelectedBackground { get; set; } = Color.FromArgb(124, 111, 247);

        [Category("BChat - Colors")]
        [Description("لون النص عند التحديد")]
        public Color SelectedTextColor { get; set; } = Color.White;

        [Category("BChat - Colors")]
        [Description("خلفية العناصر غير المحددة")]
        public Color UnselectedBackground { get; set; } = Color.FromArgb(26, 31, 60);

        [Category("BChat - Colors")]
        [Description("خلفية العنصر عند Hover")]
        public Color HoverBackground { get; set; } = Color.FromArgb(37, 43, 74);

        [Category("BChat - Colors")]
        [Description("لون حدود العناصر غير المحددة")]
        public Color UnselectedBorder { get; set; } = Color.FromArgb(220, 220, 235);

        [Category("BChat - Colors")]
        [Description("لون النص غير المحدد")]
        public Color UnselectedTextColor { get; set; } = Color.FromArgb(200, 200, 220);

        [Category("BChat - Colors")]
        [Description("لون النص الفرعي غير المحدد")]
        public Color UnselectedSubtitleColor { get; set; } = Color.FromArgb(140, 140, 160);
        public Color AccentLight { get; internal set; }

        // ── Constructor ───────────────────────────────────────

        public SegmentedControl()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            Height = 70;
            Cursor = Cursors.Hand;
        }

        // ── Public API ────────────────────────────────────────

        public void SetOptions(string[] options, string[] subtitles)
        {
            _options = options;
            _subtitles = subtitles;
            Invalidate();
        }

        public void UpdateSubtitle(int index, string subtitle)
        {
            if (index >= 0 && index < _subtitles.Length)
            {
                _subtitles[index] = subtitle;
                Invalidate();
            }
        }

        // ── Mouse ─────────────────────────────────────────────

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int h = HitTest(e.X);
            if (h != _hovered) { _hovered = h; Invalidate(); }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hovered = -1; Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            int h = HitTest(e.X);
            if (h < 0) return;

            if (h == _selected)
                _selected = -1;  // إلغاء التحديد
            else
                _selected = h;   // تحديد جديد

            Invalidate();
            SelectionChanged?.Invoke(this, _selected);
        }
        private int HitTest(int x)
        {
            if (_options == null) return -1;
            int w = Width / _options.Length;
            return Math.Min(x / w, _options.Length - 1);
        }

        // ── Paint ─────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_options == null) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);

            int w = Width / _options.Length;

            for (int i = 0; i < _options.Length; i++)
            {
                var rect = new Rectangle(i * w, 0, w - 4, Height - 2);
                bool isSelected = i == _selected && _selected != -1;
                bool isHovered = i == _hovered && !isSelected;

                // ── خلفية ─────────────────────────────────────
                Color bg = isSelected ? SelectedBackground
                         : isHovered ? HoverBackground
                                      : UnselectedBackground;

                using (var path = RoundRect(rect, 10))
                using (var brush = new SolidBrush(bg))
                    g.FillPath(brush, path);

                // ── حدود ──────────────────────────────────────
                Color border = isSelected ? AccentColor : UnselectedBorder;
                int borderW = isSelected ? 2 : 1;

                using (var path = RoundRect(rect, 10))
                using (var pen = new Pen(border, borderW))
                    g.DrawPath(pen, path);

                // ── النص الرئيسي ──────────────────────────────
                Color textColor = isSelected ? SelectedTextColor : UnselectedTextColor;

                using (var font = new Font("IBM Plex Sans Arabic", OptionFontSize, FontStyle.Bold))
                using (var brush = new SolidBrush(textColor))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Near
                    };
                    g.DrawString(_options[i], font, brush,
                        new Rectangle(rect.X + 8, rect.Y + 10, rect.Width - 16, 30), sf);
                }

                // ── النص الفرعي ───────────────────────────────
                if (_subtitles != null && i < _subtitles.Length)
                {
                    Color subColor = isSelected ? SelectedTextColor : UnselectedSubtitleColor;

                    using (var font = new Font("IBM Plex Sans Arabic", SubtitleFontSize))
                    using (var brush = new SolidBrush(subColor))
                    {
                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Near
                        };
                        g.DrawString(_subtitles[i], font, brush,
                            new Rectangle(rect.X + 8, rect.Y + 10 + LineSpacing, rect.Width - 16, 24), sf);
                    }
                }
            }
        }

        // ── Helper ────────────────────────────────────────────

        private GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}