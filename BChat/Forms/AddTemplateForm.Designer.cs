namespace BChat.Forms
{
    partial class AddTemplateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTemplateForm));
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            picClose = new PictureBox();
            lblCustomerName = new ReaLTaiizor.Controls.BigLabel();
            pictureBox1 = new PictureBox();
            bigLabel1 = new ReaLTaiizor.Controls.BigLabel();
            bigLabel2 = new ReaLTaiizor.Controls.BigLabel();
            txbTemplateName = new BChat.Controls.ModernTextBox();
            cmbCategory = new BChat.Controls.ModernComboBox();
            pnlContent = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            customPanel1 = new Car_Rental_System.CustomControls.CustomPanel();
            bigLabel9 = new ReaLTaiizor.Controls.BigLabel();
            txbPhoneNumber = new BChat.Controls.ModernTextBox();
            bigLabel8 = new ReaLTaiizor.Controls.BigLabel();
            txbPhoneText = new BChat.Controls.ModernTextBox();
            bigLabel7 = new ReaLTaiizor.Controls.BigLabel();
            txbUrlLink = new BChat.Controls.ModernTextBox();
            bigLabel5 = new ReaLTaiizor.Controls.BigLabel();
            txbUrlText = new BChat.Controls.ModernTextBox();
            bigLabel3 = new ReaLTaiizor.Controls.BigLabel();
            txbQuickReply = new BChat.Controls.ModernTextBox();
            txbImageUrl = new BChat.Controls.ModernTextBox();
            txbHeaderText = new BChat.Controls.ModernTextBox();
            cmbHeaderType = new BChat.Controls.ModernComboBox();
            bigLabel6 = new ReaLTaiizor.Controls.BigLabel();
            bigLabel4 = new ReaLTaiizor.Controls.BigLabel();
            cmbLanguage = new BChat.Controls.ModernComboBox();
            rtbxContent = new BChat.Controls.ModernRichEditor();
            btnAddTemplate = new BChat.Controls.ModernButton();
            ((System.ComponentModel.ISupportInitialize)picClose).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlContent.SuspendLayout();
            customPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.BorderRadius = 45;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // picClose
            // 
            picClose.Cursor = Cursors.Hand;
            picClose.Image = Properties.Resources.close;
            picClose.Location = new Point(968, 6);
            picClose.Name = "picClose";
            picClose.Size = new Size(56, 49);
            picClose.SizeMode = PictureBoxSizeMode.Zoom;
            picClose.TabIndex = 1;
            picClose.TabStop = false;
            picClose.Click += picClose_Click;
            // 
            // lblCustomerName
            // 
            lblCustomerName.AutoSize = true;
            lblCustomerName.BackColor = Color.Transparent;
            lblCustomerName.Font = new Font("Segoe UI", 20F);
            lblCustomerName.ForeColor = Color.FromArgb(80, 80, 80);
            lblCustomerName.Location = new Point(1297, 359);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Size = new Size(141, 37);
            lblCustomerName.TabIndex = 2;
            lblCustomerName.Text = "اسم القالب";
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.add_Template;
            pictureBox1.Location = new Point(463, 6);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(135, 98);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // bigLabel1
            // 
            bigLabel1.AutoSize = true;
            bigLabel1.BackColor = Color.Transparent;
            bigLabel1.Font = new Font("Segoe UI", 20F);
            bigLabel1.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel1.Location = new Point(278, 292);
            bigLabel1.Name = "bigLabel1";
            bigLabel1.Size = new Size(170, 37);
            bigLabel1.TabIndex = 10;
            bigLabel1.Text = "محتوى القالب";
            // 
            // bigLabel2
            // 
            bigLabel2.AutoSize = true;
            bigLabel2.BackColor = Color.Transparent;
            bigLabel2.Font = new Font("Segoe UI", 20F);
            bigLabel2.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel2.Location = new Point(1218, 74);
            bigLabel2.Name = "bigLabel2";
            bigLabel2.Size = new Size(170, 37);
            bigLabel2.TabIndex = 11;
            bigLabel2.Text = "محتوى القالب";
            // 
            // txbTemplateName
            // 
            txbTemplateName.BackColor = Color.Transparent;
            txbTemplateName.BackColorEx = Color.FromArgb(237, 235, 255);
            txbTemplateName.BorderRadius = 14;
            txbTemplateName.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbTemplateName.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbTemplateName.LabelText = "";
            txbTemplateName.Location = new Point(747, 345);
            txbTemplateName.MaxLength = 32767;
            txbTemplateName.Name = "txbTemplateName";
            txbTemplateName.PlaceholderText = "";
            txbTemplateName.RightToLeft = RightToLeft.Yes;
            txbTemplateName.Size = new Size(475, 70);
            txbTemplateName.TabIndex = 1;
            // 
            // cmbCategory
            // 
            cmbCategory.ArrowColor = Color.FromArgb(124, 111, 247);
            cmbCategory.BorderColor = Color.FromArgb(220, 215, 250);
            cmbCategory.DropdownBackColor = Color.White;
            cmbCategory.FocusBorderColor = Color.FromArgb(124, 111, 247);
            cmbCategory.Font = new Font("IBM Plex Sans Arabic", 12F);
            cmbCategory.ItemHoverColor = Color.FromArgb(237, 235, 255);
            cmbCategory.LabelText = "";
            cmbCategory.Location = new Point(980, 74);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.PlaceholderText = "";
            cmbCategory.RightToLeft = RightToLeft.Yes;
            cmbCategory.SelectedIndex = -1;
            cmbCategory.SelectedItem = null;
            cmbCategory.Size = new Size(220, 40);
            cmbCategory.TabIndex = 3;
            cmbCategory.Text = "modernComboBox1";
            cmbCategory.TextColor = Color.FromArgb(40, 40, 70);
            cmbCategory.UsePlaceholder = true;
            // 
            // pnlContent
            // 
            pnlContent.Controls.Add(customPanel1);
            pnlContent.Controls.Add(txbImageUrl);
            pnlContent.Controls.Add(txbHeaderText);
            pnlContent.Controls.Add(cmbHeaderType);
            pnlContent.Controls.Add(bigLabel6);
            pnlContent.Controls.Add(bigLabel4);
            pnlContent.Controls.Add(cmbLanguage);
            pnlContent.Controls.Add(rtbxContent);
            pnlContent.Controls.Add(cmbCategory);
            pnlContent.Controls.Add(txbTemplateName);
            pnlContent.Controls.Add(lblCustomerName);
            pnlContent.Controls.Add(bigLabel2);
            pnlContent.Controls.Add(bigLabel1);
            pnlContent.CustomizableEdges = customizableEdges3;
            pnlContent.Location = new Point(12, 129);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlContent.Size = new Size(1458, 902);
            pnlContent.TabIndex = 0;
            // 
            // customPanel1
            // 
            customPanel1.BackColorEx = Color.White;
            customPanel1.BorderColor = Color.LightGray;
            customPanel1.BorderRadius = 15;
            customPanel1.BorderThickness = 1;
            customPanel1.Controls.Add(bigLabel9);
            customPanel1.Controls.Add(txbPhoneNumber);
            customPanel1.Controls.Add(bigLabel8);
            customPanel1.Controls.Add(txbPhoneText);
            customPanel1.Controls.Add(bigLabel7);
            customPanel1.Controls.Add(txbUrlLink);
            customPanel1.Controls.Add(bigLabel5);
            customPanel1.Controls.Add(txbUrlText);
            customPanel1.Controls.Add(bigLabel3);
            customPanel1.Controls.Add(txbQuickReply);
            customPanel1.Font = new Font("Segoe UI", 10F);
            customPanel1.ForeColor = Color.Black;
            customPanel1.Location = new Point(747, 473);
            customPanel1.Name = "customPanel1";
            customPanel1.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            customPanel1.ShadowSize = 6;
            customPanel1.Size = new Size(691, 413);
            customPanel1.TabIndex = 20;
            customPanel1.UseShadow = true;
            // 
            // bigLabel9
            // 
            bigLabel9.AutoSize = true;
            bigLabel9.BackColor = Color.Transparent;
            bigLabel9.Font = new Font("Segoe UI", 14.25F);
            bigLabel9.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel9.Location = new Point(490, 360);
            bigLabel9.Name = "bigLabel9";
            bigLabel9.Size = new Size(167, 25);
            bigLabel9.TabIndex = 29;
            bigLabel9.Text = "رقم الهاتف (+966...)";
            // 
            // txbPhoneNumber
            // 
            txbPhoneNumber.BackColor = Color.Transparent;
            txbPhoneNumber.BackColorEx = Color.FromArgb(237, 235, 255);
            txbPhoneNumber.BorderRadius = 14;
            txbPhoneNumber.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbPhoneNumber.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbPhoneNumber.LabelText = "";
            txbPhoneNumber.Location = new Point(16, 350);
            txbPhoneNumber.MaxLength = 32767;
            txbPhoneNumber.Name = "txbPhoneNumber";
            txbPhoneNumber.PlaceholderText = "";
            txbPhoneNumber.RightToLeft = RightToLeft.Yes;
            txbPhoneNumber.Size = new Size(397, 46);
            txbPhoneNumber.TabIndex = 28;
            // 
            // bigLabel8
            // 
            bigLabel8.AutoSize = true;
            bigLabel8.BackColor = Color.Transparent;
            bigLabel8.Font = new Font("Segoe UI", 14.25F);
            bigLabel8.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel8.Location = new Point(506, 278);
            bigLabel8.Name = "bigLabel8";
            bigLabel8.Size = new Size(126, 25);
            bigLabel8.TabIndex = 27;
            bigLabel8.Text = "نص زر الاتصال";
            // 
            // txbPhoneText
            // 
            txbPhoneText.BackColor = Color.Transparent;
            txbPhoneText.BackColorEx = Color.FromArgb(237, 235, 255);
            txbPhoneText.BorderRadius = 14;
            txbPhoneText.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbPhoneText.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbPhoneText.LabelText = "";
            txbPhoneText.Location = new Point(16, 271);
            txbPhoneText.MaxLength = 32767;
            txbPhoneText.Name = "txbPhoneText";
            txbPhoneText.PlaceholderText = "";
            txbPhoneText.RightToLeft = RightToLeft.Yes;
            txbPhoneText.Size = new Size(397, 46);
            txbPhoneText.TabIndex = 26;
            // 
            // bigLabel7
            // 
            bigLabel7.AutoSize = true;
            bigLabel7.BackColor = Color.Transparent;
            bigLabel7.Font = new Font("Segoe UI", 14.25F);
            bigLabel7.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel7.Location = new Point(502, 196);
            bigLabel7.Name = "bigLabel7";
            bigLabel7.Size = new Size(155, 25);
            bigLabel7.TabIndex = 25;
            bigLabel7.Text = "الرابط (https://...)  ";
            // 
            // txbUrlLink
            // 
            txbUrlLink.BackColor = Color.Transparent;
            txbUrlLink.BackColorEx = Color.FromArgb(237, 235, 255);
            txbUrlLink.BorderRadius = 14;
            txbUrlLink.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbUrlLink.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbUrlLink.LabelText = "";
            txbUrlLink.Location = new Point(16, 192);
            txbUrlLink.MaxLength = 32767;
            txbUrlLink.Name = "txbUrlLink";
            txbUrlLink.PlaceholderText = "";
            txbUrlLink.RightToLeft = RightToLeft.Yes;
            txbUrlLink.Size = new Size(397, 46);
            txbUrlLink.TabIndex = 24;
            // 
            // bigLabel5
            // 
            bigLabel5.AutoSize = true;
            bigLabel5.BackColor = Color.Transparent;
            bigLabel5.Font = new Font("Segoe UI", 14.25F);
            bigLabel5.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel5.Location = new Point(536, 114);
            bigLabel5.Name = "bigLabel5";
            bigLabel5.Size = new Size(111, 25);
            bigLabel5.TabIndex = 23;
            bigLabel5.Text = "نص زر الرابط";
            // 
            // txbUrlText
            // 
            txbUrlText.BackColor = Color.Transparent;
            txbUrlText.BackColorEx = Color.FromArgb(237, 235, 255);
            txbUrlText.BorderRadius = 14;
            txbUrlText.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbUrlText.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbUrlText.LabelText = "";
            txbUrlText.Location = new Point(16, 110);
            txbUrlText.MaxLength = 32767;
            txbUrlText.Name = "txbUrlText";
            txbUrlText.PlaceholderText = "";
            txbUrlText.RightToLeft = RightToLeft.Yes;
            txbUrlText.Size = new Size(397, 49);
            txbUrlText.TabIndex = 22;
            // 
            // bigLabel3
            // 
            bigLabel3.AutoSize = true;
            bigLabel3.BackColor = Color.Transparent;
            bigLabel3.Font = new Font("Segoe UI", 14.25F);
            bigLabel3.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel3.Location = new Point(527, 32);
            bigLabel3.Name = "bigLabel3";
            bigLabel3.Size = new Size(130, 25);
            bigLabel3.TabIndex = 21;
            bigLabel3.Text = "نص الزر السريع";
            // 
            // txbQuickReply
            // 
            txbQuickReply.BackColor = Color.Transparent;
            txbQuickReply.BackColorEx = Color.FromArgb(237, 235, 255);
            txbQuickReply.BorderRadius = 14;
            txbQuickReply.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbQuickReply.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbQuickReply.LabelText = "";
            txbQuickReply.Location = new Point(16, 26);
            txbQuickReply.MaxLength = 32767;
            txbQuickReply.Name = "txbQuickReply";
            txbQuickReply.PlaceholderText = "";
            txbQuickReply.RightToLeft = RightToLeft.Yes;
            txbQuickReply.Size = new Size(397, 51);
            txbQuickReply.TabIndex = 21;
            // 
            // txbImageUrl
            // 
            txbImageUrl.BackColor = Color.Transparent;
            txbImageUrl.BackColorEx = Color.FromArgb(237, 235, 255);
            txbImageUrl.BorderRadius = 14;
            txbImageUrl.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbImageUrl.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbImageUrl.LabelText = "";
            txbImageUrl.Location = new Point(42, 177);
            txbImageUrl.MaxLength = 32767;
            txbImageUrl.Name = "txbImageUrl";
            txbImageUrl.PlaceholderText = "";
            txbImageUrl.RightToLeft = RightToLeft.Yes;
            txbImageUrl.Size = new Size(493, 39);
            txbImageUrl.TabIndex = 19;
            // 
            // txbHeaderText
            // 
            txbHeaderText.BackColor = Color.Transparent;
            txbHeaderText.BackColorEx = Color.FromArgb(237, 235, 255);
            txbHeaderText.BorderRadius = 14;
            txbHeaderText.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbHeaderText.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbHeaderText.LabelText = "";
            txbHeaderText.Location = new Point(42, 74);
            txbHeaderText.MaxLength = 32767;
            txbHeaderText.Name = "txbHeaderText";
            txbHeaderText.PlaceholderText = "";
            txbHeaderText.RightToLeft = RightToLeft.Yes;
            txbHeaderText.Size = new Size(493, 39);
            txbHeaderText.TabIndex = 18;
            // 
            // cmbHeaderType
            // 
            cmbHeaderType.ArrowColor = Color.FromArgb(124, 111, 247);
            cmbHeaderType.BorderColor = Color.FromArgb(220, 215, 250);
            cmbHeaderType.DropdownBackColor = Color.White;
            cmbHeaderType.FocusBorderColor = Color.FromArgb(124, 111, 247);
            cmbHeaderType.Font = new Font("IBM Plex Sans Arabic", 12F);
            cmbHeaderType.ItemHoverColor = Color.FromArgb(237, 235, 255);
            cmbHeaderType.Items.Add("بدون هيدر");
            cmbHeaderType.Items.Add("نص");
            cmbHeaderType.Items.Add("صورة");
            cmbHeaderType.LabelText = "";
            cmbHeaderType.Location = new Point(559, 74);
            cmbHeaderType.Name = "cmbHeaderType";
            cmbHeaderType.PlaceholderText = "";
            cmbHeaderType.RightToLeft = RightToLeft.Yes;
            cmbHeaderType.SelectedIndex = -1;
            cmbHeaderType.SelectedItem = null;
            cmbHeaderType.Size = new Size(220, 40);
            cmbHeaderType.TabIndex = 16;
            cmbHeaderType.Text = "modernComboBox1";
            cmbHeaderType.TextColor = Color.FromArgb(40, 40, 70);
            cmbHeaderType.UsePlaceholder = true;
            // 
            // bigLabel6
            // 
            bigLabel6.AutoSize = true;
            bigLabel6.BackColor = Color.Transparent;
            bigLabel6.Font = new Font("Segoe UI", 20F);
            bigLabel6.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel6.Location = new Point(797, 74);
            bigLabel6.Name = "bigLabel6";
            bigLabel6.Size = new Size(133, 37);
            bigLabel6.TabIndex = 15;
            bigLabel6.Text = "نص الهيدر";
            // 
            // bigLabel4
            // 
            bigLabel4.AutoSize = true;
            bigLabel4.BackColor = Color.Transparent;
            bigLabel4.Font = new Font("Segoe UI", 20F);
            bigLabel4.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel4.Location = new Point(1250, 180);
            bigLabel4.Name = "bigLabel4";
            bigLabel4.Size = new Size(72, 37);
            bigLabel4.TabIndex = 13;
            bigLabel4.Text = "اللغة";
            // 
            // cmbLanguage
            // 
            cmbLanguage.ArrowColor = Color.FromArgb(124, 111, 247);
            cmbLanguage.BorderColor = Color.FromArgb(220, 215, 250);
            cmbLanguage.DropdownBackColor = Color.White;
            cmbLanguage.FocusBorderColor = Color.FromArgb(124, 111, 247);
            cmbLanguage.Font = new Font("Microsoft Sans Serif", 10F);
            cmbLanguage.ItemHoverColor = Color.FromArgb(237, 235, 255);
            cmbLanguage.Items.Add("ar");
            cmbLanguage.Items.Add("en");
            cmbLanguage.LabelText = "";
            cmbLanguage.Location = new Point(980, 177);
            cmbLanguage.Name = "cmbLanguage";
            cmbLanguage.PlaceholderText = "";
            cmbLanguage.RightToLeft = RightToLeft.Yes;
            cmbLanguage.SelectedIndex = -1;
            cmbLanguage.SelectedItem = null;
            cmbLanguage.Size = new Size(220, 40);
            cmbLanguage.TabIndex = 12;
            cmbLanguage.TextColor = Color.FromArgb(40, 40, 70);
            cmbLanguage.UsePlaceholder = true;
            // 
            // rtbxContent
            // 
            rtbxContent.BackColor = Color.Transparent;
            rtbxContent.Font = new Font("IBM Plex Sans Arabic", 12F);
            rtbxContent.LabelText = "";
            rtbxContent.Location = new Point(42, 345);
            rtbxContent.Name = "rtbxContent";
            rtbxContent.Size = new Size(677, 541);
            rtbxContent.TabIndex = 2;
            rtbxContent.Variables = (List<string>)resources.GetObject("rtbxContent.Variables");
            // 
            // btnAddTemplate
            // 
            btnAddTemplate.BackColor = Color.Transparent;
            btnAddTemplate.BorderRadius = 23;
            btnAddTemplate.Font = new Font("IBM Plex Sans Arabic", 12F, FontStyle.Bold);
            btnAddTemplate.Icon = Properties.Resources.plus;
            btnAddTemplate.Location = new Point(34, 1037);
            btnAddTemplate.Name = "btnAddTemplate";
            btnAddTemplate.RightToLeft = RightToLeft.Yes;
            btnAddTemplate.Size = new Size(162, 62);
            btnAddTemplate.TabIndex = 4;
            btnAddTemplate.Text = "اضافة ";
            btnAddTemplate.Click += btnAddTemplate_Click;
            // 
            // AddTemplateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1495, 1119);
            Controls.Add(btnAddTemplate);
            Controls.Add(pictureBox1);
            Controls.Add(picClose);
            Controls.Add(pnlContent);
            FormBorderStyle = FormBorderStyle.None;
            Name = "AddTemplateForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AddCustomerForm";
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            customPanel1.ResumeLayout(false);
            customPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private PictureBox picClose;
        private ReaLTaiizor.Controls.BigLabel bigLabel2;
        private ReaLTaiizor.Controls.BigLabel lblCustomerName;
        private PictureBox pictureBox1;
        private RichTextBox richTextBox1;
        private ReaLTaiizor.Controls.RichTextBoxEdit richTextBoxEdit1;
        private ReaLTaiizor.Controls.HopeRichTextBox hopeRichTextBox1;
        private ReaLTaiizor.Controls.BigLabel bigLabel1;
        private Controls.ModernTextBox txbImageUrl;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlContent;
        private Controls.ModernRichEditor rtbxContent;
        private Controls.ModernTextBox txbTemplateName;
        private Controls.ModernComboBox cmbCategory;
        private Controls.ModernButton btnAddTemplate;
        private Controls.ModernComboBox cmbLanguage;
        private ReaLTaiizor.Controls.BigLabel bigLabel6;
        private ReaLTaiizor.Controls.BigLabel bigLabel4;
        private Controls.ModernTextBox txbHeaderText;
        private Controls.ModernComboBox cmbHeaderType;
        private Car_Rental_System.CustomControls.CustomPanel customPanel1;
        private ReaLTaiizor.Controls.BigLabel bigLabel7;
        private Controls.ModernTextBox txbUrlLink;
        private ReaLTaiizor.Controls.BigLabel bigLabel5;
        private Controls.ModernTextBox txbUrlText;
        private ReaLTaiizor.Controls.BigLabel bigLabel3;
        private Controls.ModernTextBox txbQuickReply;
        private ReaLTaiizor.Controls.BigLabel bigLabel9;
        private Controls.ModernTextBox txbPhoneNumber;
        private ReaLTaiizor.Controls.BigLabel bigLabel8;
        private Controls.ModernTextBox txbPhoneText;
    }
}