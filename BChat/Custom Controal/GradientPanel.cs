// =====================================================================
//  GradientPanel.cs — v9 (True BorderRadius via Region)
//
//  ✅ [NEW] UpdateRegion() — يطبّق Region حقيقي على الكنترول
//  ✅ [NEW] OnResize يُحدّث الـ Region تلقائياً
//  ✅ الأبناء الآن محصورون داخل الزوايا الدائرية
//  ✅ كل إصلاحات v8 محفوظة
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
    [Description("بانل عصري مع Gradient وShadow ناعم وتأثيرات Glassmorphism")]
    public class GradientPanel : Panel
    {
        // ─────────────────────────────────────────
        //  Private Fields
        // ─────────────────────────────────────────
        private Color _gradientStart = Color.FromArgb(56, 203, 180);
        private Color _gradientEnd = Color.FromArgb(35, 120, 220);
        private Color _gradientMid = Color.Empty;
        private float _gradientAngle = 135f;
        private bool _useThreeColors = false;
        private int _cornerRadius = 22;

        private bool _showShadow = true;
        private Color _shadowColor = Color.FromArgb(80, 20, 80, 180);
        private int _shadowRadius = 18;
        private int _shadowOffsetX = 0;
        private int _shadowOffsetY = 4;

        private bool _hoverGlow = true;
        private Color _hoverGlowColor = Color.FromArgb(120, 56, 203, 180);
        private int _hoverGlowRadius = 24;

        private bool _showShimmer = true;
        private int _shimmerOpacity = 40;

        private bool _showGlassBorder = true;
        private int _glassBorderAlpha = 45;

        private bool _isHovered = false;

        // ─────────────────────────────────────────
        //  Win32 Message IDs
        // ─────────────────────────────────────────
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ERASEBKGND = 0x0014;

        // ─────────────────────────────────────────
        //  CreateParams
        // ─────────────────────────────────────────
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~0x00800000;   // WS_BORDER
                cp.ExStyle &= ~0x00000200; // WS_EX_CLIENTEDGE
                cp.ExStyle &= ~0x00020000; // WS_EX_STATICEDGE
                cp.ExStyle &= ~0x00000001; // WS_EX_DLGMODALFRAME
                return cp;
            }
        }

        // ─────────────────────────────────────────
        //  WndProc
        // ─────────────────────────────────────────
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCPAINT) { m.Result = IntPtr.Zero; return; }
            if (m.Msg == WM_ERASEBKGND) { m.Result = new IntPtr(1); return; }
            base.WndProc(ref m);
        }

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

            UpdateStyles();
            BackColor = Color.Transparent;
            BorderStyle = BorderStyle.None;
            Size = new Size(300, 200);

            UpdatePadding();
            UpdateRegion(); // ✅ تطبيق الـ Region من البداية
        }

        // ─────────────────────────────────────────
        //  ✅ UpdateRegion — القلب الجديد
        //  يُطبّق Region حقيقي على الكنترول بناءً على CardRect و CornerRadius
        //  بدونه: الزوايا مرسومة مدوّرة لكن الكنترول لا يزال مستطيلاً فعلياً
        // ─────────────────────────────────────────
        private void UpdateRegion()
        {
            Rectangle rc = GetCardRect();
            if (rc.Width <= 0 || rc.Height <= 0) return;

            using var path = RoundedRect(rc, _cornerRadius);
            Region = new Region(path);
        }

        // ─────────────────────────────────────────
        //  OnResize — تحديث الـ Region عند تغيير الحجم
        // ─────────────────────────────────────────
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
            Invalidate();
        }

        // ─────────────────────────────────────────
        //  UpdatePadding
        // ─────────────────────────────────────────
        private void UpdatePadding()
        {
            int sr = _showShadow ? _shadowRadius : 4;
            int ox = Math.Abs(_shadowOffsetX);
            int oy = Math.Abs(_shadowOffsetY);

            Padding = new Padding(
                Math.Max(sr + (_shadowOffsetX < 0 ? ox : 0), 6),
                Math.Max(sr + (_shadowOffsetY < 0 ? oy : 0), 6),
                Math.Max(sr + (_shadowOffsetX > 0 ? ox : 0), 6),
                Math.Max(sr + (_shadowOffsetY > 0 ? oy : 0), 6));
        }

        // ─────────────────────────────────────────
        //  OnPaintBackground
        // ─────────────────────────────────────────
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color outerBg = GetSolidAncestorColor();
            using (var brush = new SolidBrush(outerBg))
                g.FillRectangle(brush, ClientRectangle);

            Rectangle rc = GetCardRect();
            if (rc.Width <= 0 || rc.Height <= 0) return;

            using (var path = RoundedRect(rc, _cornerRadius))
            {
                g.SetClip(path);
                PaintGradient(g, rc);
                g.ResetClip();
            }
        }

        // ─────────────────────────────────────────
        //  OnPaint
        // ─────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle rc = GetCardRect();
            if (rc.Width <= 0 || rc.Height <= 0) { base.OnPaint(e); return; }

            Color outerBg = GetSolidAncestorColor();
            using (var brush = new SolidBrush(outerBg))
                g.FillRectangle(brush, ClientRectangle);

            if (_showShadow)
            {
                bool hov = _isHovered && _hoverGlow && !DesignMode;
                Color glowC = hov ? _hoverGlowColor : _shadowColor;
                int glowR = hov ? _hoverGlowRadius : _shadowRadius;
                PaintShadow(g, rc, glowC, DesignMode ? 4 : glowR);
            }

            using (var path = RoundedRect(rc, _cornerRadius))
            {
                g.SetClip(path);
                PaintGradient(g, rc);

                if (_showShimmer && _shimmerOpacity > 0)
                    PaintShimmer(g, rc);

                g.ResetClip();

                if (_showGlassBorder && _glassBorderAlpha > 0)
                    using (var pen = new Pen(Color.FromArgb(_glassBorderAlpha, 255, 255, 255), 1.2f))
                        g.DrawPath(pen, path);
            }

            base.OnPaint(e);
        }

        // ─────────────────────────────────────────
        //  Gradient Renderer
        // ─────────────────────────────────────────
        private void PaintGradient(Graphics g, Rectangle rc)
        {
            if (rc.Width <= 0 || rc.Height <= 0) return;
            Rectangle inf = Rectangle.Inflate(rc, 2, 2);

            if (_useThreeColors && _gradientMid != Color.Empty)
            {
                int h2 = Math.Max(1, rc.Height / 2);
                var top = new Rectangle(rc.X, rc.Y, rc.Width, h2 + 1);
                var bot = new Rectangle(rc.X, rc.Y + h2, rc.Width, Math.Max(1, rc.Height - h2));

                if (top.Height > 0)
                    using (var lg1 = new LinearGradientBrush(top, _gradientStart, _gradientMid, _gradientAngle))
                        g.FillRectangle(lg1, top);

                if (bot.Height > 0)
                    using (var lg2 = new LinearGradientBrush(bot, _gradientMid, _gradientEnd, _gradientAngle))
                        g.FillRectangle(lg2, bot);
            }
            else
            {
                try
                {
                    using (var lg = new LinearGradientBrush(inf, _gradientStart, _gradientEnd, _gradientAngle))
                    {
                        lg.InterpolationColors = new ColorBlend(3)
                        {
                            Colors = new[] { _gradientStart, BlendColor(_gradientStart, _gradientEnd, 0.42f), _gradientEnd },
                            Positions = new[] { 0f, 0.5f, 1f }
                        };
                        g.FillRectangle(lg, rc);
                    }
                }
                catch
                {
                    using (var lg = new LinearGradientBrush(inf, _gradientStart, _gradientEnd, _gradientAngle))
                        g.FillRectangle(lg, rc);
                }
            }
        }

        // ─────────────────────────────────────────
        //  Shimmer
        // ─────────────────────────────────────────
        private void PaintShimmer(Graphics g, Rectangle rc)
        {
            int shimH = Math.Max(1, rc.Height * 2 / 5);
            var shimR = new Rectangle(rc.X, rc.Y, rc.Width, shimH);

            using (var lg = new LinearGradientBrush(
                new Rectangle(shimR.X, shimR.Y, shimR.Width, shimR.Height + 1),
                Color.FromArgb(Math.Min(255, _shimmerOpacity), 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                LinearGradientMode.Vertical))
            {
                lg.SetSigmaBellShape(0.4f, 0.9f);
                g.FillRectangle(lg, shimR);
            }
        }

        // ─────────────────────────────────────────
        //  Shadow
        // ─────────────────────────────────────────
        private void PaintShadow(Graphics g, Rectangle card, Color clr, int radius)
        {
            if (radius <= 0 || clr.A == 0) return;

            int ox = DesignMode ? 0 : _shadowOffsetX;
            int oy = DesignMode ? 0 : _shadowOffsetY;

            for (int i = radius; i >= 1; i--)
            {
                float t = (float)i / radius;
                int alpha = (int)(clr.A * (1f - t * t) * 0.75f);
                if (alpha <= 0) continue;

                var sr = new Rectangle(
                    card.Left - i + ox, card.Top - i + oy,
                    card.Width + i * 2, card.Height + i * 2);

                int cr = Math.Min(_cornerRadius + i, Math.Min(sr.Width, sr.Height) / 2);

                using (var sp = RoundedRect(sr, cr))
                using (var sb = new SolidBrush(Color.FromArgb(Math.Min(255, alpha), clr.R, clr.G, clr.B)))
                    g.FillPath(sb, sp);
            }
        }

        // ─────────────────────────────────────────
        //  GetCardRect
        // ─────────────────────────────────────────
        private Rectangle GetCardRect()
        {
            int sr = DesignMode ? 4 : (_showShadow ? _shadowRadius : 4);
            int ox = DesignMode ? 0 : _shadowOffsetX;
            int oy = DesignMode ? 0 : _shadowOffsetY;

            int left = sr + Math.Max(0, -ox);
            int top = sr + Math.Max(0, -oy);
            int right = sr + Math.Max(0, ox);
            int bottom = sr + Math.Max(0, oy);

            return new Rectangle(
                left, top,
                Math.Max(1, Width - left - right - 1),
                Math.Max(1, Height - top - bottom - 1));
        }

        // ─────────────────────────────────────────
        //  OnControlAdded
        // ─────────────────────────────────────────
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            ApplyTransparency(e.Control);
        }

        private void ApplyTransparency(Control ctrl)
        {
            if (ctrl is ButtonBase btn)
            {
                if (btn.FlatStyle != FlatStyle.Flat && btn.FlatStyle != FlatStyle.Popup)
                { btn.FlatStyle = FlatStyle.Flat; btn.FlatAppearance.BorderSize = 0; }
                if (btn.BackColor == SystemColors.Control || btn.BackColor == SystemColors.ButtonFace)
                    btn.BackColor = Color.Transparent;
            }
            else if (ctrl is Label lbl) { lbl.BackColor = Color.Transparent; }
            else if (ctrl is PictureBox pb) { pb.BackColor = Color.Transparent; }
            else
            {
                ctrl.SetStyle_IfSupported(ControlStyles.SupportsTransparentBackColor, true);
                if (ctrl.BackColor == SystemColors.Control)
                    ctrl.BackColor = Color.Transparent;
            }
            foreach (Control child in ctrl.Controls)
                ApplyTransparency(child);
        }

        // ─────────────────────────────────────────
        //  Mouse Events
        // ─────────────────────────────────────────
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!DesignMode) { _isHovered = true; Invalidate(); }
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (!DesignMode && !ClientRectangle.Contains(PointToClient(MousePosition)))
            { _isHovered = false; Invalidate(); }
            base.OnMouseLeave(e);
        }

        // ─────────────────────────────────────────
        //  Properties
        // ─────────────────────────────────────────
        [Category("✦ Gradient")]
        [Description("لون بداية الجراديينت")]
        public Color GradientStartColor { get => _gradientStart; set { _gradientStart = value; Invalidate(); } }

        [Category("✦ Gradient")]
        [Description("لون نهاية الجراديينت")]
        public Color GradientEndColor { get => _gradientEnd; set { _gradientEnd = value; Invalidate(); } }

        [Category("✦ Gradient")]
        [Description("لون وسط اختياري")]
        public Color GradientMidColor { get => _gradientMid; set { _gradientMid = value; Invalidate(); } }

        [Category("✦ Gradient")]
        [DefaultValue(false)]
        [Description("ثلاثة ألوان")]
        public bool UseThreeColors { get => _useThreeColors; set { _useThreeColors = value; Invalidate(); } }

        [Category("✦ Gradient")]
        [DefaultValue(135f)]
        [Description("زاوية الجراديينت")]
        public float GradientAngle { get => _gradientAngle; set { _gradientAngle = value % 360f; Invalidate(); } }

        [Category("✦ Gradient")]
        [DefaultValue(22)]
        [Description("نصف قطر الزوايا")]
        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = Math.Max(0, value);
                UpdateRegion(); // ✅ تحديث الـ Region فور تغيير القيمة
                Invalidate();
            }
        }

        [Category("✦ Shadow")]
        [DefaultValue(true)]
        [Description("تفعيل الظل")]
        public bool ShowShadow
        {
            get => _showShadow;
            set { _showShadow = value; UpdatePadding(); UpdateRegion(); Invalidate(); }
        }

        [Category("✦ Shadow")]
        [Description("لون الظل")]
        public Color ShadowColor { get => _shadowColor; set { _shadowColor = value; Invalidate(); } }

        [Category("✦ Shadow")]
        [DefaultValue(18)]
        [Description("نصف قطر الظل")]
        public int ShadowRadius
        {
            get => _shadowRadius;
            set { _shadowRadius = Math.Max(0, value); UpdatePadding(); UpdateRegion(); Invalidate(); }
        }

        [Category("✦ Shadow")]
        [DefaultValue(0)]
        [Description("إزاحة الظل أفقياً")]
        public int ShadowOffsetX
        {
            get => _shadowOffsetX;
            set { _shadowOffsetX = value; UpdatePadding(); UpdateRegion(); Invalidate(); }
        }

        [Category("✦ Shadow")]
        [DefaultValue(4)]
        [Description("إزاحة الظل عمودياً")]
        public int ShadowOffsetY
        {
            get => _shadowOffsetY;
            set { _shadowOffsetY = value; UpdatePadding(); UpdateRegion(); Invalidate(); }
        }

        [Category("✦ Shadow")]
        [DefaultValue(true)]
        [Description("Glow عند Hover")]
        public bool HoverGlow { get => _hoverGlow; set { _hoverGlow = value; Invalidate(); } }

        [Category("✦ Shadow")]
        [Description("لون Glow")]
        public Color HoverGlowColor { get => _hoverGlowColor; set { _hoverGlowColor = value; Invalidate(); } }

        [Category("✦ Shadow")]
        [DefaultValue(24)]
        [Description("حجم Glow")]
        public int HoverGlowRadius { get => _hoverGlowRadius; set { _hoverGlowRadius = Math.Max(0, value); Invalidate(); } }

        [Category("✦ Appearance")]
        [DefaultValue(true)]
        [Description("تأثير Shimmer")]
        public bool ShowShimmer { get => _showShimmer; set { _showShimmer = value; Invalidate(); } }

        [Category("✦ Appearance")]
        [DefaultValue(40)]
        [Description("شدة Shimmer: 0–120")]
        public int ShimmerOpacity { get => _shimmerOpacity; set { _shimmerOpacity = Math.Max(0, Math.Min(120, value)); Invalidate(); } }

        [Category("✦ Appearance")]
        [DefaultValue(true)]
        [Description("الحد الزجاجي")]
        public bool ShowGlassBorder { get => _showGlassBorder; set { _showGlassBorder = value; Invalidate(); } }

        [Category("✦ Appearance")]
        [DefaultValue(45)]
        [Description("شفافية الحد الزجاجي")]
        public int GlassBorderAlpha { get => _glassBorderAlpha; set { _glassBorderAlpha = Math.Max(0, Math.Min(255, value)); Invalidate(); } }

        // ─────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────
        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            radius = Math.Max(0, Math.Min(radius, Math.Min(r.Width, r.Height) / 2));
            var path = new GraphicsPath();
            if (radius == 0) { path.AddRectangle(r); return path; }
            int d = radius * 2;
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Color GetSolidAncestorColor()
        {
            var p = Parent;
            while (p != null)
            {
                if (p.BackColor.A > 0 && p.BackColor != Color.Transparent)
                    return p.BackColor;
                p = p.Parent;
            }
            return SystemColors.Control;
        }

        private static Color BlendColor(Color a, Color b, float t) =>
            Color.FromArgb(
                Clamp255((int)(a.A + (b.A - a.A) * t)),
                Clamp255((int)(a.R + (b.R - a.R) * t)),
                Clamp255((int)(a.G + (b.G - a.G) * t)),
                Clamp255((int)(a.B + (b.B - a.B) * t)));

        private static int Clamp255(int v) => v < 0 ? 0 : v > 255 ? 255 : v;
    }

    internal static class ControlExtensions
    {
        public static void SetStyle_IfSupported(this Control ctrl, ControlStyles flag, bool value)
        {
            try
            {
                var m = typeof(Control).GetMethod("SetStyle",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                m?.Invoke(ctrl, new object[] { flag, value });
            }
            catch { }
        }
    }
}