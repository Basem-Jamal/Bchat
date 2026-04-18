using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace BChat.Custom_Controal.Custom_Bchat
{
    [ToolboxItem(true)]
    public class GroupSelectCard : Control
    {
        #region Fields

        private bool _isHovered;
        private bool _isPressed;
        private bool _isSelected;

        private int _groupId;
        private string _groupName = "Group Name";
        private string _groupDescription = "Description";
        private Image _groupIcon;
        private Color _iconBoxColor = ColorTranslator.FromHtml("#FFEDD5");

        private readonly Color Accent = ColorTranslator.FromHtml("#7C6FF7");
        private readonly Color AccentPressed = ColorTranslator.FromHtml("#6257E0");
        private readonly Color Border = ColorTranslator.FromHtml("#E2E8F0");
        private readonly Color PageBG = ColorTranslator.FromHtml("#F8F7FF");

        #endregion

        public GroupSelectCard()
        {
            Size = new Size(240, 70);
            DoubleBuffered = true;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        #region Properties

        [Category("BChat - Data")]
        public int GroupId
        {
            get => _groupId;
            set { _groupId = value; Invalidate(); }
        }

        [Category("BChat - Data")]
        public string GroupName
        {
            get => _groupName;
            set { _groupName = value; Invalidate(); }
        }

        [Category("BChat - Data")]
        public string GroupDescription
        {
            get => _groupDescription;
            set { _groupDescription = value; Invalidate(); }
        }

        [Category("BChat - Data")]
        public Image GroupIcon
        {
            get => _groupIcon;
            set { _groupIcon = value; Invalidate(); }
        }

        [Category("BChat - Appearance")]
        public Color IconBoxBackColor
        {
            get => _iconBoxColor;
            set { _iconBoxColor = value; Invalidate(); }
        }

        [Category("BChat - State")]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                Invalidate();
            }
        }

        #endregion

        #region Event

        public event Action<object, bool> SelectionChanged;

        #endregion

        #region Mouse

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
            _isPressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;

            IsSelected = !IsSelected;
            SelectionChanged?.Invoke(this, IsSelected);

            Invalidate();
        }

        #endregion

        #region Paint

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null)
                pevent.Graphics.Clear(Parent.BackColor);
            else
                base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = ClientRectangle;
            rect.Inflate(-1, -1);

            int borderWidth = IsSelected ? 2 : 1;

            Color bg = Color.White;

            if (IsSelected)
                bg = PageBG;
            else if (_isHovered)
                bg = Color.FromArgb(250, 249, 255);

            using (var path = GetRoundRect(rect, 12))
            using (var brush = new SolidBrush(bg))
            using (var pen = new Pen(IsSelected ? Accent : Border, borderWidth))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            DrawContent(g);
        }

        private void DrawContent(Graphics g)
        {
            int padding = 10;

            // Checkbox (left)
            Rectangle checkRect = new Rectangle(10, Height / 2 - 10, 20, 20);

            DrawCheckbox(g, checkRect);

            // Icon Box (right)
            Rectangle iconRect = new Rectangle(Width - 54, Height / 2 - 22, 44, 44);

            using (var brush = new SolidBrush(IconBoxBackColor))
            using (var path = GetRoundRect(iconRect, 10))
            {
                g.FillPath(brush, path);
            }

            if (GroupIcon != null)
            {
                Rectangle imgRect = new Rectangle(iconRect.X + 6, iconRect.Y + 6, 32, 32);
                g.DrawImage(GroupIcon, imgRect);
            }

            // Text
            int textRight = iconRect.Left - 10;
            int textLeft = checkRect.Right + 10;

            Rectangle titleRect = new Rectangle(textLeft, 12, textRight - textLeft, 20);
            Rectangle subRect = new Rectangle(textLeft, 32, textRight - textLeft, 20);

            using var titleFont = new Font("Cairo", 11, FontStyle.Bold);
            using var subFont = new Font("Cairo", 8, FontStyle.Regular);

            TextRenderer.DrawText(g, GroupName, titleFont, titleRect,
                ColorTranslator.FromHtml("#0F172A"),
                TextFormatFlags.Right | TextFormatFlags.EndEllipsis);

            TextRenderer.DrawText(g, GroupDescription, subFont, subRect,
                ColorTranslator.FromHtml("#64748B"),
                TextFormatFlags.Right | TextFormatFlags.EndEllipsis);
        }

        private void DrawCheckbox(Graphics g, Rectangle rect)
        {
            using (var pen = new Pen(IsSelected ? Accent : ColorTranslator.FromHtml("#CBD5E1"), 2))
            {
                g.DrawEllipse(pen, rect);
            }

            if (IsSelected)
            {
                using var brush = new SolidBrush(Accent);
                g.FillEllipse(brush, rect);

                using var pen = new Pen(Color.White, 2);
                g.DrawLines(pen, new Point[]
                {
                    new Point(rect.X + 5, rect.Y + 10),
                    new Point(rect.X + 9, rect.Y + 14),
                    new Point(rect.X + 15, rect.Y + 6)
                });
            }
        }

        private GraphicsPath GetRoundRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        #endregion
    }
}