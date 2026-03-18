using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat
{
    [ToolboxItem(true)]
    [DefaultEvent("ColorChanged")]
    public class AdvancedColorPicker : UserControl
    {
        #region Fields

        private Color _color = Color.FromArgb(255, 66, 133, 244);

        // HSV
        private float _h = 210f;
        private float _s = 0.7f;
        private float _v = 0.96f;

        // Layout
        private Rectangle _cardRect;
        private Rectangle _wheelRect;
        private Rectangle _valueRect;
        private Rectangle _previewRect;
        private Rectangle _titleRect;

        private bool _dragWheel;
        private bool _dragValue;

        private Bitmap _wheelBitmap;
        private bool _wheelDirty = true;

        private TextBox _hexBox;

        #endregion

        #region Events / Properties

        [Category("Behavior")]
        public event EventHandler ColorChanged;

        [Category("Appearance")]
        public Color SelectedColor
        {
            get => _color;
            set
            {
                if (_color == value) return;

                _color = value;
                RgbToHsv(_color, out _h, out _s, out _v);
                UpdateHexBox();
                Invalidate();
                OnColorChanged(EventArgs.Empty);
            }
        }

        [Category("Appearance")]
        [DefaultValue("Color Picker")]
        public string TitleText { get; set; } = "Color Picker";

        #endregion

        #region Constructor

        public AdvancedColorPicker()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 9f);

            _hexBox = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9f),
                Text = "#4285F4",
                MaxLength = 9, // #AARRGGBB
            };
            _hexBox.Leave += HexBox_Leave;
            _hexBox.KeyDown += HexBox_KeyDown;
            Controls.Add(_hexBox);

            Size = new Size(260, 220);
        }

        #endregion

        #region Layout

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int padding = 10;
            _cardRect = new Rectangle(padding, padding, Width - padding * 2, Height - padding * 2);

            int headerHeight = 24;
            _titleRect = new Rectangle(_cardRect.X + 12, _cardRect.Y + 8, _cardRect.Width - 24, headerHeight);

            int contentTop = _titleRect.Bottom + 8;
            int contentBottom = _cardRect.Bottom - 40;

            // نجهز مساحة العجلة والشريط
            int wheelSize = Math.Min(_cardRect.Width - 60, contentBottom - contentTop);
            wheelSize = Math.Max(80, wheelSize);

            _wheelRect = new Rectangle(
                _cardRect.X + 12,
                contentTop + ((contentBottom - contentTop - wheelSize) / 2),
                wheelSize,
                wheelSize
            );

            _valueRect = new Rectangle(
                _wheelRect.Right + 10,
                _wheelRect.Top,
                16,
                _wheelRect.Height
            );

            _previewRect = new Rectangle(
                _valueRect.Right + 10,
                _wheelRect.Top,
                34,
                34
            );

            // hex textbox في الأسفل
            _hexBox.Width = _cardRect.Width - 24;
            _hexBox.Location = new Point(
                _cardRect.X + 12,
                _cardRect.Bottom - 28
            );

            _wheelDirty = true;
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawCardBackground(g);
            DrawTitle(g);
            DrawWheel(g);
            DrawValueBar(g);
            DrawPreview(g);
        }

        private void DrawCardBackground(Graphics g)
        {
            using (GraphicsPath path = GetRoundedRect(_cardRect, 16))
            {
                // ظل خفيف
                using (PathGradientBrush shadowBrush = new PathGradientBrush(path))
                {
                    shadowBrush.CenterColor = Color.FromArgb(40, 0, 0, 0);
                    shadowBrush.SurroundColors = new[] { Color.Transparent };
                    var shadowRect = _cardRect;
                    shadowRect.Offset(0, 4);
                    g.FillPath(shadowBrush, GetRoundedRect(shadowRect, 16));
                }

                using (LinearGradientBrush bg = new LinearGradientBrush(
                           _cardRect,
                           Color.FromArgb(250, 252, 255),
                           Color.FromArgb(238, 241, 248),
                           LinearGradientMode.Vertical))
                {
                    g.FillPath(bg, path);
                }

                using (Pen border = new Pen(Color.FromArgb(210, 215, 230)))
                {
                    g.DrawPath(border, path);
                }
            }
        }

        private void DrawTitle(Graphics g)
        {
            using (SolidBrush b = new SolidBrush(Color.FromArgb(60, 60, 70)))
            {
                g.DrawString(TitleText, Font, b, _titleRect,
                    new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near });
            }
        }

        private void DrawWheel(Graphics g)
        {
            if (_wheelRect.Width <= 0 || _wheelRect.Height <= 0)
                return;

            if (_wheelBitmap == null || _wheelDirty)
            {
                _wheelBitmap?.Dispose();
                _wheelBitmap = RenderWheelBitmap(_wheelRect.Size);
                _wheelDirty = false;
            }

            g.DrawImage(_wheelBitmap, _wheelRect);

            // مؤشر / كروس صغير يدل على المكان الحالي
            var center = new PointF(_wheelRect.Left + _wheelRect.Width / 2f, _wheelRect.Top + _wheelRect.Height / 2f);
            float radius = _wheelRect.Width / 2f;

            float rad = (float)(Math.PI * _h / 180f);
            float r = _s * radius;

            float cx = center.X + (float)(Math.Cos(rad) * r);
            float cy = center.Y - (float)(Math.Sin(rad) * r); // ناقص لأن Y لأسفل

            using (Pen pOuter = new Pen(Color.Black, 1.5f))
            using (Pen pInner = new Pen(Color.White, 1.5f))
            {
                g.DrawEllipse(pOuter, cx - 5, cy - 5, 10, 10);
                g.DrawEllipse(pInner, cx - 4, cy - 4, 8, 8);
            }
        }

        private void DrawValueBar(Graphics g)
        {
            if (_valueRect.Width <= 0 || _valueRect.Height <= 0) return;

            // أعلى = V=1، أسفل = V=0
            using (LinearGradientBrush lgb = new LinearGradientBrush(
                       _valueRect,
                       ColorFromHsv(_h, _s, 1f),
                       ColorFromHsv(_h, _s, 0f),
                       LinearGradientMode.Vertical))
            {
                using (GraphicsPath path = GetRoundedRect(_valueRect, 6))
                {
                    g.FillPath(lgb, path);
                    using (Pen border = new Pen(Color.FromArgb(180, 50, 50, 60)))
                    {
                        g.DrawPath(border, path);
                    }
                }
            }

            // مؤشر القيمة
            float y = _valueRect.Top + (1f - _v) * _valueRect.Height;
            var lineRect = new RectangleF(_valueRect.Left - 2, y - 3, _valueRect.Width + 4, 6);

            using (GraphicsPath p = GetRoundedRect(lineRect, 3))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(230, 255, 255, 255)))
            using (Pen pen = new Pen(Color.FromArgb(180, 80, 80, 90)))
            {
                g.FillPath(b, p);
                g.DrawPath(pen, p);
            }
        }

        private void DrawPreview(Graphics g)
        {
            using (GraphicsPath path = GetRoundedRect(_previewRect, _previewRect.Width / 2))
            {
                // خلفية checker
                DrawChecker(g, _previewRect, 4);

                using (SolidBrush b = new SolidBrush(_color))
                {
                    g.SetClip(path);
                    g.FillRectangle(b, _previewRect);
                    g.ResetClip();
                }

                using (Pen p = new Pen(Color.FromArgb(180, 0, 0, 0)))
                {
                    g.DrawPath(p, path);
                }
            }
        }

        #endregion

        #region Mouse Handling

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left) return;

            if (_wheelRect.Contains(e.Location))
            {
                _dragWheel = true;
                UpdateWheelFromPoint(e.Location);
            }
            else if (_valueRect.Contains(e.Location))
            {
                _dragValue = true;
                UpdateValueFromPoint(e.Location);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_dragWheel)
            {
                UpdateWheelFromPoint(e.Location);
            }
            else if (_dragValue)
            {
                UpdateValueFromPoint(e.Location);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragWheel = false;
            _dragValue = false;
        }

        private void UpdateWheelFromPoint(Point p)
        {
            var center = new PointF(_wheelRect.Left + _wheelRect.Width / 2f, _wheelRect.Top + _wheelRect.Height / 2f);
            float dx = p.X - center.X;
            float dy = center.Y - p.Y; // عكس Y

            float angle = (float)(Math.Atan2(dy, dx) * 180f / Math.PI);
            if (angle < 0) angle += 360f;

            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            float radius = _wheelRect.Width / 2f;
            float s = dist / radius;
            s = Math.Max(0f, Math.Min(1f, s));

            _h = angle;
            _s = s;

            UpdateColorFromHsv();
        }

        private void UpdateValueFromPoint(Point p)
        {
            float t = (float)(p.Y - _valueRect.Top) / _valueRect.Height;
            t = Math.Max(0f, Math.Min(1f, t));
            _v = 1f - t;

            UpdateColorFromHsv();
        }

        private void UpdateColorFromHsv()
        {
            _color = ColorFromHsv(_h, _s, _v);
            UpdateHexBox();
            Invalidate();
            OnColorChanged(EventArgs.Empty);
        }

        protected virtual void OnColorChanged(EventArgs e)
        {
            ColorChanged?.Invoke(this, e);
        }

        #endregion

        #region Hex Box

        private void UpdateHexBox()
        {
            _hexBox.Text = $"#{_color.R:X2}{_color.G:X2}{_color.B:X2}";
        }

        private void HexBox_Leave(object sender, EventArgs e)
        {
            ApplyHex();
        }

        private void HexBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ApplyHex();
            }
        }

        private void ApplyHex()
        {
            string hex = _hexBox.Text.Trim().TrimStart('#');
            if (hex.Length == 6)
            {
                try
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(4, 2), 16);

                    SelectedColor = Color.FromArgb(255, r, g, b); // يمر عبر الـ property
                }
                catch
                {
                    // تجاهل الخطأ وأرجع اللون الصحيح
                    UpdateHexBox();
                }
            }
            else
            {
                UpdateHexBox();
            }
        }

        #endregion

        #region Helpers (Drawing)

        private static GraphicsPath GetRoundedRect(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0f)
            {
                path.AddRectangle(rect);
                return path;
            }

            float d = radius * 2;
            RectangleF arc = new RectangleF(rect.Location, new SizeF(d, d));

            // TL
            path.AddArc(arc, 180, 90);
            // TR
            arc.X = rect.Right - d;
            path.AddArc(arc, 270, 90);
            // BR
            arc.Y = rect.Bottom - d;
            path.AddArc(arc, 0, 90);
            // BL
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            return GetRoundedRect((RectangleF)rect, radius);
        }

        private static void DrawChecker(Graphics g, Rectangle rect, int size)
        {
            using (SolidBrush b1 = new SolidBrush(Color.FromArgb(220, 220, 220)))
            using (SolidBrush b2 = new SolidBrush(Color.White))
            {
                for (int y = rect.Top; y < rect.Bottom; y += size)
                {
                    for (int x = rect.Left; x < rect.Right; x += size)
                    {
                        bool odd = ((x / size) + (y / size)) % 2 == 0;
                        Rectangle r = new Rectangle(x, y, size, size);
                        g.FillRectangle(odd ? b1 : b2, r);
                    }
                }
            }
        }

        private Bitmap RenderWheelBitmap(Size size)
        {
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                float cx = size.Width / 2f;
                float cy = size.Height / 2f;
                float radius = Math.Min(cx, cy);

                for (int y = 0; y < size.Height; y++)
                {
                    for (int x = 0; x < size.Width; x++)
                    {
                        float dx = x - cx;
                        float dy = cy - y; // عكس Y
                        float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                        if (dist > radius) continue;

                        float s = dist / radius;
                        float angle = (float)(Math.Atan2(dy, dx) * 180f / Math.PI);
                        if (angle < 0) angle += 360f;

                        Color c = ColorFromHsv(angle, s, 1f);
                        bmp.SetPixel(x, y, c);
                    }
                }
            }
            return bmp;
        }

        #endregion

        #region Helpers (HSV/RGB)

        private static Color ColorFromHsv(float h, float s, float v)
        {
            if (s <= 0f)
            {
                int vInt = (int)(v * 255);
                return Color.FromArgb(255, vInt, vInt, vInt);
            }

            h = (h % 360f + 360f) % 360f;
            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60f) % 2 - 1));
            float m = v - c;

            float rPrime = 0, gPrime = 0, bPrime = 0;

            if (h < 60) { rPrime = c; gPrime = x; bPrime = 0; }
            else if (h < 120) { rPrime = x; gPrime = c; bPrime = 0; }
            else if (h < 180) { rPrime = 0; gPrime = c; bPrime = x; }
            else if (h < 240) { rPrime = 0; gPrime = x; bPrime = c; }
            else if (h < 300) { rPrime = x; gPrime = 0; bPrime = c; }
            else { rPrime = c; gPrime = 0; bPrime = x; }

            int r = (int)((rPrime + m) * 255);
            int g = (int)((gPrime + m) * 255);
            int b = (int)((bPrime + m) * 255);

            r = Math.Max(0, Math.Min(255, r));
            g = Math.Max(0, Math.Min(255, g));
            b = Math.Max(0, Math.Min(255, b));

            return Color.FromArgb(255, r, g, b);
        }

        private static void RgbToHsv(Color c, out float h, out float s, out float v)
        {
            float r = c.R / 255f;
            float g = c.G / 255f;
            float b = c.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            // Hue
            if (delta < 0.00001f)
            {
                h = 0;
            }
            else if (max == r)
            {
                h = 60f * (((g - b) / delta) % 6f);
            }
            else if (max == g)
            {
                h = 60f * (((b - r) / delta) + 2f);
            }
            else
            {
                h = 60f * (((r - g) / delta) + 4f);
            }

            if (h < 0) h += 360f;

            // Saturation
            s = (max <= 0) ? 0 : (delta / max);

            // Value
            v = max;
        }

        #endregion
    }
}
