namespace BChat.UserControls
{
    partial class CampaignsControl
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            picTemplates = new PictureBox();
            stcdCountCampaign = new BChat.Controls.StatCard();
            btnCreateACampaign = new BChat.Controls.ModernButton();
            pnlContent = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            ((System.ComponentModel.ISupportInitialize)picTemplates).BeginInit();
            SuspendLayout();
            // 
            // picTemplates
            // 
            picTemplates.Cursor = Cursors.Hand;
            picTemplates.Image = Properties.Resources.Sent;
            picTemplates.Location = new Point(84, 22);
            picTemplates.Name = "picTemplates";
            picTemplates.Size = new Size(135, 98);
            picTemplates.SizeMode = PictureBoxSizeMode.Zoom;
            picTemplates.TabIndex = 20;
            picTemplates.TabStop = false;
            // 
            // stcdCountCampaign
            // 
            stcdCountCampaign.AccentColor = Color.FromArgb(32, 201, 151);
            stcdCountCampaign.BackColor = Color.Transparent;
            stcdCountCampaign.CardColor = Color.White;
            stcdCountCampaign.IconBgColor = Color.FromArgb(220, 245, 235);
            stcdCountCampaign.IconColor = Color.FromArgb(32, 201, 151);
            stcdCountCampaign.Location = new Point(814, 708);
            stcdCountCampaign.Name = "stcdCountCampaign";
            stcdCountCampaign.ShadowColor = Color.FromArgb(30, 0, 0, 0);
            stcdCountCampaign.Size = new Size(425, 135);
            stcdCountCampaign.TabIndex = 19;
            stcdCountCampaign.Text = "statCard1";
            stcdCountCampaign.Title = "الحملات المرسلة";
            stcdCountCampaign.TitleColor = Color.FromArgb(150, 160, 175);
            stcdCountCampaign.Value = "0";
            stcdCountCampaign.ValueColor = Color.FromArgb(25, 35, 60);
            // 
            // btnCreateACampaign
            // 
            btnCreateACampaign.BackColor = Color.Transparent;
            btnCreateACampaign.BorderRadius = 20;
            btnCreateACampaign.Font = new Font("IBM Plex Sans Arabic", 10F, FontStyle.Bold);
            btnCreateACampaign.Icon = Properties.Resources.plus;
            btnCreateACampaign.Location = new Point(1068, 50);
            btnCreateACampaign.Name = "btnCreateACampaign";
            btnCreateACampaign.RightToLeft = RightToLeft.Yes;
            btnCreateACampaign.Size = new Size(209, 70);
            btnCreateACampaign.TabIndex = 18;
            btnCreateACampaign.Text = "انشاء حملة";
            btnCreateACampaign.Click += btnCreateACampaign_Click;
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.FromArgb(248, 247, 255);
            pnlContent.CustomizableEdges = customizableEdges3;
            pnlContent.FillColor = Color.FromArgb(248, 247, 255);
            pnlContent.FillColor2 = Color.FromArgb(248, 247, 255);
            pnlContent.FillColor3 = Color.FromArgb(248, 247, 255);
            pnlContent.FillColor4 = Color.FromArgb(248, 247, 255);
            pnlContent.Location = new Point(0, 229);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlContent.Size = new Size(1306, 461);
            pnlContent.TabIndex = 17;
            // 
            // MessagesControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 247, 255);
            Controls.Add(picTemplates);
            Controls.Add(stcdCountCampaign);
            Controls.Add(btnCreateACampaign);
            Controls.Add(pnlContent);
            Name = "MessagesControl";
            Size = new Size(1306, 890);
            ((System.ComponentModel.ISupportInitialize)picTemplates).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox picTemplates;
        private Controls.StatCard stcdCountCampaign;
        private Controls.ModernButton btnCreateACampaign;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlContent;
    }
}
