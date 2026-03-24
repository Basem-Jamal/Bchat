namespace BChat.Forms
{
    partial class SendCampaignForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            picClose = new PictureBox();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            btnSendCampaign = new BChat.Controls.ModernButton();
            guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            segmented = new SegmentedControl();
            cmbTemplate = new BChat.Controls.ModernComboBox();
            txbTemplateName = new BChat.Controls.ModernTextBox();
            lblCustomerName = new ReaLTaiizor.Controls.BigLabel();
            bigLabel2 = new ReaLTaiizor.Controls.BigLabel();
            bigLabel1 = new ReaLTaiizor.Controls.BigLabel();
            modernButton3 = new BChat.Controls.ModernButton();
            ((System.ComponentModel.ISupportInitialize)picClose).BeginInit();
            guna2CustomGradientPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // picClose
            // 
            picClose.Cursor = Cursors.Hand;
            picClose.Image = Properties.Resources.close;
            picClose.Location = new Point(822, 12);
            picClose.Name = "picClose";
            picClose.Size = new Size(56, 49);
            picClose.SizeMode = PictureBoxSizeMode.Zoom;
            picClose.TabIndex = 2;
            picClose.TabStop = false;
            picClose.Click += picClose_Click;
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.BorderRadius = 45;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // btnSendCampaign
            // 
            btnSendCampaign.BackColor = Color.Transparent;
            btnSendCampaign.BorderRadius = 10;
            btnSendCampaign.Font = new Font("IBM Plex Sans Arabic", 15F, FontStyle.Bold);
            btnSendCampaign.Icon = Properties.Resources.sent_hd;
            btnSendCampaign.Location = new Point(716, 814);
            btnSendCampaign.Name = "btnSendCampaign";
            btnSendCampaign.RightToLeft = RightToLeft.Yes;
            btnSendCampaign.Size = new Size(162, 62);
            btnSendCampaign.TabIndex = 6;
            btnSendCampaign.Text = "نشر";
            btnSendCampaign.Click += btnSendCampaign_Click;
            // 
            // guna2CustomGradientPanel1
            // 
            guna2CustomGradientPanel1.Controls.Add(segmented);
            guna2CustomGradientPanel1.Controls.Add(cmbTemplate);
            guna2CustomGradientPanel1.Controls.Add(txbTemplateName);
            guna2CustomGradientPanel1.Controls.Add(lblCustomerName);
            guna2CustomGradientPanel1.Controls.Add(bigLabel2);
            guna2CustomGradientPanel1.Controls.Add(bigLabel1);
            guna2CustomGradientPanel1.CustomizableEdges = customizableEdges3;
            guna2CustomGradientPanel1.Location = new Point(66, 83);
            guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            guna2CustomGradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2CustomGradientPanel1.Size = new Size(812, 712);
            guna2CustomGradientPanel1.TabIndex = 5;
            // 
            // segmented
            // 
            segmented.AccentColor = Color.FromArgb(124, 111, 247);
            segmented.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            segmented.Font = new Font("IBM Plex Sans Arabic", 15F, FontStyle.Regular, GraphicsUnit.Point, 0);
            segmented.HoverBackground = Color.FromArgb(37, 43, 74);
            segmented.LineSpacing = 50;
            segmented.Location = new Point(279, 448);
            segmented.Name = "segmented";
            segmented.OptionFontSize = 15F;
            segmented.Options = "كل العملاء";
            segmented.SelectedBackground = Color.FromArgb(124, 111, 247);
            segmented.SelectedIndex = 0;
            segmented.SelectedTextColor = Color.White;
            segmented.Size = new Size(335, 128);
            segmented.SubtitleFontSize = 11.5F;
            segmented.Subtitles = "عملاء";
            segmented.TabIndex = 12;
            segmented.UnselectedBackground = Color.FromArgb(26, 31, 60);
            segmented.UnselectedBorder = Color.FromArgb(220, 220, 235);
            segmented.UnselectedSubtitleColor = Color.FromArgb(140, 140, 160);
            segmented.UnselectedTextColor = Color.FromArgb(200, 200, 220);
            // 
            // cmbTemplate
            // 
            cmbTemplate.BackColor = Color.Transparent;
            cmbTemplate.BorderRadius = 14;
            cmbTemplate.Font = new Font("IBM Plex Sans Arabic", 12F);
            cmbTemplate.LabelText = "";
            cmbTemplate.Location = new Point(92, 303);
            cmbTemplate.Name = "cmbTemplate";
            cmbTemplate.PlaceholderText = "";
            cmbTemplate.RightToLeft = RightToLeft.Yes;
            cmbTemplate.SelectedIndex = -1;
            cmbTemplate.SelectedValue = null;
            cmbTemplate.Size = new Size(664, 57);
            cmbTemplate.TabIndex = 3;
            cmbTemplate.Text = "modernComboBox1";
            // 
            // txbTemplateName
            // 
            txbTemplateName.BackColor = Color.Transparent;
            txbTemplateName.BackColorEx = Color.FromArgb(237, 235, 255);
            txbTemplateName.BorderRadius = 14;
            txbTemplateName.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbTemplateName.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbTemplateName.LabelText = "";
            txbTemplateName.Location = new Point(92, 128);
            txbTemplateName.MaxLength = 32767;
            txbTemplateName.Name = "txbTemplateName";
            txbTemplateName.PlaceholderText = "";
            txbTemplateName.RightToLeft = RightToLeft.Yes;
            txbTemplateName.Size = new Size(664, 58);
            txbTemplateName.TabIndex = 1;
            // 
            // lblCustomerName
            // 
            lblCustomerName.AutoSize = true;
            lblCustomerName.BackColor = Color.Transparent;
            lblCustomerName.Font = new Font("IBM Plex Sans Arabic", 20.25F);
            lblCustomerName.ForeColor = Color.FromArgb(80, 80, 80);
            lblCustomerName.Location = new Point(621, 67);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Size = new Size(135, 46);
            lblCustomerName.TabIndex = 2;
            lblCustomerName.Text = "اسم الحملة";
            // 
            // bigLabel2
            // 
            bigLabel2.AutoSize = true;
            bigLabel2.BackColor = Color.Transparent;
            bigLabel2.Font = new Font("IBM Plex Sans Arabic", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bigLabel2.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel2.Location = new Point(671, 477);
            bigLabel2.Name = "bigLabel2";
            bigLabel2.Size = new Size(85, 46);
            bigLabel2.TabIndex = 11;
            bigLabel2.Text = "العملاء";
            // 
            // bigLabel1
            // 
            bigLabel1.AutoSize = true;
            bigLabel1.BackColor = Color.Transparent;
            bigLabel1.Font = new Font("IBM Plex Sans Arabic", 20.25F);
            bigLabel1.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel1.Location = new Point(592, 246);
            bigLabel1.Name = "bigLabel1";
            bigLabel1.Size = new Size(161, 46);
            bigLabel1.TabIndex = 10;
            bigLabel1.Text = "محتوى القالب";
            // 
            // modernButton3
            // 
            modernButton3.BackColor = Color.Transparent;
            modernButton3.BorderRadius = 10;
            modernButton3.Font = new Font("IBM Plex Sans Arabic", 12F, FontStyle.Bold);
            modernButton3.Icon = null;
            modernButton3.Location = new Point(584, 814);
            modernButton3.Name = "modernButton3";
            modernButton3.RightToLeft = RightToLeft.Yes;
            modernButton3.Size = new Size(114, 62);
            modernButton3.TabIndex = 7;
            modernButton3.Text = "الغاء";
            modernButton3.Variant = BChat.Controls.ButtonVariant.CustomBasem;
            modernButton3.Click += modernButton3_Click;
            // 
            // SendCampaignForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(917, 888);
            Controls.Add(modernButton3);
            Controls.Add(btnSendCampaign);
            Controls.Add(guna2CustomGradientPanel1);
            Controls.Add(picClose);
            FormBorderStyle = FormBorderStyle.None;
            Name = "SendCampaignForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SendCampaignForm";
            Load += SendCampaignForm_Load;
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            guna2CustomGradientPanel1.ResumeLayout(false);
            guna2CustomGradientPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox picClose;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Controls.ModernButton btnSendCampaign;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private Controls.ModernRichEditor rtbxContent;
        private Controls.ModernComboBox cmbTemplate;
        private Controls.ModernTextBox txbTemplateName;
        private ReaLTaiizor.Controls.BigLabel lblCustomerName;
        private ReaLTaiizor.Controls.BigLabel bigLabel2;
        private ReaLTaiizor.Controls.BigLabel bigLabel1;
        private Controls.ModernButton modernButton3;
        private SegmentedControl segmentedControl1;
        private SegmentedControl segmented;
    }
}