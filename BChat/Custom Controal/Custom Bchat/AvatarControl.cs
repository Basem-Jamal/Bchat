using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BChat.Custom_Controal.Custom_Bchat
{
    public enum AvatarImageFit
    {
        Cover,   // تملأ الكل وتقطع الزيادة
        Contain, // تظهر كاملة مع مسافات
        Stretch, // تمتد لتملأ الكل بدون نسبة
        Zoom     // زووم مخصص
    }

    public enum AvatarBorderStyle
    {
        None,
        Solid,
        Gradient,
        DoubleLine,
        Dashed,
        Glow
    }

    [ToolboxItem(true)]
    public class AvatarControl : Control
    {
        #region Fields

        private string _fullName = "User";
        private Image _image = null;
        private int _borderRadius = 50;
        private float _fontSize = 14f;
        private float _zoomFactor = 1.0f;
        private PointF _imageOffset = PointF.Empty;

        // Border
        private AvatarBorderStyle _borderStyle = AvatarBorderStyle.Solid;
        private Color _borderColor = Color.White;
        private Color _borderColor2 = Color.FromArgb(100, 180, 255);
        private int _borderThickness = 2;
        private int _glowSize = 8;
        private Color _glowColor = Color.FromArgb(80, 100, 180, 255);
        private float _gradientAngle = 135f;
        private int _dashSize = 4;

        // Image
        private AvatarImageFit _imageFit = AvatarImageFit.Cover;

        private static readonly Color[] _palette = new Color[]
        {
            Color.FromArgb(52,  152, 219),
            Color.FromArgb(46,  204, 113),
            Color.FromArgb(155, 89,  182),
            Color.FromArgb(231, 76,  60),
            Color.FromArgb(230, 126, 34),
            Color.FromArgb(26,  188, 156),
            Color.FromArgb(41,  128, 185),
            Color.FromArgb(39,  174, 96),
            Color.FromArgb(142, 68,  173),
            Color.FromArgb(192, 57,  43),
        };

        #endregion

        #region Constructor

        public AvatarControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Size = new Size(48, 48);
            Font = new Font("Segoe UI", _fontSize, FontStyle.Bold, GraphicsUnit.Point);
            Cursor = Cursors.Hand;
        }

        #endregion

        #region Properties — Avatar

        [Category("Avatar")]
        public string FullName
        {
            get => _fullName;
            set { _fullName = value?.Trim() ?? ""; Invalidate(); }
        }

        [Category("Avatar")]
        public Image AvatarImage
        {
            get => _image;
            set { _image = value; Invalidate(); }
        }

        [Category("Avatar")]
        [Description("نسبة الاستدارة 0-50 — 50 = دائرة")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, Math.Min(50, value)); Invalidate(); }
        }

        [Category("Avatar")]
        public float FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = Math.Max(6f, value);
                Font = new Font("Segoe UI", _fontSize, FontStyle.Bold, GraphicsUnit.Point);
                Invalidate();
            }
        }

        #endregion

        #region Properties — Image Fit

        [Category("Avatar - Image")]
        [Description("طريقة عرض الصورة داخل الأفاتر")]
        public AvatarImageFit ImageFit
        {
            get => _imageFit;
            set { _imageFit = value; Invalidate(); }
        }

        [Category("Avatar - Image")]
        [Description("زووم مخصص — يعمل فقط مع Zoom Fit (1.0 = طبيعي)")]
        public float ZoomFactor
        {
            get => _zoomFactor;
            set { _zoomFactor = Math.Max(0.1f, value); Invalidate(); }
        }

        [Category("Avatar - Image")]
        [Description("إزاحة الصورة أفقياً وعمودياً (بالبكسل)")]
        public PointF ImageOffset
        {
            get => _imageOffset;
            set { _imageOffset = value; Invalidate(); }
        }

        #endregion

        #region Properties — Border

        [Category("Avatar - Border")]
        public AvatarBorderStyle BorderStyle
        {
            get => _borderStyle;
            set { _borderStyle = value; Invalidate(); }
        }

        [Category("Avatar - Border")]
        [Description("اللون الأساسي للحدود")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Avatar - Border")]
        [Description("اللون الثاني — يُستخدم في Gradient و DoubleLine و Glow")]
        public Color BorderColor2
        {
            get => _borderColor2;
            set { _borderColor2 = value; Invalidate(); }
        }

        [Category("Avatar - Border")]
        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = Math.Max(0, value); Invalidate(); }
        }

        [Category("Avatar - Border")]
        [Description("زاوية التدرج — يعمل مع Gradient")]
        public float GradientAngle
        {
            get => _gradientAngle;
            set { _gradientAngle = value; Invalidate(); }
        }

        [Category("Avatar - Border")]
        [Description("حجم توهج الـ Glow")]
        public int GlowSize
        {
            get => _glowSize;
            set { _glowSize = Math.Max(1, value); Invalidate(); }
        }

        [Category("Avatar - Border")]
        [Description("لون توهج الـ Glow")]
        public Color GlowColor
        {
            get => _glowColor;
            set { _glowColor = value; Invalidate(); }
        }

        [Category("Avatar - Border")]
        [Description("حجم الشرطة — يعمل مع Dashed")]
        public int DashSize
        {
            get => _dashSize;
            set { _dashSize = Math.Max(1, value); Invalidate(); }
        }

        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            PaintTransparentBackground(g);

            // حساب مساحة الـ Glow خارج الـ border
            int glowPad = _borderStyle == AvatarBorderStyle.Glow ? _glowSize : 0;
            int thick = _borderStyle == AvatarBorderStyle.None ? 0 : _borderThickness;

            Rectangle innerRect = new Rectangle(
                glowPad + thick,
                glowPad + thick,
                Width - 1 - (glowPad + thick) * 2,
                Height - 1 - (glowPad + thick) * 2);

            Rectangle outerRect = new Rectangle(
                glowPad,
                glowPad,
                Width - 1 - glowPad * 2,
                Height - 1 - glowPad * 2);

            using (GraphicsPath innerPath = GetRoundedPath(innerRect, _borderRadius))
            using (GraphicsPath outerPath = GetRoundedPath(outerRect, _borderRadius))
            {
                // 1 — Glow خلف الكل
                if (_borderStyle == AvatarBorderStyle.Glow)
                    DrawGlow(g, outerRect);

                // 2 — الجسم
                DrawBody(g, innerRect, innerPath);

                // 3 — الحدود فوق الجسم
                if (_borderStyle != AvatarBorderStyle.None && thick > 0)
                    DrawBorder(g, outerRect, outerPath, thick);
            }
        }

        // ─── الجسم (صورة أو حرف) ───────────────────────────────────────────────

        private void DrawBody(Graphics g, Rectangle rect, GraphicsPath path)
        {
            if (_image != null)
            {
                g.SetClip(path);
                DrawImage(g, rect);
                g.ResetClip();
            }
            else
            {
                Color avatarColor = GetAvatarColor();
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect, Lighten(avatarColor, 25), avatarColor,
                    LinearGradientMode.ForwardDiagonal))
                {
                    g.FillPath(brush, path);
                }

                using (StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                using (SolidBrush tb = new SolidBrush(Color.White))
                {
                    g.DrawString(GetInitial(), Font, tb, rect, sf);
                }
            }
        }

        // ─── رسم الصورة حسب الـ Fit ─────────────────────────────────────────────

        private void DrawImage(Graphics g, Rectangle dest)
        {
            if (_image == null) return;

            float iw = _image.Width;
            float ih = _image.Height;
            float dw = dest.Width;
            float dh = dest.Height;

            RectangleF src;
            RectangleF dst;

            switch (_imageFit)
            {
                case AvatarImageFit.Cover:
                    {
                        // نسبة التكبير — نأخذ الأكبر لتغطية الكل
                        float scale = Math.Max(dw / iw, dh / ih);
                        float sw = dw / scale;
                        float sh = dh / scale;
                        float sx = (iw - sw) / 2f - _imageOffset.X / scale;
                        float sy = (ih - sh) / 2f - _imageOffset.Y / scale;
                        src = new RectangleF(sx, sy, sw, sh);
                        dst = dest;
                        break;
                    }

                case AvatarImageFit.Contain:
                    {
                        float scale = Math.Min(dw / iw, dh / ih);
                        float nw = iw * scale;
                        float nh = ih * scale;
                        float nx = dest.X + (dw - nw) / 2f + _imageOffset.X;
                        float ny = dest.Y + (dh - nh) / 2f + _imageOffset.Y;
                        src = new RectangleF(0, 0, iw, ih);
                        dst = new RectangleF(nx, ny, nw, nh);
                        break;
                    }

                case AvatarImageFit.Zoom:
                    {
                        float scale = Math.Max(dw / iw, dh / ih) * _zoomFactor;
                        float nw = iw * scale;
                        float nh = ih * scale;
                        float nx = dest.X + (dw - nw) / 2f + _imageOffset.X;
                        float ny = dest.Y + (dh - nh) / 2f + _imageOffset.Y;
                        src = new RectangleF(0, 0, iw, ih);
                        dst = new RectangleF(nx, ny, nw, nh);
                        break;
                    }

                default: // Stretch
                    src = new RectangleF(0, 0, iw, ih);
                    dst = dest;
                    break;
            }

            g.DrawImage(_image, dst, src, GraphicsUnit.Pixel);
        }

        // ─── رسم الحدود ─────────────────────────────────────────────────────────

        private void DrawBorder(Graphics g, Rectangle rect, GraphicsPath path, int thick)
        {
            switch (_borderStyle)
            {
                case AvatarBorderStyle.Solid:
                    using (Pen pen = new Pen(_borderColor, thick) { Alignment = PenAlignment.Center })
                        g.DrawPath(pen, path);
                    break;

                case AvatarBorderStyle.Gradient:
                    using (PathGradientBrush pgb = new PathGradientBrush(path))
                    {
                        pgb.CenterColor = _borderColor;
                        pgb.SurroundColors = new[] { _borderColor2 };

                        // نرسم الـ gradient كـ pen عبر تضخيم المسار
                        using (Pen pen = new Pen(pgb, thick) { Alignment = PenAlignment.Center })
                            g.DrawPath(pen, path);
                    }
                    break;

                case AvatarBorderStyle.DoubleLine:
                    // الخط الخارجي
                    using (Pen p1 = new Pen(_borderColor, 1) { Alignment = PenAlignment.Outset })
                        g.DrawPath(p1, path);
                    // الخط الداخلي
                    Rectangle inner2 = new Rectangle(
                        rect.X + thick - 1, rect.Y + thick - 1,
                        rect.Width - (thick - 1) * 2, rect.Height - (thick - 1) * 2);
                    using (GraphicsPath ip = GetRoundedPath(inner2, _borderRadius))
                    using (Pen p2 = new Pen(_borderColor2, 1) { Alignment = PenAlignment.Inset })
                        g.DrawPath(p2, ip);
                    break;

                case AvatarBorderStyle.Dashed:
                    using (Pen pen = new Pen(_borderColor, thick)
                    {
                        Alignment = PenAlignment.Center,
                        DashStyle = DashStyle.Custom,
                        DashPattern = new float[] { _dashSize, _dashSize }
                    })
                        g.DrawPath(pen, path);
                    break;

                case AvatarBorderStyle.Glow:
                    using (Pen pen = new Pen(_borderColor, thick) { Alignment = PenAlignment.Center })
                        g.DrawPath(pen, path);
                    break;
            }
        }

        private void DrawGlow(Graphics g, Rectangle rect)
        {
            for (int i = _glowSize; i >= 1; i--)
            {
                int alpha = (int)(_glowColor.A * (1f - (float)i / (_glowSize + 1)));
                Rectangle gr = new Rectangle(
                    rect.X - i, rect.Y - i,
                    rect.Width + i * 2, rect.Height + i * 2);

                using (GraphicsPath gp = GetRoundedPath(gr, _borderRadius))
                using (Pen pen = new Pen(Color.FromArgb(alpha, _glowColor), 1))
                    g.DrawPath(pen, gp);
            }
        }

        #endregion

        #region Helpers

        private void PaintTransparentBackground(Graphics g)
        {
            if (Parent == null) return;
            var pe = new PaintEventArgs(g, ClientRectangle);
            var state = g.Save();
            g.TranslateTransform(-Left, -Top);
            InvokePaintBackground(Parent, pe);
            InvokePaint(Parent, pe);
            g.Restore(state);
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radiusPercent)
        {
            int r = (int)(Math.Min(rect.Width, rect.Height) * radiusPercent / 100f) * 2;
            r = Math.Max(0, r);
            var p = new GraphicsPath();
            p.AddArc(rect.X, rect.Y, r, r, 180, 90);
            p.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            p.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            p.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            p.CloseFigure();
            return p;
        }

        private string GetInitial()
        {
            if (string.IsNullOrWhiteSpace(_fullName)) return "?";
            foreach (char c in _fullName)
                if (char.IsLetter(c)) return c.ToString().ToUpper();
            return "?";
        }

        private Color GetAvatarColor()
        {
            if (string.IsNullOrWhiteSpace(_fullName)) return _palette[0];
            return _palette[Math.Abs(_fullName.ToLowerInvariant().GetHashCode()) % _palette.Length];
        }

        private static Color Lighten(Color c, int amount) =>
            Color.FromArgb(c.A,
                Math.Min(255, c.R + amount),
                Math.Min(255, c.G + amount),
                Math.Min(255, c.B + amount));

        protected override void OnResize(EventArgs e) { base.OnResize(e); Invalidate(); }

        #endregion
    }
}