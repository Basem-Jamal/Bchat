using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Car_Rental_System.CustomControls
{
    [ToolboxItem(true)]
    [DefaultProperty("BackColorEx")]
    public class CustomPanel : Panel
    {
        private Color _backColorEx = Color.White;
        private Color _borderColor = Color.LightGray;
        private int _borderThickness = 1;
        private int _borderRadius = 15;

        private bool _useShadow = true;
        private int _shadowSize = 6;
        private Color _shadowColor = Color.FromArgb(80, Color.Black);

        public CustomPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true); // ✅ إضافة دعم الشفافية

            DoubleBuffered = true;
            Size = new Size(200, 120);
            Font = new Font("Segoe UI", 10f);
            ForeColor = Color.Black;
            BackColor = Color.Transparent; // ✅ جعل الخلفية شفافة
        }

        [Category("Appearance")]
        public Color BackColorEx
        {
            get => _backColorEx;
            set { _backColorEx = value; Invalidate(); }
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
        public int ShadowSize
        {
            get => _shadowSize;
            set { _shadowSize = Math.Max(0, value); Invalidate(); }
        }

        [Category("Shadow")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // ✅ رسم خلفية الأب بدل Clear
            PaintTransparentBackground(e.Graphics, ClientRectangle);

            int shadow = _useShadow ? _shadowSize : 0;

            Rectangle rectPanel = new Rectangle(0, 0, Width - 1 - shadow, Height - 1 - shadow);
            Rectangle rectShadow = new Rectangle(shadow, shadow, Width - 1 - shadow, Height - 1 - shadow);

            using (GraphicsPath pathPanel = GetRoundedPath(rectPanel, _borderRadius))
            using (GraphicsPath pathShadow = GetRoundedPath(rectShadow, _borderRadius))
            using (SolidBrush brushPanel = new SolidBrush(_backColorEx))
            using (Pen penBorder = new Pen(_borderColor, _borderThickness))
            {
                // 🎨 الظل
                if (_useShadow)
                {
                    using (PathGradientBrush shadowBrush = new PathGradientBrush(pathShadow))
                    {
                        shadowBrush.CenterColor = _shadowColor;
                        shadowBrush.SurroundColors = new Color[] { Color.Transparent };
                        e.Graphics.FillPath(shadowBrush, pathShadow);
                    }
                }

                // 🧱 الجسم الأساسي
                e.Graphics.FillPath(brushPanel, pathPanel);

                // 🖋️ الحدود
                if (_borderThickness > 0)
                    e.Graphics.DrawPath(penBorder, pathPanel);
            }
        }

        // ✅ الدالة الجديدة لرسم خلفية الأب
        private void PaintTransparentBackground(Graphics g, Rectangle rect)
        {
            if (Parent == null) return;

            var pe = new PaintEventArgs(g, rect);
            var state = g.Save();

            g.TranslateTransform(-Left, -Top);
            InvokePaintBackground(Parent, pe);
            InvokePaint(Parent, pe);

            g.Restore(state);
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
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