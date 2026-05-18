using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BChat.Controls
{
    public enum TextDirection { Auto, RTL, LTR }

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

        // ─── Blur Fields ──────────────────────────────────────
        private bool _useBlur = false;
        private int _blurRadius = 10;
        private Bitmap? _blurCache;
        private Size _blurCacheSize = Size.Empty;
        private int _blurCacheRadius = -1;

        private readonly Timer _caretTimer;

        // ─── Constructor ──────────────────────────────────────
        public ModernTextBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.Selectable, true);

            DoubleBuffered = true;
            TabStop = true;
            BackColor = Color.Transparent; // ✅ مهم للشفافية
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

        // ════════════════════════════════════════════════════
        //  Properties
        // ════════════════════════════════════════════════════

        [Category("BChat")]
        public string LabelText
        { get => _labelText; set { _labelText = value; Invalidate(); } }

        [Category("BChat")]
        public string PlaceholderText
        { get => _placeholderText; set { _placeholderText = value; Invalidate(); } }

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
        { get => _maxLength; set => _maxLength = Math.Max(0, value); }

        [Category("BChat")]
        public int BorderRadius
        { get => _borderRadius; set { _borderRadius = Math.Max(0, value); InvalidateBlurCache(); Invalidate(); } }

        [Category("BChat")]
        [Description("لون خلفية الـ TextBox — يدعم الـ Alpha مثل Color.FromArgb(40, 255, 255, 255)")]
        public Color BackColorEx
        { get => _backColorEx; set { _backColorEx = value; Invalidate(); } }

        [Category("BChat")]
        public Color FocusBorderColor
        { get => _focusBorderColor; set { _focusBorderColor = value; Invalidate(); } }

        [Category("BChat")]
        public Color BorderColor
        { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        [Category("BChat")]
        public int TextPadding
        { get => _padding; set { _padding = Math.Max(0, value); Invalidate(); } }

        [Category("BChat")]
        public bool UsePasswordChar
        { get => _usePasswordChar; set { _usePasswordChar = value; Invalidate(); } }

        [Category("BChat")]
        public TextDirection Direction
        { get => _direction; set { _direction = value; Invalidate(); } }

        // ─── Glass / Blur ─────────────────────────────────────
        [Category("BChat - Glass")]
        [DefaultValue(false)]
        [Description("تفعيل الـ Blur خلف الـ TextBox (Glassmorphism)")]
        public bool UseBlur
        { get => _useBlur; set { _useBlur = value; InvalidateBlurCache(); Invalidate(); } }

        [Category("BChat - Glass")]
        [DefaultValue(10)]
        [Description("شدة الضبابية: 1 = خفيف  |  40 = كثيف")]
        public int BlurRadius
        { get => _blurRadius; set { _blurRadius = Math.Clamp(value, 1, 40); InvalidateBlurCache(); Invalidate(); } }

        // ════════════════════════════════════════════════════
        //  Suppress default background — نمنع الـ Control
        //  من رسم خلفية معتمة تكسر الشفافية
        // ════════════════════════════════════════════════════
        protected override void OnPaintBackground(PaintEventArgs e) { }

        // ════════════════════════════════════════════════════
        //  OnPaint
        // ════════════════════════════════════════════════════
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            bool isRTL = IsRTL();

            int top = string.IsNullOrEmpty(_labelText) ? 0 : _labelHeight;
            int h = Height - top;
            var box = new Rectangle(0, top, Width - 1, h - 1);

            using var path = RoundedRect(box, _borderRadius);

            // ── 1: خلفية الـ Parent الحقيقية (يُمكّن الشفافية) ──
            PaintSceneBackground(g);

            // ── 2: Blur داخل الـ Path (اختياري) ─────────────────
            if (_useBlur)
                DrawBlurredBackground(g, path, box);

            // ── 3: Label ──────────────────────────────────────────
            if (!string.IsNullOrEmpty(_labelText))
            {
                using var labelBrush = new SolidBrush(_labelColor);
                using var labelFont = new Font("Cairo", 9.5f);
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(_labelText, labelFont, labelBrush,
                    new RectangleF(0, 0, Width, _labelHeight), format);
            }

            // ── 4: خلفية الـ Box (تدعم Alpha) ────────────────────
            using (var bg = new SolidBrush(_backColorEx))
                g.FillPath(bg, path);

            // ── 5: البوردر ────────────────────────────────────────
            var borderClr = _focused ? _focusBorderColor : _borderColor;
            using (var pen = new Pen(borderClr, _focused ? 1.8f : 1f))
                g.DrawPath(pen, path);

            // ── 6: النص ───────────────────────────────────────────
            string display = string.IsNullOrEmpty(_textValue)
                ? _placeholderText
                : (_usePasswordChar ? new string('●', _textValue.Length) : _textValue);

            Color textClr = string.IsNullOrEmpty(_textValue)
                ? _placeholderColor : _textColor;

            float textW = g.MeasureString(display, Font).Width;
            float x = isRTL ? Width - textW - _padding : _padding;
            float y = top + (h - Font.Height) / 2f;

            // ── 7: Selection ──────────────────────────────────────
            if (_selectAll && !string.IsNullOrEmpty(_textValue))
            {
                float selW = g.MeasureString(_textValue, Font).Width;
                float selX = isRTL ? Width - selW - _padding : _padding;
                using var selBrush = new SolidBrush(_selectionColor);
                g.FillRectangle(selBrush, new RectangleF(selX, y, selW, Font.Height));
            }

            using (var textBrush = new SolidBrush(textClr))
                g.DrawString(display, Font, textBrush, new PointF(x, y));

            // ── 8: Caret ──────────────────────────────────────────
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

        // ════════════════════════════════════════════════════
        //  Glassmorphism Blur
        // ════════════════════════════════════════════════════

        private void DrawBlurredBackground(Graphics g, GraphicsPath clipPath, Rectangle box)
        {
            if (Parent == null) return;

            if (_blurCache == null ||
                _blurCacheSize != Size ||
                _blurCacheRadius != _blurRadius)
            {
                _blurCache?.Dispose();
                _blurCache = BuildBlurredBitmap();
                _blurCacheSize = Size;
                _blurCacheRadius = _blurRadius;
            }

            // TextureBrush + FillPath = حواف ناعمة بدون artifacts
            using var tb = new TextureBrush(_blurCache);
            g.FillPath(tb, clipPath);
        }

        private Bitmap BuildBlurredBitmap()
        {
            const float scale = 0.25f;
            int smallW = Math.Max(1, (int)(Width * scale));
            int smallH = Math.Max(1, (int)(Height * scale));

            using var full = CaptureSceneBehind();

            using var small = new Bitmap(smallW, smallH, PixelFormat.Format32bppArgb);
            using (var sg = Graphics.FromImage(small))
            {
                sg.InterpolationMode = InterpolationMode.Bilinear;
                sg.DrawImage(full, 0, 0, smallW, smallH);
            }

            int smallRadius = Math.Max(1, (int)(_blurRadius * scale));
            using var blurredSmall = GaussianBlur(small, smallRadius);

            var result = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using (var rg = Graphics.FromImage(result))
            {
                rg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                rg.DrawImage(blurredSmall, 0, 0, Width, Height);
            }
            return result;
        }

        // ════════════════════════════════════════════════════
        //  Scene Background (Parent + Siblings)
        // ════════════════════════════════════════════════════

        /// <summary>يرسم المشهد الكامل خلفنا مباشرة على الـ Graphics</summary>
        private void PaintSceneBackground(Graphics g)
        {
            if (Parent == null) return;
            var state = g.Save();
            g.TranslateTransform(-Left, -Top);

            using (var pe = new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height)))
            {
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);
            }

            DrawSiblingsBehind(g);
            g.Restore(state);
        }

        /// <summary>يلتقط المشهد الكامل خلفنا في Bitmap</summary>
        private Bitmap CaptureSceneBehind()
        {
            var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bmp);
            g.TranslateTransform(-Left, -Top);

            using (var pe = new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height)))
            {
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);
            }

            DrawSiblingsBehind(g);
            return bmp;
        }

        /// <summary>يرسم الـ Siblings التي خلفنا في الـ Z-Order</summary>
        private void DrawSiblingsBehind(Graphics g)
        {
            if (Parent == null) return;
            int myIndex = Parent.Controls.GetChildIndex(this);
            var myBounds = new Rectangle(Left, Top, Width, Height);

            for (int i = Parent.Controls.Count - 1; i > myIndex; i--)
            {
                var sib = Parent.Controls[i];
                if (!sib.Visible) continue;
                if (!sib.Bounds.IntersectsWith(myBounds)) continue;

                var st = g.Save();
                g.TranslateTransform(sib.Left, sib.Top);
                using var pe = new PaintEventArgs(g, new Rectangle(0, 0, sib.Width, sib.Height));
                InvokePaintBackground(sib, pe);
                InvokePaint(sib, pe);
                g.Restore(st);
            }
        }

        // ════════════════════════════════════════════════════
        //  Blur Cache Management
        // ════════════════════════════════════════════════════
        private void InvalidateBlurCache()
        {
            _blurCache?.Dispose();
            _blurCache = null;
            _blurCacheSize = Size.Empty;
            _blurCacheRadius = -1;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InvalidateBlurCache();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                InvalidateBlurCache();
                _caretTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        // ════════════════════════════════════════════════════
        //  Gaussian Blur (3 × Box Blur H+V)
        // ════════════════════════════════════════════════════
        private static Bitmap GaussianBlur(Bitmap source, int radius)
        {
            Bitmap a = BoxBlurH(source, radius);
            Bitmap b = BoxBlurV(a, radius); a.Dispose();
            Bitmap c = BoxBlurH(b, radius); b.Dispose();
            Bitmap d = BoxBlurV(c, radius); c.Dispose();
            Bitmap e = BoxBlurH(d, radius); d.Dispose();
            Bitmap f = BoxBlurV(e, radius); e.Dispose();
            return f;
        }

        private static unsafe Bitmap BoxBlurH(Bitmap src, int radius)
        {
            int w = src.Width, h = src.Height;
            var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            var srcData = src.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var dstData = dst.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int kernel = radius * 2 + 1;

            for (int y = 0; y < h; y++)
            {
                byte* sRow = (byte*)srcData.Scan0 + y * srcData.Stride;
                byte* dRow = (byte*)dstData.Scan0 + y * dstData.Stride;
                long b = 0, gr = 0, r = 0, a = 0;

                for (int kx = -radius; kx <= radius; kx++)
                { int px = Math.Clamp(kx, 0, w - 1) * 4; b += sRow[px]; gr += sRow[px + 1]; r += sRow[px + 2]; a += sRow[px + 3]; }

                for (int x = 0; x < w; x++)
                {
                    dRow[x * 4] = (byte)(b / kernel); dRow[x * 4 + 1] = (byte)(gr / kernel);
                    dRow[x * 4 + 2] = (byte)(r / kernel); dRow[x * 4 + 3] = (byte)(a / kernel);
                    int ap = Math.Clamp(x + radius + 1, 0, w - 1) * 4, rp = Math.Clamp(x - radius, 0, w - 1) * 4;
                    b += sRow[ap] - sRow[rp]; gr += sRow[ap + 1] - sRow[rp + 1];
                    r += sRow[ap + 2] - sRow[rp + 2]; a += sRow[ap + 3] - sRow[rp + 3];
                }
            }
            src.UnlockBits(srcData); dst.UnlockBits(dstData);
            return dst;
        }

        private static unsafe Bitmap BoxBlurV(Bitmap src, int radius)
        {
            int w = src.Width, h = src.Height;
            var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            var srcData = src.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var dstData = dst.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int stride = srcData.Stride, kernel = radius * 2 + 1;

            for (int x = 0; x < w; x++)
            {
                long b = 0, gr = 0, r = 0, a = 0;
                for (int ky = -radius; ky <= radius; ky++)
                { byte* p = (byte*)srcData.Scan0 + Math.Clamp(ky, 0, h - 1) * stride + x * 4; b += p[0]; gr += p[1]; r += p[2]; a += p[3]; }

                for (int y = 0; y < h; y++)
                {
                    byte* dp = (byte*)dstData.Scan0 + y * stride + x * 4;
                    dp[0] = (byte)(b / kernel); dp[1] = (byte)(gr / kernel);
                    dp[2] = (byte)(r / kernel); dp[3] = (byte)(a / kernel);
                    byte* ap = (byte*)srcData.Scan0 + Math.Clamp(y + radius + 1, 0, h - 1) * stride + x * 4;
                    byte* rp = (byte*)srcData.Scan0 + Math.Clamp(y - radius, 0, h - 1) * stride + x * 4;
                    b += ap[0] - rp[0]; gr += ap[1] - rp[1]; r += ap[2] - rp[2]; a += ap[3] - rp[3];
                }
            }
            src.UnlockBits(srcData); dst.UnlockBits(dstData);
            return dst;
        }

        // ════════════════════════════════════════════════════
        //  Focus / Mouse / Keyboard
        // ════════════════════════════════════════════════════
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
        { Focus(); _selectAll = false; base.OnMouseDown(e); }

        protected override void OnMouseEnter(EventArgs e)
        { Cursor = Cursors.IBeam; base.OnMouseEnter(e); }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.Handled) return;

            if (_selectAll) { _textValue = ""; _caretIndex = 0; _selectAll = false; }

            if (e.KeyChar == '\b')
            { if (_caretIndex > 0) { _textValue = _textValue.Remove(_caretIndex - 1, 1); _caretIndex--; } }
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
            { _selectAll = true; Invalidate(); e.SuppressKeyPress = true; }
            else if (e.Control && e.KeyCode == Keys.C)
            { if (!string.IsNullOrEmpty(_textValue)) Clipboard.SetText(_textValue); e.SuppressKeyPress = true; }
            else if (e.Control && e.KeyCode == Keys.X)
            {
                if (!string.IsNullOrEmpty(_textValue))
                { Clipboard.SetText(_textValue); _textValue = ""; _caretIndex = 0; Invalidate(); }
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                string clip = Clipboard.GetText();
                if (!string.IsNullOrEmpty(clip))
                {
                    int avail = _maxLength - _textValue.Length;
                    if (avail <= 0) return;
                    if (clip.Length > avail) clip = clip[..avail];
                    _textValue = _textValue.Insert(_caretIndex, clip);
                    _caretIndex += clip.Length;
                    Invalidate();
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Left)
            { if (_caretIndex > 0) _caretIndex--; _selectAll = false; Invalidate(); }
            else if (e.KeyCode == Keys.Right)
            { if (_caretIndex < _textValue.Length) _caretIndex++; _selectAll = false; Invalidate(); }
            else if (e.KeyCode == Keys.Delete)
            { if (_caretIndex < _textValue.Length) _textValue = _textValue.Remove(_caretIndex, 1); _selectAll = false; Invalidate(); }
        }

        // ════════════════════════════════════════════════════
        //  RTL Helper
        // ════════════════════════════════════════════════════
        private bool IsRTL()
        {
            if (_direction == TextDirection.RTL) return true;
            if (_direction == TextDirection.LTR) return false;
            if (string.IsNullOrEmpty(_textValue))
                return RightToLeft == RightToLeft.Yes;
            return char.GetUnicodeCategory(_textValue[0]) == UnicodeCategory.OtherLetter;
        }

        // ════════════════════════════════════════════════════
        //  Rounded Rect Helper
        // ════════════════════════════════════════════════════
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