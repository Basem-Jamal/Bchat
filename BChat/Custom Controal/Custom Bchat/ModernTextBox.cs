using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BChat.Controls
{
    public enum TextDirection
    {
        Auto,
        RTL,
        LTR
    }

    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    [ToolboxItem(true)]
    public class ModernTextBox : Control
    {
        // ─── Fields ───────────────────────────────────────────
        private string _textValue = "";
        private string _placeholderText = "";
        private string _labelText = "";

        private Color _placeholderColor = Color.FromArgb(180, 160, 200);
        private Color _textColor = Color.FromArgb(40, 40, 70);
        private Color _backColorEx = Color.FromArgb(237, 235, 255);
        private Color _borderColor = Color.FromArgb(220, 215, 250);
        private Color _focusBorderColor = Color.FromArgb(124, 111, 247);
        private Color _labelColor = Color.FromArgb(60, 60, 90);
        private Color _selectionColor = Color.FromArgb(80, 124, 111, 247);

        private int _borderRadius = 14;
        private int _labelHeight = 24;
        private int _caretIndex = 0;
        private bool _focused = false;
        private bool _showCaret = true;
        private bool _selectAll = false;
        private int _maxLength = 32767;

        private bool _usePasswordChar = false;
        private int _padding = 14;
        private TextDirection _direction = TextDirection.Auto;

        private readonly Timer _caretTimer;

        // ─── Constructor ──────────────────────────────────────
        public ModernTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.Selectable, true);

            DoubleBuffered = true;
            TabStop = true;

            Size = new Size(220, 70);
            Font = new Font("Cairo", 10f);

            RightToLeft = RightToLeft.Yes;

            _caretTimer = new Timer { Interval = 500 };
            _caretTimer.Tick += (s, e) =>
            {
                _showCaret = !_showCaret;
                if (_focused) Invalidate();
            };
            _caretTimer.Start();
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

        [Category("BChat")]
        public int MaxLength
        {
            get => _maxLength;
            set => _maxLength = Math.Max(0, value);
        }

        [Category("BChat")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("BChat")]
        public Color BackColorEx
        {
            get => _backColorEx;
            set { _backColorEx = value; Invalidate(); }
        }

        [Category("BChat")]
        public Color FocusBorderColor
        {
            get => _focusBorderColor;
            set { _focusBorderColor = value; Invalidate(); }
        }

        [Category("BChat")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("BChat")]
        public int TextPadding
        {
            get => _padding;
            set { _padding = Math.Max(0, value); Invalidate(); }
        }

        [Category("BChat")]
        public bool UsePasswordChar
        {
            get => _usePasswordChar;
            set { _usePasswordChar = value; Invalidate(); }
        }

        [Category("BChat")]
        public TextDirection Direction
        {
            get => _direction;
            set { _direction = value; Invalidate(); }
        }

        // ─── RTL / LTR ───────────────────────────────────────
        private bool IsRTL()
        {
            if (_direction == TextDirection.RTL) return true;
            if (_direction == TextDirection.LTR) return false;

            if (string.IsNullOrEmpty(_textValue))
                return RightToLeft == RightToLeft.Yes;

            return char.GetUnicodeCategory(_textValue[0]) == UnicodeCategory.OtherLetter;
        }

        // ─── Focus ────────────────────────────────────────────
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
            _selectAll = false;
            _caretTimer.Stop();
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            _selectAll = false;
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Cursor = Cursors.IBeam;
            base.OnMouseEnter(e);
        }

        // ─── Keyboard ─────────────────────────────────────────
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.Handled) return;

            if (_selectAll)
            {
                _textValue = "";
                _caretIndex = 0;
                _selectAll = false;
            }

            if (e.KeyChar == '\b')
            {
                if (_caretIndex > 0)
                {
                    _textValue = _textValue.Remove(_caretIndex - 1, 1);
                    _caretIndex--;
                }
            }
            else if (!char.IsControl(e.KeyChar))
            {
                if (_textValue.Length >= _maxLength) return;

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

            if (e.Control && e.KeyCode == Keys.A)
            {
                _selectAll = true;
                Invalidate();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                if (!string.IsNullOrEmpty(_textValue))
                    Clipboard.SetText(_textValue);

                e.SuppressKeyPress = true;
            }
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
            else if (e.Control && e.KeyCode == Keys.V)
            {
                string clip = Clipboard.GetText();

                if (!string.IsNullOrEmpty(clip))
                {
                    int available = _maxLength - _textValue.Length;
                    if (available <= 0) return;

                    if (clip.Length > available)
                        clip = clip[..available];

                    _textValue = _textValue.Insert(_caretIndex, clip);
                    _caretIndex += clip.Length;
                    Invalidate();
                }

                e.SuppressKeyPress = true;
            }
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
            else if (e.KeyCode == Keys.Delete)
            {
                if (_caretIndex < _textValue.Length)
                    _textValue = _textValue.Remove(_caretIndex, 1);

                _selectAll = false;
                Invalidate();
            }
        }

        // ─── Paint ────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // الخلفية بدون كسر الكنترولات
            g.Clear(Parent?.BackColor ?? Color.WhiteSmoke);

            bool isRTL = IsRTL();

            // ── Label ─────────────────────────────────────────
            if (!string.IsNullOrEmpty(_labelText))
            {
                using var brush = new SolidBrush(_labelColor);
                var rect = new RectangleF(0, 0, Width, _labelHeight);

                using var font = new Font("Cairo", 9.5f);
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(_labelText, font, brush, rect, format);
            }

            int top = string.IsNullOrEmpty(_labelText) ? 0 : _labelHeight;
            int h = Height - top;

            var box = new Rectangle(0, top, Width - 1, h - 1);
            using var path = RoundedRect(box, _borderRadius);

            using var bg = new SolidBrush(_backColorEx);
            g.FillPath(bg, path);

            var border = _focused ? _focusBorderColor : _borderColor;
            using var pen = new Pen(border, _focused ? 1.8f : 1f);
            g.DrawPath(pen, path);

            string text = string.IsNullOrEmpty(_textValue)
                ? _placeholderText
                : (_usePasswordChar ? new string('●', _textValue.Length) : _textValue);

            Color color = string.IsNullOrEmpty(_textValue)
                ? _placeholderColor
                : _textColor;

            float textW = g.MeasureString(text, Font).Width;

            float x = isRTL ? Width - textW - _padding : _padding;
            float y = top + (h - Font.Height) / 2f;

            if (_selectAll && !string.IsNullOrEmpty(_textValue))
            {
                float selW = g.MeasureString(_textValue, Font).Width;

                float selX = isRTL ? Width - selW - _padding : _padding;

                using var selBrush = new SolidBrush(_selectionColor);
                g.FillRectangle(selBrush, new RectangleF(selX, y, selW, Font.Height));
            }

            using var textBrush = new SolidBrush(color);
            g.DrawString(text, Font, textBrush, new PointF(x, y));

            // ── Caret ───────────────────────────────────────
            if (_focused && !_selectAll && _showCaret)
            {
                string before = _textValue[.._caretIndex];
                float offset = g.MeasureString(before, Font).Width;

                float caretX = isRTL
                    ? Width - _padding - offset
                    : _padding + offset;

                using var caretPen = new Pen(_focusBorderColor, 1.5f);
                g.DrawLine(caretPen, caretX, y, caretX, y + Font.Height);
            }
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
}