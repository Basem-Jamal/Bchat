// Controls/ModernButton.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BChat.Controls
{
    public enum ButtonVariant { Primary, Secondary, Ghost, Danger, OnPrimary,
        CustomBasem
    }

    [DefaultEvent("Click")]
    [ToolboxItem(true)]
    public class ModernButton : Control
    {
        // ─── Fields 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        private ButtonVariant _variant = ButtonVariant.Primary;
        private Image? _icon;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private int _borderRadius = 999; // pill by default

        // ─── Colors per variant ───────────────────────────────
        private static readonly Color PrimaryBg = Color.FromArgb(85, 69, 205);
        private static readonly Color PrimaryHover = Color.FromArgb(63, 43, 184);
        private static readonly Color PrimaryFg = Color.White;

        private static readonly Color SecondaryBg = Color.Transparent;
        private static readonly Color SecondaryHover = Color.FromArgb(236, 236, 255);
        private static readonly Color SecondaryFg = Color.FromArgb(87, 92, 126);

        private static readonly Color GhostBg = Color.White;
        private static readonly Color GhostHover = Color.FromArgb(228, 223, 255);
        private static readonly Color GhostFg = Color.FromArgb(85, 69, 205);
        private static readonly Color GhostBorder = Color.FromArgb(50, 200, 196, 214);

        private static readonly Color DangerBg = Color.FromArgb(255, 218, 214);
        private static readonly Color DangerHover = Color.FromArgb(240, 185, 180);
        private static readonly Color DangerFg = Color.FromArgb(186, 26, 26);

        private static readonly Color OnPrimaryBg = Color.White;
        private static readonly Color OnPrimaryFg = Color.FromArgb(85, 69, 205);


        private static readonly Color CustomBasemBg = Color.FromArgb(87, 92, 126);
        private static readonly Color CustomBasemFg = Color.White;

        // ─── Properties ───────────────────────────────────────
        [Category("BChat")]
        [DefaultValue(ButtonVariant.Primary)]
        public ButtonVariant Variant
        {
            get => _variant;
            set { _variant = value; Invalidate(); }
        }

        [Category("BChat")]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        [Category("BChat")]
        [DefaultValue(999)]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = Math.Max(0, value); Invalidate(); }
        }

        // ─── Constructor ──────────────────────────────────────
        public ModernButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Size = new Size(160, 44);
            Font = new Font("IBM Plex Sans Arabic", 10f, FontStyle.Bold);
            RightToLeft = RightToLeft.Yes;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        // ─── Mouse 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true; Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false; _isPressed = false; Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true; Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false; Invalidate();
            base.OnMouseUp(e);
        }

        // ─── Paint 8387rcPNz8SRX6pYXgdxCZg3VMLFwtdJB3Z9LeX8Ge2n
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // ✅ FIX: بدلاً من g.Clear() نطلب من الـ Parent يرسم خلفيته (Gradient/Image/إلخ)
            // هذا يجعل الزر "شفاف" حقيقياً فوق أي خلفية
            if (Parent != null)
            {
                // نقل الـ Graphics لإحداثيات الـ Parent ونرسم خلفيته
                g.TranslateTransform(-Left, -Top);
                var parentArgs = new PaintEventArgs(g, new Rectangle(Left, Top, Width, Height));
                InvokePaintBackground(Parent, parentArgs);
                InvokePaint(Parent, parentArgs);
                g.TranslateTransform(Left, Top); // نرجع الإحداثيات
            }
            else
            {
                g.Clear(Color.WhiteSmoke);
            }

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = Math.Min(_borderRadius, Height / 2);

            using var path = RoundedRect(rect, r);

            // ── Background ────────────────────────────────────
            Color bg = GetBgColor();
            if (_isPressed) bg = ControlPaint.Dark(bg, 0.08f);

            if (_variant != ButtonVariant.Secondary)
            {
                using var bgBrush = new SolidBrush(bg);
                g.FillPath(bgBrush, path);
            }
            else if (_isHovered)
            {
                using var bgBrush = new SolidBrush(SecondaryHover);
                g.FillPath(bgBrush, path);
            }

            // ── Border (Ghost only) ───────────────────────────
            if (_variant == ButtonVariant.Ghost)
            {
                using var pen = new Pen(GhostBorder, 1f);
                g.DrawPath(pen, path);
            }

            // ── Scale down if pressed ─────────────────────────
            if (_isPressed)
            {
                g.ScaleTransform(0.97f, 0.97f);
                g.TranslateTransform(Width * 0.015f, Height * 0.015f);
            }

            // ── Icon + Text ───────────────────────────────────
            Color fg = GetFgColor();
            DrawContent(g, fg);
        }

        // ─── Draw Content ─────────────────────────────────────
        private void DrawContent(Graphics g, Color fg)
        {
            bool hasIcon = _icon != null;
            string txt = Text ?? "";
            bool hasText = !string.IsNullOrEmpty(txt);

            float iconW = hasIcon ? 20f : 0f;
            float gap = (hasIcon && hasText) ? 6f : 0f;

            var textSize = hasText ? g.MeasureString(txt, Font) : SizeF.Empty;
            float totalW = iconW + gap + textSize.Width;

            float startX = (Width - totalW) / 2f;
            float centerY = Height / 2f;

            // Icon
            if (hasIcon)
            {
                float iconX = startX;
                float iconY = centerY - iconW / 2f;
                g.DrawImage(_icon!, new RectangleF(iconX, iconY, iconW, iconW));
                startX += iconW + gap;
            }

            // Text
            if (hasText)
            {
                using var brush = new SolidBrush(fg);
                float textY = centerY - textSize.Height / 2f;
                g.DrawString(txt, Font, brush, new PointF(startX, textY));
            }
        }

        // ─── Color Helpers ────────────────────────────────────
        private Color GetBgColor() => _variant switch

        {   ButtonVariant.CustomBasem => CustomBasemBg,
            ButtonVariant.Primary => _isHovered ? PrimaryHover : PrimaryBg,
            ButtonVariant.Secondary => SecondaryBg,
            ButtonVariant.Ghost => _isHovered ? GhostHover : GhostBg,
            ButtonVariant.Danger => _isHovered ? DangerHover : DangerBg,
            ButtonVariant.OnPrimary => OnPrimaryBg,
            _ => PrimaryBg



        };

        private Color GetFgColor() => _variant switch
        {
            ButtonVariant.CustomBasem => CustomBasemFg,

            ButtonVariant.Primary => PrimaryFg,
            ButtonVariant.Secondary => SecondaryFg,
            ButtonVariant.Ghost => GhostFg,
            ButtonVariant.Danger => DangerFg,
            ButtonVariant.OnPrimary => OnPrimaryFg,
            _ => PrimaryFg
        };

        // ─── Rounded Rectangle Helper ─────────────────────────
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