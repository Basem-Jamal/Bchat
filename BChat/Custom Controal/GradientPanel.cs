// =====================================================================
//  GradientPanel.cs — v3 (No System.Design dependency)
//  ✅ لا يحتاج System.Design.dll — يعمل في أي مشروع WinForms
//  ✅ Gradient قابل للتخصيص من Designer
//  ✅ Shadow ناعم من كل الاتجاهات
//  ✅ Glassmorphism shimmer layer
//  ✅ Hover Glow effect
//  ✅ Drag & Drop في Designer بدون أخطاء
// =====================================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat
{
    [ToolboxItem(true)]
    [DefaultProperty("GradientStartColor")]
    [Description("بانل عصري 2026 مع Gradient وShadow من كل الاتجاهات")]
    public class GradientPanel : Panel          // ✅ Panel مباشرة — لا ContainerControl
    {
        // ─────────────────────────────────────────
        //  Private Fields
        // ─────────────────────────────────────────
        private Color _gradientStart = Color.FromArgb(56, 203, 180);
        private Color _gradientEnd = Color.FromArgb(35, 120, 220);
        private Color _gradientMid = Color.Empty;
        private float _gradientAngle = 135f;
        private bool _useThreeColors = false;
        private Color _shadowColor = Color.FromArgb(90, 35, 120, 220);
        private int _shadowRadius = 18;
        private bool _hoverGlow = true;
        private Color _hoverGlowColor = Color.FromArgb(140, 56, 203, 180);
        private int _hoverGlowRadius = 26;
        private int _cornerRadius = 22;
        private bool _isHovered = false;

        // ─────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────
        public GradientPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            Size = new Size(300, 200);
        }

        // ─────────────────────────────────────────
        //  Gradient Properties
        // ─────────────────────────────────────────
        [Category("✦ Gradient")]
        [Description("لون بداية الجراديينت")]
        public Color GradientStartColor
        {
            get => _gradientStart;
            set { _gradientStart = value; Invalidate(); }
        }

        [Category("✦ Gradient")]
        [Description("لون نهاية الجراديينت")]
        public Color GradientEndColor
        {
            get => _gradientEnd;
            set { _gradientEnd = value; Invalidate(); }
        }

        [Category("✦ Gradient")]
        [Description("لون وسط اختياري — يعمل مع UseThreeColors = true")]
        public Color GradientMidColor
        {
            get => _gradientMid;
            set { _gradientMid = value; Invalidate(); }
        }

        [Category("✦ Gradient")]
        [DefaultValue(false)]
        [Description("فعّل ثلاثة ألوان في الجراديينت")]
        public bool UseThreeColors
        {
            get => _useThreeColors;
            set { _useThreeColors = value; Invalidate(); }
        }

        [Category("✦ Gradient")]
        [DefaultValue(135f)]
        [Description("زاوية الجراديينت: 0=أفقي | 90=عمودي | 135=قطري")]
        public float GradientAngle
        {
            get => _gradientAngle;
            set { _gradientAngle = value % 360f; Invalidate(); }
        }

        // ─────────────────────────────────────────
        //  Shadow Properties
        // ─────────────────────────────────────────
        [Category("✦ Shadow")]
        [Description("لون الظل — Alpha يتحكم في الكثافة")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("✦ Shadow")]
        [DefaultValue(18)]
        [Description("نصف قطر الظل من كل الاتجاهات")]
        public int ShadowRadius
        {
            get => _shadowRadius;
            set { _shadowRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("✦ Shadow")]
        [DefaultValue(true)]
        [Description("تأثير Glow عند تمرير الماوس")]
        public bool HoverGlow
        {
            get => _hoverGlow;
            set { _hoverGlow = value; Invalidate(); }
        }

        [Category("✦ Shadow")]
        [Description("لون Glow عند Hover")]
        public Color HoverGlowColor
        {
            get => _hoverGlowColor;
            set { _hoverGlowColor = value; Invalidate(); }
        }

        [Category("✦ Shadow")]
        [DefaultValue(26)]
        [Description("حجم Glow عند Hover")]
        public int HoverGlowRadius
        {
            get => _hoverGlowRadius;
            set { _hoverGlowRadius = Math.Max(0, value); Invalidate(); }
        }

        // ─────────────────────────────────────────
        //  Shape
        // ─────────────────────────────────────────
        [Category("✦ Gradient")]
        [DefaultValue(22)]
        [Description("نصف قطر الزوايا المدورة")]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = Math.Max(0, value); Invalidate(); }
        }

        // ─────────────────────────────────────────
        //  Mouse — Hover Glow
        // ─────────────────────────────────────────
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!DesignMode) { _isHovered = true; Invalidate(); }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!DesignMode && !ClientRectangle.Contains(PointToClient(MousePosition)))
            {
                _isHovered = false;
                Invalidate();
            }
            base.OnMouseLeave(e);
        }

        // ─────────────────────────────────────────
        //  Paint
        // ─────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // ① مسح الخلفية بلون الـ Parent
            Color bg = Parent?.BackColor ?? SystemColors.Control;
            using (SolidBrush eb = new SolidBrush(bg))
                g.FillRectangle(eb, ClientRectangle);

            // منطقة البانل الفعلية (تترك مساحة للظل)
            int sr = DesignMode ? 4 : _shadowRadius;
            Rectangle rc = new Rectangle(sr, sr, Width - sr * 2 - 1, Height - sr * 2 - 1);
            if (rc.Width <= 0 || rc.Height <= 0) return;

            // ② الظل من كل الاتجاهات
            Color glowC = (_isHovered && _hoverGlow) ? _hoverGlowColor : _shadowColor;
            int glowR = (_isHovered && _hoverGlow) ? _hoverGlowRadius : _shadowRadius;
            PaintShadow(g, rc, glowC, glowR);

            // ③ الجراديينت + Clip بالزوايا المدورة
            using (GraphicsPath path = RoundedRect(rc, _cornerRadius))
            {
                g.SetClip(path);
                PaintGradient(g, rc);
                g.ResetClip();

                // ④ حافة زجاجية ناعمة
                using (Pen glassPen = new Pen(Color.FromArgb(55, 255, 255, 255), 1.5f))
                    g.DrawPath(glassPen, path);
            }

            // ✅ base يرسم الـ Controls الداخلية
            base.OnPaint(e);
        }

        // ─────────────────────────────────────────
        //  Gradient Renderer
        // ─────────────────────────────────────────
        private void PaintGradient(Graphics g, Rectangle rc)
        {
            Rectangle inf = new Rectangle(rc.X - 1, rc.Y - 1, rc.Width + 2, rc.Height + 2);

            if (_useThreeColors && _gradientMid != Color.Empty)
            {
                int h2 = rc.Height / 2;
                Rectangle top = new Rectangle(rc.X, rc.Y, rc.Width, h2 + 1);
                Rectangle bot = new Rectangle(rc.X, rc.Y + h2, rc.Width, rc.Height - h2);

                using (var lg1 = new LinearGradientBrush(top, _gradientStart, _gradientMid, _gradientAngle))
                    g.FillRectangle(lg1, top);
                using (var lg2 = new LinearGradientBrush(bot, _gradientMid, _gradientEnd, _gradientAngle))
                    g.FillRectangle(lg2, bot);
            }
            else
            {
                using (var lg = new LinearGradientBrush(inf, _gradientStart, _gradientEnd, _gradientAngle))
                {
                    lg.InterpolationColors = new ColorBlend(3)
                    {
                        Colors = new[] { _gradientStart, BlendColor(_gradientStart, _gradientEnd, 0.45f), _gradientEnd },
                        Positions = new[] { 0f, 0.5f, 1f }
                    };
                    g.FillRectangle(lg, rc);
                }
            }

            // Glassmorphism shimmer (طبقة بيضاء علوية خفيفة)
            Rectangle shimmer = new Rectangle(rc.X, rc.Y, rc.Width, rc.Height / 3);
            if (shimmer.Height > 0)
            {
                using (var sg = new LinearGradientBrush(
                    shimmer,
                    Color.FromArgb(50, 255, 255, 255),
                    Color.FromArgb(0, 255, 255, 255),
                    90f))
                    g.FillRectangle(sg, shimmer);
            }
        }

        // ─────────────────────────────────────────
        //  Shadow — Omni-directional
        // ─────────────────────────────────────────
        private void PaintShadow(Graphics g, Rectangle card, Color clr, int radius)
        {
            if (radius <= 0) return;
            for (int i = radius; i >= 1; i--)
            {
                Rectangle sr = new Rectangle(
                    card.Left - i,
                    card.Top - i,
                    card.Width + i * 2,
                    card.Height + i * 2);

                float t = 1f - (i / (float)radius);
                int alpha = (int)(clr.A * t * t * 0.55f);
                alpha = Math.Max(0, Math.Min(255, alpha));
                if (alpha == 0) continue;

                using (GraphicsPath sp = RoundedRect(sr, _cornerRadius + i))
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(alpha, clr.R, clr.G, clr.B)))
                    g.FillPath(sb, sp);
            }
        }

        // ─────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────
        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            radius = Math.Max(0, Math.Min(radius, Math.Min(r.Width, r.Height) / 2));
            var p = new GraphicsPath();
            if (radius == 0) { p.AddRectangle(r); return p; }
            int d = radius * 2;
            p.AddArc(r.Left, r.Top, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static Color BlendColor(Color a, Color b, float t) =>
            Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
    }
}