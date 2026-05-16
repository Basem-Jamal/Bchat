namespace BChat.Menu___Nav.UserControls.CustomerUC.Customer_Info_UC
{
    partial class ucCustomerInfo
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlContent = new Car_Rental_System.CustomControls.CustomPanel();
            modernPanel1 = new ModernPanel();
            customPanel2 = new Car_Rental_System.CustomControls.CustomPanel();
            pnlContent.SuspendLayout();
            SuspendLayout();
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.FromArgb(248, 247, 255);
            pnlContent.BackColorEx = Color.FromArgb(248, 247, 255);
            pnlContent.BorderColor = Color.LightGray;
            pnlContent.BorderRadius = 1;
            pnlContent.BorderThickness = 1;
            pnlContent.Controls.Add(modernPanel1);
            pnlContent.Controls.Add(customPanel2);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Font = new Font("Segoe UI", 10F);
            pnlContent.ForeColor = Color.Black;
            pnlContent.Location = new Point(0, 0);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            pnlContent.ShadowSize = 0;
            pnlContent.Size = new Size(1176, 682);
            pnlContent.TabIndex = 0;
            pnlContent.UseShadow = true;
            // 
            // modernPanel1
            // 
            modernPanel1.BorderColor = Color.FromArgb(230, 230, 230);
            modernPanel1.BorderRadius = 1;
            modernPanel1.BorderThickness = 1;
            modernPanel1.Font = new Font("Segoe UI", 9F);
            modernPanel1.ForeColor = Color.Black;
            modernPanel1.GlassTransparency = 30;
            modernPanel1.GlowColor = Color.FromArgb(100, 0, 120, 255);
            modernPanel1.GradientColor1 = SystemColors.WindowText;
            modernPanel1.GradientColor2 = Color.FromArgb(245, 247, 250);
            modernPanel1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            modernPanel1.Location = new Point(3, -34);
            modernPanel1.Name = "modernPanel1";
            modernPanel1.ShadowBlur = 8;
            modernPanel1.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            modernPanel1.ShadowDepth = 10;
            modernPanel1.ShadowOffsetX = 3;
            modernPanel1.ShadowOffsetY = 3;
            modernPanel1.ShadowSize = 10;
            modernPanel1.ShrinkContentWithShadow = false;
            modernPanel1.Size = new Size(971, 842);
            modernPanel1.TabIndex = 2;
            modernPanel1.UseGlass = false;
            modernPanel1.UseGlow = false;
            modernPanel1.UseGradient = true;
            modernPanel1.UseShadow = true;
            // 
            // customPanel2
            // 
            customPanel2.BackColorEx = SystemColors.ActiveCaptionText;
            customPanel2.BorderColor = Color.LightGray;
            customPanel2.BorderRadius = 15;
            customPanel2.BorderThickness = 1;
            customPanel2.Font = new Font("Segoe UI", 10F);
            customPanel2.ForeColor = Color.Black;
            customPanel2.Location = new Point(1059, 103);
            customPanel2.Name = "customPanel2";
            customPanel2.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            customPanel2.ShadowSize = 6;
            customPanel2.Size = new Size(239, 621);
            customPanel2.TabIndex = 1;
            customPanel2.UseShadow = true;
            // 
            // ucCustomerInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlContent);
            Name = "ucCustomerInfo";
            Size = new Size(1176, 682);
            pnlContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Car_Rental_System.CustomControls.CustomPanel pnlContent;
        private Car_Rental_System.CustomControls.CustomPanel customPanel2;
        private ModernPanel modernPanel1;
    }
}
