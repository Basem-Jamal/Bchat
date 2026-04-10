using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BChat.Custom_Controal
{
    [ToolboxItem(true)]
    [Description("بطاقة إضافة مجموعة جديدة — بحدود منقطة")]
    public class AddGroupCard : Control
    {
        // State
        private bool _isHovered;
        private bool _isPressed;

        // Fields
        private Color _cardBackColor = Color.White;
        private Color _cardHoverBackColor = Color.FromArgb(248, 247, 255);
        private Color _cardPressedBackColor = Color.FromArgb(243, 240, 255);

        private Color _dashedBorderColor = Color.FromArgb(196, 181, 253);
        private Color _dashedHoverBorderColor = Color.FromArgb(124, 111, 247);

        private int _cardBorderRadius = 16;
        private float _dashedBorderWidth = 1.5f;
        private float _dashLength = 7f;
        private float _dashSpace = 4f;

        private Color _plusIconColor = Color.FromArgb(124, 111, 247);
        private Color _plusIconBgColor = Color.FromArgb(237, 233, 254);
        private Color _plusIconBgHoverColor = Color.FromArgb(221, 214, 254);

        private int _plusIconBgSize = 52;
        private int _plusIconBgRadius = 26;
        private float _plusLineSize = 16f;
        private float _plusLineWidth = 2.2f;

        private string _title = "إضافة مجموعة جديدة";
        private string _subtitle = "ابدأ بتصنيف عملائك الآن";

        private Color _titleColor = Color.FromArgb(15, 23, 42);
        private Color _subtitleColor = Color.FromArgb(100, 116, 139);

        private Font _titleFont = new Font("IBM Plex Sans Arabic", 13f, FontStyle.Bold);
        private Font _subtitleFont = new Font("IBM Plex Sans Arabic", 9.5f, FontStyle.Regular);

        private int _titleSubtitleGap = 6;

        // Event
        [Category("BChat - Events")]
        public event EventHandler AddClicked;

        public AddGroupCard()
        {


            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            UpdateStyles();

            Size = new Size(280, 215);

            // ❗ أهم تعديل: ممنوع Transparent في Designer
            BackColor = Color.White;

            Cursor = Cursors.Hand;
        }

        // ❗ FIX: Designer Safe Background
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (DesignMode)
            {
                base.OnPaintBackground(e);
                return;
            }

            if (Parent != null)
            {
                Graphics g = e.Graphics;

                // نحفظ الحالة
                var state = g.Save();

                // نحرك الرسم لمكان الكنترول
                g.TranslateTransform(-Left, -Top);

                // نرسم الأب (الخلفية الحقيقية)
                var pe = new PaintEventArgs(g, Parent.ClientRectangle);
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);

                // نرجع الحالة
                g.Restore(state);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var bounds = new RectangleF(1, 1, Width - 2, Height - 2);

            Color bg = _isPressed
                ? _cardPressedBackColor
                : _isHovered
                    ? _cardHoverBackColor
                    : _cardBackColor;

            using (var path = GetRoundedRect(bounds, _cardBorderRadius))
            using (var brush = new SolidBrush(bg))
                g.FillPath(brush, path);

            Color borderColor = (_isHovered || _isPressed)
                ? _dashedHoverBorderColor
                : _dashedBorderColor;

            using (var path = GetRoundedRect(bounds, _cardBorderRadius))
            using (var pen = new Pen(borderColor, _dashedBorderWidth))
            {
                pen.DashStyle = DashStyle.Custom;
                pen.DashPattern = new float[] { _dashLength, _dashSpace };
                g.DrawPath(pen, path);
            }

            float cx = Width / 2f;

            float titleH = g.MeasureString(_title, _titleFont).Height;
            float subH = g.MeasureString(_subtitle, _subtitleFont).Height;

            float totalH = _plusIconBgSize + 14f + titleH + _titleSubtitleGap + subH;
            float startY = (Height - totalH) / 2f;

            var bgRect = new RectangleF(
                cx - _plusIconBgSize / 2f,
                startY,
                _plusIconBgSize,
                _plusIconBgSize
            );

            Color plusBg = (_isHovered || _isPressed)
                ? _plusIconBgHoverColor
                : _plusIconBgColor;

            using (var path = GetRoundedRect(bgRect, _plusIconBgRadius))
            using (var brush = new SolidBrush(plusBg))
                g.FillPath(brush, path);

            float plusCx = bgRect.X + bgRect.Width / 2f;
            float plusCy = bgRect.Y + bgRect.Height / 2f;
            float half = _plusLineSize / 2f;

            using (var pen = new Pen(_plusIconColor, _plusLineWidth)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            })
            {
                g.DrawLine(pen, plusCx - half, plusCy, plusCx + half, plusCy);
                g.DrawLine(pen, plusCx, plusCy - half, plusCx, plusCy + half);
            }

            float titleY = startY + _plusIconBgSize + 14f;

            using (var brush = new SolidBrush(_titleColor))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(_title, _titleFont, brush,
                    new RectangleF(0, titleY, Width, titleH), sf);
            }

            using (var brush = new SolidBrush(_subtitleColor))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(_subtitle, _subtitleFont, brush,
                    new RectangleF(0, titleY + titleH + _titleSubtitleGap, Width, subH), sf);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                bool was = _isPressed;
                _isPressed = false;
                Invalidate();

                if (was && ClientRectangle.Contains(e.Location))
                    AddClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        private GraphicsPath GetRoundedRect(RectangleF rect, float radius)
        {
            float r = Math.Max(1, radius * 2f);
            var path = new GraphicsPath();

            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);

            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _titleFont?.Dispose();
                _subtitleFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}