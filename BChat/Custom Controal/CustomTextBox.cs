using BChat.Controls;
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

        // ✅ MaxLength & TextAlign
        private int _maxLength = 32767;
        private HorizontalAlignment _textAlign = HorizontalAlignment.Left;

        // ============================================================
        //  Properties
        // ============================================================

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
            set { _borderRadius = Math.Max(0, value); Invalidate(); Update(); }
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

        // ✅ MaxLength
        [Category("Behavior")]
        [Description("Maximum number of characters the user can type.")]
        public int MaxLength
        {
            get => _maxLength;
            set { _maxLength = Math.Max(0, value); Invalidate(); }
        }

        // ✅ TextAlign
        [Category("Appearance")]
        [Description("The alignment of the text inside the textbox.")]
        public HorizontalAlignment TextAlign
        {
            get => _textAlign;
            set { _textAlign = value; Invalidate(); }
        }

        // ============================================================
        //  Constructor
        // ============================================================

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

            _caretTimer = new Timer { Interval = 500 };
            _caretTimer.Tick += (s, e) =>
            {
                _showCaret = !_showCaret;
                if (_focused) Invalidate();
            };
            _caretTimer.Start();
        }

        // ============================================================
        //  Focus
        // ============================================================

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

        // ============================================================
        //  Keyboard
        // ============================================================

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e); // ✅ يطلق الحدث للخارج أولاً

            if (e.Handled) return; // ✅ إذا الخارج قال Handled = true، توقف

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
                // ✅ تطبيق MaxLength
                if (_maxLength > 0 && _textValue.Length >= _maxLength)
                    return;

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

            // Ctrl + A
            if (e.Control && e.KeyCode == Keys.A)
            {
                _selectAll = true;
                Invalidate();
                e.SuppressKeyPress = true;
            }

            // Ctrl + C
            else if (e.Control && e.KeyCode == Keys.C)
            {
                if (!string.IsNullOrEmpty(_textValue))
                    Clipboard.SetText(_selectAll ? _textValue : GetCurrentWord());
                e.SuppressKeyPress = true;
            }

            // Ctrl + X
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

            // Ctrl + V
            else if (e.Control && e.KeyCode == Keys.V)
            {
                string clip = Clipboard.GetText();
                if (!string.IsNullOrEmpty(clip))
                {
                    // ✅ احترام MaxLength عند اللصق
                    int available = _maxLength - _textValue.Length;
                    if (available <= 0) return;
                    if (clip.Length > available)
                        clip = clip.Substring(0, available);

                    _textValue = _textValue.Insert(_caretIndex, clip);
                    _caretIndex += clip.Length;
                    Invalidate();
                }
                e.SuppressKeyPress = true;
            }

            // Left Arrow
            else if (e.KeyCode == Keys.Left)
            {
                if (_caretIndex > 0) _caretIndex--;
                _selectAll = false;
                Invalidate();
            }

            // Right Arrow
            else if (e.KeyCode == Keys.Right)
            {
                if (_caretIndex < _textValue.Length) _caretIndex++;
                _selectAll = false;
                Invalidate();
            }

            // Delete
            else if (e.KeyCode == Keys.Delete)
            {
                if (_textValue.Length > 0 && _caretIndex < _textValue.Length)
                    _textValue = _textValue.Remove(_caretIndex, 1);
                _selectAll = false;
                Invalidate();
            }
        }

        private string GetCurrentWord() => _textValue;

        // ============================================================
        //  Mouse
        // ============================================================

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            _selectAll = false;
            base.OnMouseDown(e);
        }

        // ============================================================
        //  Paint
        // ============================================================

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

            // حساب نقطة بداية النص (بعد الأيقونة إن وُجدت)
            int left = 10;
            if (_icon != null)
            {
                int iconY = (Height - _iconSize) / 2;
                e.Graphics.DrawImage(_icon, new Rectangle(left, iconY, _iconSize, _iconSize));
                left += _iconSize + _iconPadding;
            }

            // ✅ حساب X بناءً على TextAlign
            string drawText = string.IsNullOrEmpty(_textValue) ? _placeholderText : _textValue;
            Color drawColor = string.IsNullOrEmpty(_textValue) ? _placeholderColor : _textColor;
            float textWidth = e.Graphics.MeasureString(drawText, Font).Width;
            float y = (Height - Font.Height) / 2f;
            float x;

            switch (_textAlign)
            {
                case HorizontalAlignment.Center:
                    x = left + (Width - left - textWidth) / 2f;
                    break;
                case HorizontalAlignment.Right:
                    x = Width - textWidth - 10f;
                    break;
                default: // Left
                    x = left;
                    break;
            }

            // تحديد الكل (Select All)
            if (_selectAll && !string.IsNullOrEmpty(_textValue))
            {
                float selWidth = e.Graphics.MeasureString(_textValue, Font).Width;
                using (Brush selBrush = new SolidBrush(_selectionColor))
                    e.Graphics.FillRectangle(selBrush, new RectangleF(x, y, selWidth, Font.Height));
            }

            // رسم النص أو الـ Placeholder
            using (Brush textBrush = new SolidBrush(drawColor))
                e.Graphics.DrawString(drawText, Font, textBrush, new PointF(x, y));

            // ✅ رسم المؤشر (Caret) مع مراعاة TextAlign
            if (_focused && !_selectAll && _showCaret)
            {
                float caretOffset = e.Graphics.MeasureString(_textValue.Substring(0, _caretIndex), Font).Width;
                float caretX = x + caretOffset;
                e.Graphics.DrawLine(Pens.Black, caretX, y, caretX, y + Font.Height);
            }
        }

        // ============================================================
        //  Helper
        // ============================================================

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

        public static implicit operator CustomTextBox(ModernTextBox v)
        {
            throw new NotImplementedException();
        }
    }
}