using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Custom_Controal.Custom_Bchat
{
    public class ArabicLabel : Control
    {
        public ArabicLabel()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            this.ForeColor = Color.Black;
            this.Font = new Font("IBM Plex Sans Arabic", 18.75f, FontStyle.Regular);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(this.BackColor);

            // تحسين الجودة
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            TextRenderer.DrawText(
                e.Graphics,
                this.Text,
                this.Font,
                this.ClientRectangle,
                this.ForeColor,
                TextFormatFlags.RightToLeft |
                TextFormatFlags.WordBreak |
                TextFormatFlags.VerticalCenter
            );
        }
    }
}
