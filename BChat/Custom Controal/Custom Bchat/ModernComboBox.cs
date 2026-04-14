using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace BChat.Controls
{
    [DefaultEvent("SelectedIndexChanged")]
    [ToolboxItem(true)]
    public class ModernComboBox : Control
    {
        // ─── Fields ───────────────────────────────────────────
        private string _labelText = "";
        private string _placeholderText = "";
        private int _selectedIndex = -1;
        private bool _isOpen = false;

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

        private readonly Collection<string> _items = new();
        private Form? _dropdownForm;

        // ─── Events ───────────────────────────────────────────
        public event EventHandler? SelectionChanged;
        public event EventHandler? SelectedIndexChanged;

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
            Size = new Size(220, 70);
            Font = new Font("Cairo", 10f);
            RightToLeft = RightToLeft.Yes;
            Cursor = Cursors.Hand;
        }

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
        public bool UsePlaceholder { get; set; } = true;

        [Category("BChat")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value < -1 || value >= _items.Count)
                    value = -1;

                if (_selectedIndex == value)
                    return;

                _selectedIndex = value;

                Invalidate();

                SelectionChanged?.Invoke(this, EventArgs.Empty);
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string? SelectedItem
        {
            get => SelectedIndex >= 0 && SelectedIndex < _items.Count
                ? _items[SelectedIndex]
                : null;

            set
            {
                if (value == null)
                {
                    SelectedIndex = -1;
                    return;
                }

                int index = _items.IndexOf(value);
                if (index >= 0)
                    SelectedIndex = index;
            }
        }

        [Category("BChat")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<string> Items => _items;

        // ─── Appearance Properties ────────────────────────────

        [Category("BChat - Appearance")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("BChat - Appearance")]
        public Color FocusBorderColor
        {
            get => _focusBorderColor;
            set { _focusBorderColor = value; Invalidate(); }
        }

        [Category("BChat - Appearance")]
        public Color TextColor
        {
            get => _textColor;
            set { _textColor = value; Invalidate(); }
        }

        [Category("BChat - Appearance")]
        public Color ArrowColor
        {
            get => _arrowColor;
            set { _arrowColor = value; Invalidate(); }
        }

        [Category("BChat - Appearance")]
        public Color DropdownBackColor
        {
            get => _dropdownBg;
            set { _dropdownBg = value; }
        }

        [Category("BChat - Appearance")]
        public Color ItemHoverColor
        {
            get => _itemHoverColor;
            set { _itemHoverColor = value; }
        }

        // ─── Items API ────────────────────────────────────────
        public void AddItem(string text)
        {
            _items.Add(text);

            if (_selectedIndex == -1)
                _selectedIndex = 0;

            Invalidate();
        }

        public void AddItems(IEnumerable<string> items)
        {
            foreach (var item in items)
                _items.Add(item);

            if (_selectedIndex == -1 && _items.Count > 0)
                _selectedIndex = 0;

            Invalidate();
        }

        public void ClearItems()
        {
            _items.Clear();
            _selectedIndex = -1;
            Invalidate();
        }

        // ─── Mouse ────────────────────────────────────────────
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            if (_isOpen)
                CloseDropdown();
            else
                OpenDropdown();
        }

        // ─── Dropdown ─────────────────────────────────────────
        private void OpenDropdown()
        {
            if (_items.Count == 0) return;

            _isOpen = true;

            int dropH = _items.Count * _itemHeight + 8;

            _dropdownForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                BackColor = Color.White,
                Size = new Size(Width, dropH),
                TopMost = true
            };

            _dropdownForm.Location = PointToScreen(new Point(0, Height));

            var panel = new DropdownPanel(
                _items,
                _selectedIndex,
                _itemHeight,
                Font,
                _textColor,
                _itemHoverColor,
                _borderRadius
            );

            panel.ItemSelected += (idx) =>
            {
                SelectedIndex = idx;
                CloseDropdown();
            };

            _dropdownForm.Controls.Add(panel);
            _dropdownForm.Deactivate += (s, e) => CloseDropdown();
            _dropdownForm.Show(this);
        }

        private void CloseDropdown()
        {
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

            g.Clear(Parent?.BackColor ?? Color.White);

            int boxTop = string.IsNullOrEmpty(_labelText) ? 0 : _labelHeight;
            var rect = new Rectangle(0, boxTop, Width - 1, Height - boxTop - 1);

            using var path = RoundedRect(rect, _borderRadius);
            using var bg = new SolidBrush(_backColor);
            g.FillPath(bg, path);

            using var pen = new Pen(_isOpen ? _focusBorderColor : _borderColor, 1.5f);
            g.DrawPath(pen, path);

            string text = SelectedItem ?? (UsePlaceholder ? _placeholderText : "");
            var color = SelectedItem != null ? _textColor : _placeholderColor;

            using var brush = new SolidBrush(color);
            g.DrawString(text, Font, brush,
                new PointF(Width - g.MeasureString(text, Font).Width - 40,
                           boxTop + (rect.Height - Font.Height) / 2));

            DrawArrow(g, boxTop, rect.Height);
        }

        private void DrawArrow(Graphics g, int top, int height)
        {
            int x = 18;
            int y = top + height / 2;

            var state = g.Save();
            g.TranslateTransform(x, y);

            if (_isOpen) g.RotateTransform(180);

            using var pen = new Pen(_arrowColor, 2);
            g.DrawLine(pen, -5, -2, 0, 3);
            g.DrawLine(pen, 0, 3, 5, -2);

            g.Restore(state);
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

    // ─── DropdownPanel ───────────────────────────────────────
    internal class DropdownPanel : Panel
    {
        private readonly IList<string> _items;
        private readonly int _itemH;
        private readonly Font _font;
        private readonly Color _textColor;
        private readonly Color _hoverColor;
        private readonly int _radius;

        private int _hovered = -1;
        private int _selectedIndex;

        public event Action<int>? ItemSelected;

        public DropdownPanel(
            IList<string> items,
            int selectedIndex,
            int itemH,
            Font font,
            Color textColor,
            Color hoverColor,
            int radius)
        {
            _items = items;
            _selectedIndex = selectedIndex;
            _itemH = itemH;
            _font = font;
            _textColor = textColor;
            _hoverColor = hoverColor;
            _radius = radius;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            Dock = DockStyle.Fill;
            BackColor = Color.White;

            MouseMove += (s, e) =>
            {
                int idx = (e.Y - 4) / _itemH;
                _hovered = (idx >= 0 && idx < _items.Count) ? idx : -1;
                Invalidate();
            };

            MouseLeave += (s, e) =>
            {
                _hovered = -1;
                Invalidate();
            };

            MouseClick += (s, e) =>
            {
                int idx = (e.Y - 4) / _itemH;
                if (idx >= 0 && idx < _items.Count)
                    ItemSelected?.Invoke(idx);
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            using var path = RoundedRect(rect, _radius);
            using var bg = new SolidBrush(Color.White);
            g.FillPath(bg, path);

            using var border = new Pen(Color.FromArgb(220, 215, 250));
            g.DrawPath(border, path);

            this.Region = new Region(path);

            for (int i = 0; i < _items.Count; i++)
            {
                var itemRect = new Rectangle(6, 4 + i * _itemH, Width - 12, _itemH - 4);

                if (i == _hovered || i == _selectedIndex)
                {
                    using var hPath = RoundedRect(itemRect, 8);
                    using var hBrush = new SolidBrush(_hoverColor);
                    g.FillPath(hBrush, hPath);
                }

                float textY = itemRect.Top + (itemRect.Height - _font.Height) / 2f;
                float textX = Width - g.MeasureString(_items[i], _font).Width - 16;

                using var tBrush = new SolidBrush(_textColor);
                g.DrawString(_items[i], _font, tBrush, new PointF(textX, textY));
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