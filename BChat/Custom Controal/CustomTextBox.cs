using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BChat
{
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    [ToolboxItem(true)]
    public partial class CustomTextBox : Control
    {
        private string _textValue = "";
        private string _placeholderText = "Search...";
        private Color _placeholderColor = Color.Gray;
        private Color _textColor = Color.Black;
        private Color _borderColor = Color.Silver;
        private Color _focusBorderColor = Color.DeepSkyBlue;
        private Color _backColor = Color.White;
        private int _borderRadius = 10;
        private int _borderThickness = 1;
        private Image _icon;
        private int _iconSize = 18;
        private int _iconPadding = 6;
        private bool _focused = false;
        private int _caretIndex = 0;

        // Selection
        private bool _selectAll = false;
        private Color _selectionColor = Color.FromArgb(100, Color.DeepSkyBlue);

        // Caret blink
        private bool _showCaret = true;
        private readonly Timer _caretTimer;

        [Category("Appearance")]
        public Image Icon { get => _icon; set { _icon = value; Invalidate(); } }

        [Category("Appearance")]
        public int IconSize { get => _iconSize; set { _iconSize = Math.Max(8, value); Invalidate(); } }

        [Category("Appearance")]
        public int IconPadding { get => _iconPadding; set { _iconPadding = Math.Max(0, value); Invalidate(); } }

        [Category("Appearance")]
        public Color PlaceholderColor { get => _placeholderColor; set { _placeholderColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color TextColor { get => _textColor; set { _textColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color FocusBorderColor { get => _focusBorderColor; set { _focusBorderColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BackColorEx { get => _backColor; set { _backColor = value; Invalidate(); } }

        [Category("Appearance")]
        [Description("Controls how rounded the corners of the textbox are.")]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = Math.Max(0, value);
                Invalidate();
                Update();
            }
        }

        [Category("Appearance")]
        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = Math.Max(1, value); Invalidate(); Update(); }
        }

        [Category("Placeholder")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set { _placeholderText = value; Invalidate(); }
        }

        [Category("Data")]
        public override string Text
        {
            get => _textValue;
            set
            {
                _textValue = value ?? "";
                _caretIndex = _textValue.Length;
                _selectAll = false;
                Invalidate();
                base.OnTextChanged(EventArgs.Empty);
            }
        }

        public CustomTextBox()
        {
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 10f);
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.Selectable, true);

            TabStop = true;

            // Caret blink timer
            _caretTimer = new Timer { Interval = 500 };
            _caretTimer.Tick += (s, e) =>
            {
                _showCaret = !_showCaret;
                if (_focused) Invalidate();
            };
            _caretTimer.Start();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _focused = true;
            _caretTimer.Start();
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _focused = false;
            _caretTimer.Stop();
            _selectAll = false;
            Invalidate();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (_selectAll)
            {
                _textValue = "";
                _caretIndex = 0;
                _selectAll = false;
            }

            if (e.KeyChar == '\b') // Backspace
            {
                if (_caretIndex > 0 && _textValue.Length > 0)
                {
                    _textValue = _textValue.Remove(_caretIndex - 1, 1);
                    _caretIndex--;
                }
            }
            else if (!char.IsControl(e.KeyChar))
            {
                _textValue = _textValue.Insert(_caretIndex, e.KeyChar.ToString());
                _caretIndex++;
            }

            _showCaret = true;
            Invalidate();
            base.OnTextChanged(EventArgs.Empty);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // ✅ Ctrl + A : Select All
            if (e.Control && e.KeyCode == Keys.A)
            {
                _selectAll = true;
                Invalidate();
                e.SuppressKeyPress = true;
            }

            // ✅ Ctrl + C : Copy
            else if (e.Control && e.KeyCode == Keys.C)
            {
                if (!string.IsNullOrEmpty(_textValue))
                    Clipboard.SetText(_selectAll ? _textValue : GetCurrentWord());
                e.SuppressKeyPress = true;
            }

            // ✅ Ctrl + X : Cut
            else if (e.Control && e.KeyCode == Keys.X)
            {
                if (!string.IsNullOrEmpty(_textValue))
                {
                    Clipboard.SetText(_textValue);
                    _textValue = "";
                    _caretIndex = 0;
                    Invalidate();
                }
                e.SuppressKeyPress = true;
            }

            // ✅ Ctrl + V : Paste
            else if (e.Control && e.KeyCode == Keys.V)
            {
                string clip = Clipboard.GetText();
                if (!string.IsNullOrEmpty(clip))
                {
                    _textValue = _textValue.Insert(_caretIndex, clip);
                    _caretIndex += clip.Length;
                    Invalidate();
                }
                e.SuppressKeyPress = true;
            }

            // ✅ Left / Right Arrows
            else if (e.KeyCode == Keys.Left)
            {
                if (_caretIndex > 0) _caretIndex--;
                _selectAll = false;
                Invalidate();
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (_caretIndex < _textValue.Length) _caretIndex++;
                _selectAll = false;
                Invalidate();
            }

            // ✅ Delete
            else if (e.KeyCode == Keys.Delete)
            {
                if (_textValue.Length > 0 && _caretIndex < _textValue.Length)
                {
                    _textValue = _textValue.Remove(_caretIndex, 1);
                }
                _selectAll = false;
                Invalidate();
            }
        }

        private string GetCurrentWord()
        {
            // بسيطة جدًا — لاحقًا ممكن نطورها
            return _textValue;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            _selectAll = false;
            base.OnMouseDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Parent?.BackColor ?? Color.White);

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = RoundedRect(rect, _borderRadius))
            using (SolidBrush bg = new SolidBrush(_backColor))
            using (Pen pen = new Pen(_focused ? _focusBorderColor : _borderColor, _borderThickness))
            {
                e.Graphics.FillPath(bg, path);
                e.Graphics.DrawPath(pen, path);
            }

            int left = 10;
            if (_icon != null)
            {
                int iconY = (Height - _iconSize) / 2;
                e.Graphics.DrawImage(_icon, new Rectangle(left, iconY, _iconSize, _iconSize));
                left += _iconSize + _iconPadding;
            }

            // تحديد الكل
            if (_selectAll && !string.IsNullOrEmpty(_textValue))
            {
                using (Brush selBrush = new SolidBrush(_selectionColor))
                {
                    float textWidth = e.Graphics.MeasureString(_textValue, Font).Width;
                    var y = (Height - Font.Height) / 2f;
                    e.Graphics.FillRectangle(selBrush, new RectangleF(left, y, textWidth, Font.Height));
                }
            }

            // النص أو placeholder
            string drawText = string.IsNullOrEmpty(_textValue) ? _placeholderText : _textValue;
            Color drawColor = string.IsNullOrEmpty(_textValue) ? _placeholderColor : _textColor;

            using (Brush textBrush = new SolidBrush(drawColor))
            {
                var y = (Height - Font.Height) / 2f;
                e.Graphics.DrawString(drawText, Font, textBrush, new PointF(left, y));
            }

            // المؤشر (Caret)
            if (_focused && !_selectAll && _showCaret)
            {
                float textWidth = e.Graphics.MeasureString(_textValue.Substring(0, _caretIndex), Font).Width;
                float caretX = left + textWidth;
                float caretY = (Height - Font.Height) / 2f;
                e.Graphics.DrawLine(Pens.Black, caretX, caretY, caretX, caretY + Font.Height);
            }
        }

        private GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
