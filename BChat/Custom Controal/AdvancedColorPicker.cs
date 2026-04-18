using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat
{
    [DefaultEvent("ColorChanged")]
    public class AdvancedColorPicker : UserControl
    {
        #region Fields

        private Color _color = Color.FromArgb(255, 66, 133, 244);

        private float _h = 210f;
        private float _s = 0.7f;
        private float _v = 0.96f;

        private Rectangle _wheelRect, _valueRect, _alphaRect, _previewRect;

        private bool _dragWheel, _dragValue, _dragAlpha;

        private Bitmap _wheelBitmap;
        private bool _wheelDirty = true;

        private TextBox _hexBox;
        private NumericUpDown _rBox, _gBox, _bBox, _aBox;
        private Button _copyBtn;

        private bool _updating;
        private bool _initialized;

        #endregion

        #region Designer Safe

        private bool IsInDesignMode =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        #endregion

        #region Events

        public event EventHandler ColorChanged;

        protected virtual void OnColorChanged()
        {
            if (!IsInDesignMode)
                ColorChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        [Category("Appearance")]
        public Color SelectedColor
        {
            get => _color;
            set
            {
                if (_color == value) return;

                _color = value;
                RgbToHsv(_color, out _h, out _s, out _v);

                if (_initialized)
                    UpdateUI();

                Invalidate();
                OnColorChanged();
            }
        }
        public string Hex
        {
            get => $"{_color.R:X2}{_color.G:X2}{_color.B:X2}";
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                try
                {
                    var hex = value.Trim().TrimStart('#');

                    if (hex.Length == 6)
                    {
                        int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                        int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                        int b = Convert.ToInt32(hex.Substring(4, 2), 16);

                        SelectedColor = Color.FromArgb(255, r, g, b);
                    }
                }
                catch
                {
                    // تجاهل الخطأ
                }
            }
        }

        [Category("Behavior")]
        public bool ShowAlpha { get; set; } = true;

        [Category("Behavior")]
        public bool ShowRgb { get; set; } = true;

        [Category("Behavior")]
        public bool ShowHex { get; set; } = true;

        #endregion

        #region Constructor

        public AdvancedColorPicker()
        {
            DoubleBuffered = true;
            Size = new Size(320, 260);

            if (!IsInDesignMode)
                InitControls();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!_initialized && !IsInDesignMode)
            {
                InitControls();
            }
        }

        private void InitControls()
        {
            if (_initialized) return;

            _hexBox = new TextBox { Width = 130 };
            _hexBox.Leave += (s, e) => ApplyHex();

            _rBox = CreateBox();
            _gBox = CreateBox();
            _bBox = CreateBox();
            _aBox = CreateBox();

            _rBox.ValueChanged += RgbChanged;
            _gBox.ValueChanged += RgbChanged;
            _bBox.ValueChanged += RgbChanged;
            _aBox.ValueChanged += RgbChanged;

            _copyBtn = new Button
            {
                Text = "Copy",
                Width = 50
            };
            _copyBtn.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_hexBox.Text))
                    Clipboard.SetText(_hexBox.Text);
            };

            Controls.AddRange(new Control[]
            {
                _hexBox,_rBox,_gBox,_bBox,_aBox,_copyBtn
            });

            _initialized = true;
            UpdateUI();
        }

        private NumericUpDown CreateBox()
        {
            return new NumericUpDown
            {
                Minimum = 0,
                Maximum = 255,
                Width = 50
            };
        }

        #endregion

        #region Layout

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _wheelRect = new Rectangle(10, 10, 160, 160);
            _valueRect = new Rectangle(180, 10, 20, 160);
            _alphaRect = new Rectangle(210, 10, 20, 160);
            _previewRect = new Rectangle(240, 10, 60, 60);

            if (!_initialized) return;

            int top = 180;

            _rBox.Location = new Point(10, top);
            _gBox.Location = new Point(65, top);
            _bBox.Location = new Point(120, top);
            _aBox.Location = new Point(175, top);

            _hexBox.Location = new Point(10, top + 30);
            _copyBtn.Location = new Point(150, top + 28);

            _wheelDirty = true;
        }

        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (IsInDesignMode)
            {
                e.Graphics.DrawString("Advanced Color Picker", Font, Brushes.Gray, 10, 10);
                return;
            }

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawWheel(g);
            DrawValueBar(g);
            if (ShowAlpha) DrawAlphaBar(g);
            DrawPreview(g);
        }

        private void DrawWheel(Graphics g)
        {
            if (_wheelBitmap == null || _wheelDirty)
            {
                _wheelBitmap?.Dispose();
                _wheelBitmap = RenderWheel(_wheelRect.Size);
                _wheelDirty = false;
            }

            g.DrawImage(_wheelBitmap, _wheelRect);
        }

        private void DrawValueBar(Graphics g)
        {
            using var lgb = new LinearGradientBrush(
                _valueRect,
                ColorFromHsv(_h, _s, 1),
                ColorFromHsv(_h, _s, 0),
                LinearGradientMode.Vertical);

            g.FillRectangle(lgb, _valueRect);
        }

        private void DrawAlphaBar(Graphics g)
        {
            DrawChecker(g, _alphaRect);

            using var lgb = new LinearGradientBrush(
                _alphaRect,
                Color.FromArgb(255, _color),
                Color.FromArgb(0, _color),
                LinearGradientMode.Vertical);

            g.FillRectangle(lgb, _alphaRect);
        }

        private void DrawPreview(Graphics g)
        {
            DrawChecker(g, _previewRect);

            using var b = new SolidBrush(_color);
            g.FillRectangle(b, _previewRect);
        }

        #endregion

        #region Mouse

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (IsInDesignMode) return;

            if (_wheelRect.Contains(e.Location))
            {
                _dragWheel = true;
                UpdateWheel(e.Location);
            }
            else if (_valueRect.Contains(e.Location))
            {
                _dragValue = true;
                UpdateValue(e.Location);
            }
            else if (_alphaRect.Contains(e.Location))
            {
                _dragAlpha = true;
                UpdateAlpha(e.Location);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsInDesignMode) return;

            if (_dragWheel) UpdateWheel(e.Location);
            if (_dragValue) UpdateValue(e.Location);
            if (_dragAlpha) UpdateAlpha(e.Location);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _dragWheel = _dragValue = _dragAlpha = false;
        }

        #endregion

        #region Update Logic

        private void UpdateWheel(Point p)
        {
            var center = new PointF(
                _wheelRect.X + _wheelRect.Width / 2f,
                _wheelRect.Y + _wheelRect.Height / 2f
            );

            float dx = p.X - center.X;
            float dy = center.Y - p.Y;

            float angle = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
            if (angle < 0) angle += 360;

            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            float radius = _wheelRect.Width / 2f;

            float s = dist / radius;

            // 🔥 Clamp مهم جدًا
            s = Math.Max(0f, Math.Min(1f, s));

            _h = angle;
            _s = s; // ✅ الصحيح

            UpdateColor();
        }

        private void UpdateValue(Point p)
        {
            float t = (float)(p.Y - _valueRect.Y) / _valueRect.Height;
            t = Math.Max(0f, Math.Min(1f, t)); // 🔥 مهم

            _v = 1 - t; UpdateColor();
        }

        private void UpdateAlpha(Point p)
        {
            float t = (float)(p.Y - _alphaRect.Y) / _alphaRect.Height;

            // 🔥 Clamp بين 0 و 1
            t = Math.Max(0f, Math.Min(1f, t));

            int a = (int)((1f - t) * 255f);

            // 🔥 حماية إضافية (Safety)
            a = Math.Max(0, Math.Min(255, a));

            _color = Color.FromArgb(a, _color.R, _color.G, _color.B);

            UpdateUI();
        }

        private void UpdateColor()
        {
            _color = ColorFromHsv(_h, _s, _v);
            _color = Color.FromArgb(_color.A, _color);

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (!_initialized || _updating) return;

            _updating = true;

            _rBox.Value = _color.R;
            _gBox.Value = _color.G;
            _bBox.Value = _color.B;
            _aBox.Value = _color.A;

            _hexBox.Text = $"#{_color.R:X2}{_color.G:X2}{_color.B:X2}";

            Invalidate();
            OnColorChanged();

            _updating = false;
        }

        private void RgbChanged(object sender, EventArgs e)
        {
            if (_updating) return;

            _color = Color.FromArgb(
                (int)_aBox.Value,
                (int)_rBox.Value,
                (int)_gBox.Value,
                (int)_bBox.Value
            );

            RgbToHsv(_color, out _h, out _s, out _v);

            UpdateUI();
        }

        private void ApplyHex()
        {
            try
            {
                var hex = _hexBox.Text.Trim().TrimStart('#');

                if (hex.Length == 6)
                {
                    int r = Convert.ToInt32(hex.Substring(2, 2), 16);
                    int g = Convert.ToInt32(hex.Substring(4, 2), 16);
                    int b = Convert.ToInt32(hex.Substring(6, 2), 16);

                    SelectedColor = Color.FromArgb(r, g, b);
                }
            }
            catch
            {
                UpdateUI();
            }
        }

        #endregion

        #region Helpers

        private Bitmap RenderWheel(Size size)
        {
            Bitmap bmp = new Bitmap(size.Width, size.Height);

            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    float dx = x - size.Width / 2f;
                    float dy = size.Height / 2f - y;

                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    float radius = size.Width / 2f;

                    if (dist > radius) continue;

                    float s = dist / radius;
                    float angle = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
                    if (angle < 0) angle += 360;

                    bmp.SetPixel(x, y, ColorFromHsv(angle, s, 1));
                }
            }

            return bmp;
        }

        private static void DrawChecker(Graphics g, Rectangle rect)
        {
            for (int y = 0; y < rect.Height; y += 6)
            {
                for (int x = 0; x < rect.Width; x += 6)
                {
                    bool alt = ((x + y) / 6) % 2 == 0;
                    g.FillRectangle(alt ? Brushes.LightGray : Brushes.White,
                        rect.X + x, rect.Y + y, 6, 6);
                }
            }
        }

        private static Color ColorFromHsv(float h, float s, float v)
        {
            int hi = (int)(h / 60) % 6;
            float f = h / 60 - (int)(h / 60);

            v *= 255;
            int vi = (int)v;
            int p = (int)(v * (1 - s));
            int q = (int)(v * (1 - f * s));
            int t = (int)(v * (1 - (1 - f) * s));

            return hi switch
            {
                0 => Color.FromArgb(255, vi, t, p),
                1 => Color.FromArgb(255, q, vi, p),
                2 => Color.FromArgb(255, p, vi, t),
                3 => Color.FromArgb(255, p, q, vi),
                4 => Color.FromArgb(255, t, p, vi),
                _ => Color.FromArgb(255, vi, p, q),
            };
        }

        private static void RgbToHsv(Color c, out float h, out float s, out float v)
        {
            float r = c.R / 255f;
            float g = c.G / 255f;
            float b = c.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));

            float d = max - min;

            h = 0;
            if (d != 0)
            {
                if (max == r)
                    h = 60 * (((g - b) / d) % 6);
                else if (max == g)
                    h = 60 * ((b - r) / d + 2);
                else
                    h = 60 * ((r - g) / d + 4);
            }

            if (h < 0) h += 360;

            s = max == 0 ? 0 : d / max;
            v = max;
        }

        #endregion
    }
}