// Controls/ModernComboBox.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat.Controls
{
    [DefaultEvent("SelectionChanged")]
    [ToolboxItem(true)]
    public class ModernComboBox : Control
    {
        // ─── Fields ───────────────────────────────────────────
        private string _labelText = "";
        private string _placeholderText = "";
        private int _selectedIndex = -1;
        private bool _isOpen = false;
        private bool _isHovered = false;

        private Color _backColor = Color.FromArgb(237, 235, 255);
        private Color _borderColor = Color.FromArgb(220, 215, 250);
        private Color _focusBorderColor = Color.FromArgb(124, 111, 247);
        private Color _textColor = Color.FromArgb(40, 40, 70);
        private Color _placeholderColor = Color.FromArgb(180, 160, 200);
        private Color _labelColor = Color.FromArgb(60, 60, 90);
        private Color _dropdownBg = Color.White;
        private Color _itemHoverColor = Color.FromArgb(237, 235, 255);
        private Color _arrowColor = Color.FromArgb(124, 111, 247);

        private int _borderRadius = 14;
        private int _labelHeight = 24;
        private int _itemHeight = 34;
        private int _hoveredItem = -1;

        private readonly System.Collections.Generic.List<string> _items = new();

        // Dropdown overlay form
        private Form? _dropdownForm;

        // ─── Properties ───────────────────────────────────────

        [Category("BChat")]
        public string LabelText
        {
            get => _labelText;
            set { _labelText = value; Invalidate(); }
        }

        [Category("BChat")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set { _placeholderText = value; Invalidate(); }
        }

        [Category("BChat")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= -1 && value < _items.Count)
                {
                    _selectedIndex = value;
                    Invalidate();
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string? SelectedItem 
            => _selectedIndex >= 0 && _selectedIndex < _items.Count
               ? _items[_selectedIndex] : null;

        // بعد SelectedItem مباشرة
        public string? SelectedValue
        {
            get => SelectedItem;
            set
            {
                int idx = _items.IndexOf(value ?? "");
                if (idx >= 0)
                    SelectedIndex = idx;
            }
        }

        [Category("BChat")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, value); Invalidate(); }
        }

        // ─── Events ───────────────────────────────────────────
        public event EventHandler? SelectionChanged;

        // ─── Items API ────────────────────────────────────────
        public void AddItem(string item) => _items.Add(item);

        public void AddItems(IEnumerable<string> items)
        {
            _items.AddRange(items);
            Invalidate();
        }

        public void ClearItems()
        {
            _items.Clear();
            _selectedIndex = -1;
            Invalidate();
        }

        // ─── Constructor ──────────────────────────────────────
        public ModernComboBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.Selectable, true);

            DoubleBuffered = true;
            TabStop = true;
            Size = new Size(220, 70);
            Font = new Font("Cairo", 10f);
            RightToLeft = RightToLeft.Yes;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        // ─── Mouse ────────────────────────────────────────────
        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true; Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false; Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            if (_isOpen) CloseDropdown();
            else OpenDropdown();
            base.OnMouseDown(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        // ─── Keyboard ─────────────────────────────────────────
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Down)
            {
                SelectedIndex = Math.Min(_selectedIndex + 1, _items.Count - 1);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                SelectedIndex = Math.Max(_selectedIndex - 1, 0);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                if (_isOpen) CloseDropdown();
                else OpenDropdown();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CloseDropdown();
            }
        }

        // ─── Dropdown ─────────────────────────────────────────
        private void OpenDropdown()
        {
            if (_items.Count == 0) return;

            _isOpen = true;
            Invalidate();

            int dropH = _items.Count * _itemHeight + 8;

            // نافذة بدون حدود تظهر تحت الـ Control
            _dropdownForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                BackColor = _dropdownBg,
                Size = new Size(Width, dropH),
            };

            // تحديد موقعه تحت الـ Control مباشرةً
            var screenPt = PointToScreen(new Point(0, Height - _labelHeight));
            _dropdownForm.Location = screenPt;

            // رسم الـ Dropdown
            var panel = new DropdownPanel(_items, _selectedIndex, _itemHeight,
                                          Font, _textColor, _itemHoverColor,
                                          _focusBorderColor, _borderRadius);

            panel.ItemSelected += (idx) =>
            {
                SelectedIndex = idx;
                CloseDropdown();
            };

            _dropdownForm.Controls.Add(panel);
            _dropdownForm.Deactivate += (s, e) => CloseDropdown();
            _dropdownForm.Show(this.FindForm());
        }

        private void CloseDropdown()
        {
            if (!_isOpen) return;
            _isOpen = false;
            _dropdownForm?.Close();
            _dropdownForm = null;
            Invalidate();
        }

        // ─── Paint ────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? Color.WhiteSmoke);

            // ── 1. Label ──────────────────────────────────────
            if (!string.IsNullOrEmpty(_labelText))
            {
                using var labelBrush = new SolidBrush(_labelColor);
                using var labelFont = new Font("Cairo", 9.5f);
                var labelRect = new RectangleF(0, 0, Width, _labelHeight);
                var labelFormat = new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(_labelText, labelFont, labelBrush, labelRect, labelFormat);
            }

            // ── 2. Box ────────────────────────────────────────
            int boxTop = string.IsNullOrEmpty(_labelText) ? 0 : _labelHeight;
            int boxH = Height - boxTop;
            var boxRect = new Rectangle(0, boxTop, Width - 1, boxH - 1);

            using var path = RoundedRect(boxRect, _borderRadius);

            using var bgBrush = new SolidBrush(_backColor);
            g.FillPath(bgBrush, path);

            bool focused = _isOpen || Focused;
            Color borderC = focused ? _focusBorderColor : _borderColor;
            float borderW = focused ? 1.8f : 1f;
            using var pen = new Pen(borderC, borderW);
            g.DrawPath(pen, path);

            // ── 3. النص ───────────────────────────────────────
            string drawText = SelectedItem ?? _placeholderText;
            Color drawColor = SelectedItem != null ? _textColor : _placeholderColor;

            float textY = boxTop + (boxH - Font.Height) / 2f;
            float textX = Width - 44f; // يمين مع مسافة للسهم

            using var textBrush = new SolidBrush(drawColor);
            g.DrawString(drawText, Font, textBrush, new PointF(textX - g.MeasureString(drawText, Font).Width, textY));

            // ── 4. سهم الـ Dropdown ───────────────────────────
            DrawArrow(g, boxTop, boxH);
        }

        private void DrawArrow(Graphics g, int boxTop, int boxH)
        {
            // السهم على اليسار لأنه RTL
            int arrowX = 18;
            int arrowY = boxTop + boxH / 2;
            float rotation = _isOpen ? 180f : 0f;

            var state = g.Save();
            g.TranslateTransform(arrowX, arrowY);
            g.RotateTransform(rotation);

            using var arrowPen = new Pen(_arrowColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(arrowPen, -5, -2, 0, 3);
            g.DrawLine(arrowPen, 0, 3, 5, -2);

            g.Restore(state);
        }

        // ─── Helper ───────────────────────────────────────────
        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // ─── DropdownPanel ────────────────────────────────────────────
    internal class DropdownPanel : Panel
    {
        private readonly List<string> _items;
        private readonly int _itemH;
        private readonly Font _font;
        private readonly Color _textColor;
        private readonly Color _hoverColor;
        private readonly Color _accentColor;
        private readonly int _radius;
        private int _hovered = -1;
        private int _selected;

        public event Action<int>? ItemSelected;

        public DropdownPanel(List<string> items, int selected, int itemH,
                             Font font, Color textColor, Color hoverColor,
                             Color accentColor, int radius)
        {
            _items = items;
            _selected = selected;
            _itemH = itemH;
            _font = font;
            _textColor = textColor;
            _hoverColor = hoverColor;
            _accentColor = accentColor;
            _radius = radius;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            Dock = DockStyle.Fill;
            BackColor = Color.White;
            RightToLeft = RightToLeft.Yes;
            Cursor = Cursors.Hand;

            MouseMove += OnMouseMoved;
            MouseLeave += (s, e) => { _hovered = -1; Invalidate(); };
            MouseClick += OnMouseClicked;
        }

        private void OnMouseMoved(object? sender, MouseEventArgs e)
        {
            int idx = (e.Y - 4) / _itemH;
            if (idx != _hovered)
            {
                _hovered = idx >= 0 && idx < _items.Count ? idx : -1;
                Invalidate();
            }
        }

        private void OnMouseClicked(object? sender, MouseEventArgs e)
        {
            int idx = (e.Y - 4) / _itemH;
            if (idx >= 0 && idx < _items.Count)
                ItemSelected?.Invoke(idx);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            // Shadow خفيف
            using var shadowPen = new Pen(Color.FromArgb(20, 0, 0, 0), 1f);
            g.DrawRectangle(shadowPen, 0, 0, Width - 1, Height - 1);

            for (int i = 0; i < _items.Count; i++)
            {
                var itemRect = new Rectangle(4, 4 + i * _itemH, Width - 8, _itemH - 2);

                // Hover / Selected
                if (i == _hovered || i == _selected)
                {
                    using var hBrush = new SolidBrush(
                        i == _selected
                            ? Color.FromArgb(30, 124, 111, 247)
                            : _hoverColor);

                    using var hPath = RoundedRect(itemRect, 8);
                    g.FillPath(hBrush, hPath);
                }

                // النص
                float textY = 4 + i * _itemH + (_itemH - _font.Height) / 2f;
                float textX = Width - g.MeasureString(_items[i], _font).Width - 14f;
                using var tBrush = new SolidBrush(
                    i == _selected ? _accentColor : _textColor);
                g.DrawString(_items[i], _font, tBrush, new PointF(textX, textY));

                // نقطة للعنصر المحدد
                if (i == _selected)
                {
                    using var dotBrush = new SolidBrush(_accentColor);
                    g.FillEllipse(dotBrush, 10, 4 + i * _itemH + (_itemH - 6) / 2, 6, 6);
                }
            }
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}