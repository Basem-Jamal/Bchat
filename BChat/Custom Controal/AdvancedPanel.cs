using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat
{
    [ToolboxItem(true)]
    [DefaultProperty("BackColor")]
    public class ModernPanel : Panel
    {
        #region 🔹 Fields
        private Color _backColor = Color.White;
        private Color _borderColor = Color.FromArgb(230, 230, 230);
        private int _borderThickness = 1;
        private int _borderRadius = 20;

        // 🌫 Shadow Settings
        private bool _useShadow = true;
        private int _shadowDepth = 10;
        private int _shadowBlur = 8;
        private Color _shadowColor = Color.FromArgb(80, 0, 0, 0);
        private int _shadowOffsetX = 3;
        private int _shadowOffsetY = 3;
        private bool _shrinkContentWithShadow = true;

        // 🌈 Gradient
        private bool _useGradient = true;
        private Color _gradientColor1 = Color.White;
        private Color _gradientColor2 = Color.FromArgb(245, 247, 250);
        private LinearGradientMode _gradientMode = LinearGradientMode.Vertical;

        // ✨ Effects
        private bool _useGlass = true;
        private int _glassTransparency = 30;
        private bool _useGlow = false;
        private Color _glowColor = Color.FromArgb(100, 0, 120, 255);

        // Hover
        private bool _isHovered = false;
        private int _hoverDepthBoost = 4;
        #endregion

        #region 🔹 Constructor
        public ModernPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Size = new Size(300, 180);
            Font = new Font("Segoe UI", 9f);
            BackColor = Color.Transparent;
            ForeColor = Color.Black;

            MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
        }
        #endregion

        #region 🔹 Properties

        [Category("Appearance")]
        public new Color BackColor
        {
            get => _backColor;
            set { _backColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("Shadow")]
        public bool UseShadow
        {
            get => _useShadow;
            set { _useShadow = value; Invalidate(); }
        }

        [Category("Shadow")]
        public int ShadowDepth
        {
            get => _shadowDepth;
            set { _shadowDepth = Math.Max(1, value); Invalidate(); }
        }

        [Category("Shadow")]
        public int ShadowBlur
        {
            get => _shadowBlur;
            set { _shadowBlur = Math.Max(0, value); Invalidate(); }
        }

        [Category("Shadow")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("Shadow")]
        public int ShadowOffsetX
        {
            get => _shadowOffsetX;
            set { _shadowOffsetX = value; Invalidate(); }
        }

        [Category("Shadow")]
        public int ShadowOffsetY
        {
            get => _shadowOffsetY;
            set { _shadowOffsetY = value; Invalidate(); }
        }

        [Category("Shadow")]
        public bool ShrinkContentWithShadow
        {
            get => _shrinkContentWithShadow;
            set { _shrinkContentWithShadow = value; Invalidate(); }
        }

        [Category("Gradient")]
        public bool UseGradient
        {
            get => _useGradient;
            set { _useGradient = value; Invalidate(); }
        }

        [Category("Gradient")]
        public Color GradientColor1
        {
            get => _gradientColor1;
            set { _gradientColor1 = value; Invalidate(); }
        }

        [Category("Gradient")]
        public Color GradientColor2
        {
            get => _gradientColor2;
            set { _gradientColor2 = value; Invalidate(); }
        }

        [Category("Gradient")]
        public LinearGradientMode GradientMode
        {
            get => _gradientMode;
            set { _gradientMode = value; Invalidate(); }
        }

        [Category("Effects")]
        public bool UseGlass
        {
            get => _useGlass;
            set { _useGlass = value; Invalidate(); }
        }

        [Category("Effects")]
        public int GlassTransparency
        {
            get => _glassTransparency;
            set { _glassTransparency = Math.Max(0, Math.Min(100, value)); Invalidate(); }
        }

        [Category("Effects")]
        public bool UseGlow
        {
            get => _useGlow;
            set { _useGlow = value; Invalidate(); }
        }

        [Category("Effects")]
        public Color GlowColor
        {
            get => _glowColor;
            set { _glowColor = value; Invalidate(); }
        }

        #endregion

        #region 🎨 Drawing

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Parent?.BackColor ?? Color.White);

            if (_useShadow) DrawInsetShadow(e.Graphics);

            DrawPanelBackground(e.Graphics);
            if (_useGlass) DrawGlassEffect(e.Graphics);
            if (_useGlow) DrawGlowEffect(e.Graphics);
        }

        private int _shadowSize = 10;

        [Category("Shadow")]
        public int ShadowSize
        {
            get => _shadowSize;
            set { _shadowSize = Math.Max(0, value); Invalidate(); }
        }

        private void DrawInsetShadow(Graphics g)
        {
            if (!_useShadow) return;

            int shrink = _shrinkContentWithShadow ? _shadowDepth : 0;
            int offsetX = _shadowOffsetX;
            int offsetY = _shadowOffsetY;

            // ✅ نحسب مستطيل البانل الداخلي
            Rectangle shadowRect = new Rectangle(
                shrink + offsetX,
                shrink + offsetY,
                Width - (shrink * 2) - 1,
                Height - (shrink * 2) - 1
            );

            // ✅ منطقة الظل — عمقه يحدد بـ ShadowSize
            int shadowHeight = Math.Max(2, _shadowSize);
            Rectangle shadowArea = new Rectangle(
                shadowRect.X,
                shadowRect.Bottom - shadowHeight,
                shadowRect.Width,
                shadowHeight
            );

            using (GraphicsPath shadowPath = CreateRoundedPath(shadowRect, _borderRadius))
            using (LinearGradientBrush shadowBrush = new LinearGradientBrush(
                shadowArea,
                Color.FromArgb(_shadowColor.A, _shadowColor), // داكن في الأسفل
                Color.FromArgb(0, _shadowColor),               // يتلاشى للأعلى
                LinearGradientMode.Vertical))
            {
                // نملأ فقط الجزء السفلي لإظهار الظل بأسلوب أنيق وحديث
                Region oldClip = g.Clip;
                g.SetClip(shadowArea);
                g.FillPath(shadowBrush, shadowPath);
                g.Clip = oldClip;
            }
        }

        private void DrawPanelBackground(Graphics g)
        {
            int shrink = _shrinkContentWithShadow ? _shadowDepth : 0;

            Rectangle rect = new Rectangle(
                shrink,
                shrink,
                Width - 1 - (shrink * 2),
                Height - 1 - (shrink * 2)
            );

            using (GraphicsPath path = CreateRoundedPath(rect, _borderRadius))
            {
                // الخلفية
                if (_useGradient)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(rect, _gradientColor1, _gradientColor2, _gradientMode))
                        g.FillPath(brush, path);
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(_backColor))
                        g.FillPath(brush, path);
                }

                // الحد
                if (_borderThickness > 0)
                {
                    using (Pen borderPen = new Pen(_borderColor, _borderThickness))
                        g.DrawPath(borderPen, path);
                }
            }
        }

        private void DrawGlassEffect(Graphics g)
        {
            int shrink = _shrinkContentWithShadow ? _shadowDepth : 0;
            Rectangle rect = new Rectangle(
                shrink,
                shrink,
                Width - 1 - (shrink * 2),
                Height / 3
            );

            using (GraphicsPath glassPath = CreateRoundedPath(rect, _borderRadius, true))
            using (LinearGradientBrush glassBrush = new LinearGradientBrush(rect,
                Color.FromArgb(_glassTransparency, Color.White),
                Color.FromArgb(0, Color.White),
                LinearGradientMode.Vertical))
            {
                g.FillPath(glassBrush, glassPath);
            }
        }

        private void DrawGlowEffect(Graphics g)
        {
            int shrink = _shrinkContentWithShadow ? _shadowDepth : 0;
            Rectangle rect = new Rectangle(
                shrink,
                shrink,
                Width - 1 - (shrink * 2),
                Height - 1 - (shrink * 2)
            );

            using (GraphicsPath path = CreateRoundedPath(rect, _borderRadius))
            using (Pen glowPen = new Pen(_glowColor, 2))
            {
                glowPen.Alignment = PenAlignment.Center;
                g.DrawPath(glowPen, path);
            }
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius, bool topOnly = false)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int d = radius * 2;
            if (topOnly)
            {
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddLine(rect.Right, rect.Bottom, rect.X, rect.Bottom);
            }
            else
            {
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            }
            path.CloseFigure();
            return path;
        }

        #endregion

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
