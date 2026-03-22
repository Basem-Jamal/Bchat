// Controls/ModernTextBox.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BChat.Controls
{
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
        private Color _backColor = Color.FromArgb(237, 235, 255);
        private Color _borderColor = Color.FromArgb(220, 215, 250);
        private Color _focusBorderColor = Color.FromArgb(124, 111, 247);
        private Color _labelColor = Color.FromArgb(60, 60, 90);
        private Color _selectionColor = Color.FromArgb(80, 124, 111, 247);

        private int _borderRadius = 14;
        private int _borderThickness = 1;
        private int _labelHeight = 24;
        private int _caretIndex = 0;
        private bool _focused = false;
        private bool _showCaret = true;
        private bool _selectAll = false;
        private int _maxLength = 32767;

        private readonly Timer _caretTimer;

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
            set { _maxLength = Math.Max(0, value); }
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
            get => _backColor;
            set { _backColor = value; Invalidate(); }
        }

        [Category("BChat")]
        public Color FocusBorderColor
        {
            get => _focusBorderColor;
            set { _focusBorderColor = value; Invalidate(); }
        }

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
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            _caretTimer = new Timer { Interval = 500 };
            _caretTimer.Tick += (s, e) =>
            {
                _showCaret = !_showCaret;
                if (_focused) Invalidate();
            };
            _caretTimer.Start();
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
                if (_maxLength > 0 && _textValue.Length >= _maxLength) return;
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
                _selectAll = true; Invalidate();
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
                    _textValue = ""; _caretIndex = 0; Invalidate();
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
                    if (clip.Length > available) clip = clip[..available];
                    _textValue = _textValue.Insert(_caretIndex, clip);
                    _caretIndex += clip.Length;
                    Invalidate();
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Left)
            {
                if (_caretIndex > 0) _caretIndex--;
                _selectAll = false; Invalidate();
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (_caretIndex < _textValue.Length) _caretIndex++;
                _selectAll = false; Invalidate();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (_caretIndex < _textValue.Length)
                    _textValue = _textValue.Remove(_caretIndex, 1);
                _selectAll = false; Invalidate();
            }
        }

        // ─── Mouse ────────────────────────────────────────────
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            _selectAll = false;
            base.OnMouseDown(e);
        }

        // ─── Paint ────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? Color.WhiteSmoke);

            // ── 1. رسم الـ Label ─────────────────────────────
            if (!string.IsNullOrEmpty(_labelText))
            {
                using var labelBrush = new SolidBrush(_labelColor);
                var labelRect = new RectangleF(0, 0, Width, _labelHeight);
                var labelFormat = new StringFormat
                {
                    Alignment = StringAlignment.Far,   // يمين لـ RTL
                    LineAlignment = StringAlignment.Center
                };
                using var labelFont = new Font("Cairo", 9.5f, FontStyle.Regular);
                g.DrawString(_labelText, labelFont, labelBrush, labelRect, labelFormat);
            }

            // ── 2. رسم الـ Input Box ──────────────────────────
            int boxTop = string.IsNullOrEmpty(_labelText) ? 0 : _labelHeight;
            int boxH = Height - boxTop;
            var boxRect = new Rectangle(0, boxTop, Width - 1, boxH - 1);

            using var path = RoundedRect(boxRect, _borderRadius);

            // خلفية
            using var bgBrush = new SolidBrush(_backColor);
            g.FillPath(bgBrush, path);

            // Border — يتغير لون عند الـ Focus
            var borderCol = _focused ? _focusBorderColor : _borderColor;
            float borderW = _focused ? 1.8f : 1f;
            using var borderPen = new Pen(borderCol, borderW);
            g.DrawPath(borderPen, path);

            // ── 3. النص والـ Placeholder ──────────────────────
            string drawText = string.IsNullOrEmpty(_textValue) ? _placeholderText : _textValue;
            Color drawColor = string.IsNullOrEmpty(_textValue) ? _placeholderColor : _textColor;

            float textY = boxTop + (boxH - Font.Height) / 2f;
            float textWidth = g.MeasureString(drawText, Font).Width;

            // RTL → النص يبدأ من اليمين
            float textX = Width - textWidth - 14f;

            // تحديد الكل
            if (_selectAll && !string.IsNullOrEmpty(_textValue))
            {
                float selW = g.MeasureString(_textValue, Font).Width;
                using var selBrush = new SolidBrush(_selectionColor);
                g.FillRectangle(selBrush, new RectangleF(textX, textY, selW, Font.Height));
            }

            using var textBrush = new SolidBrush(drawColor);
            g.DrawString(drawText, Font, textBrush, new PointF(textX, textY));

            // ── 4. الـ Caret ──────────────────────────────────
            if (_focused && !_selectAll && _showCaret)
            {
                // RTL → Caret يبدأ من اليمين ويتحرك يسار
                string beforeCaret = _textValue[.._caretIndex];
                float caretOffset = g.MeasureString(beforeCaret, Font).Width;
                float caretX = Width - 14f - caretOffset;

                using var caretPen = new Pen(Color.FromArgb(124, 111, 247), 1.5f);
                g.DrawLine(caretPen, caretX, textY, caretX, textY + Font.Height);
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