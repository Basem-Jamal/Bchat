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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTemplateForm));
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            picClose = new PictureBox();
            lblCustomerName = new ReaLTaiizor.Controls.BigLabel();
            btnAddTemplate = new Guna.UI2.WinForms.Guna2Button();
            pictureBox1 = new PictureBox();
            bigLabel1 = new ReaLTaiizor.Controls.BigLabel();
            bigLabel2 = new ReaLTaiizor.Controls.BigLabel();
            txbTemplateName = new BChat.Controls.ModernTextBox();
            cmbCategory = new BChat.Controls.ModernComboBox();
            guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            modernButton1 = new BChat.Controls.ModernButton();
            rtbxContent = new BChat.Controls.ModernRichEditor();
            modernFabButton1 = new BChat.Controls.ModernFabButton();
            modernNavButton1 = new BChat.Controls.ModernNavButton();
            modernPaginationButton1 = new BChat.Controls.ModernPaginationButton();
            modernPanel1 = new ModernPanel();
            modernToolbarButton1 = new BChat.Controls.ModernToolbarButton();
            ((System.ComponentModel.ISupportInitialize)picClose).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            guna2CustomGradientPanel1.SuspendLayout();
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
            picClose.Location = new Point(718, 6);
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
            lblCustomerName.Location = new Point(621, 76);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Size = new Size(141, 37);
            lblCustomerName.TabIndex = 2;
            lblCustomerName.Text = "اسم القالب";
            // 
            // btnAddTemplate
            // 
            btnAddTemplate.BorderRadius = 20;
            btnAddTemplate.Cursor = Cursors.Hand;
            btnAddTemplate.CustomizableEdges = customizableEdges1;
            btnAddTemplate.DisabledState.BorderColor = Color.DarkGray;
            btnAddTemplate.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAddTemplate.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAddTemplate.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAddTemplate.Font = new Font("Segoe UI", 13F);
            btnAddTemplate.ForeColor = Color.White;
            btnAddTemplate.Location = new Point(45, 775);
            btnAddTemplate.Name = "btnAddTemplate";
            btnAddTemplate.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnAddTemplate.Size = new Size(180, 45);
            btnAddTemplate.TabIndex = 6;
            btnAddTemplate.Text = "اضافة";
            btnAddTemplate.Click += btnAddTemplate_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.add_f;
            pictureBox1.Location = new Point(354, 45);
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
            bigLabel1.Location = new Point(602, 217);
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
            bigLabel2.Location = new Point(592, 466);
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
            txbTemplateName.Font = new Font("IBM Plex Sans Arabic", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txbTemplateName.LabelText = "";
            txbTemplateName.Location = new Point(326, 66);
            txbTemplateName.MaxLength = 32767;
            txbTemplateName.Name = "txbTemplateName";
            txbTemplateName.PlaceholderText = "";
            txbTemplateName.RightToLeft = RightToLeft.Yes;
            txbTemplateName.Size = new Size(220, 70);
            txbTemplateName.TabIndex = 12;
            // 
            // cmbCategory
            // 
            cmbCategory.BackColor = Color.Transparent;
            cmbCategory.BorderRadius = 14;
            cmbCategory.Font = new Font("IBM Plex Sans Arabic SemiBold", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cmbCategory.LabelText = "";
            cmbCategory.Location = new Point(326, 447);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.PlaceholderText = "";
            cmbCategory.RightToLeft = RightToLeft.Yes;
            cmbCategory.SelectedIndex = -1;
            cmbCategory.SelectedValue = null;
            cmbCategory.Size = new Size(220, 70);
            cmbCategory.TabIndex = 13;
            cmbCategory.Text = "modernComboBox1";
            // 
            // guna2CustomGradientPanel1
            // 
            guna2CustomGradientPanel1.Controls.Add(modernToolbarButton1);
            guna2CustomGradientPanel1.Controls.Add(modernButton1);
            guna2CustomGradientPanel1.Controls.Add(rtbxContent);
            guna2CustomGradientPanel1.Controls.Add(cmbCategory);
            guna2CustomGradientPanel1.Controls.Add(txbTemplateName);
            guna2CustomGradientPanel1.Controls.Add(lblCustomerName);
            guna2CustomGradientPanel1.Controls.Add(bigLabel2);
            guna2CustomGradientPanel1.Controls.Add(bigLabel1);
            guna2CustomGradientPanel1.CustomizableEdges = customizableEdges3;
            guna2CustomGradientPanel1.Location = new Point(12, 169);
            guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            guna2CustomGradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2CustomGradientPanel1.Size = new Size(788, 569);
            guna2CustomGradientPanel1.TabIndex = 0;
            // 
            // modernButton1
            // 
            modernButton1.BackColor = Color.Transparent;
            modernButton1.Font = new Font("IBM Plex Sans Arabic", 10F, FontStyle.Bold);
            modernButton1.Icon = null;
            modernButton1.Location = new Point(655, 362);
            modernButton1.Name = "modernButton1";
            modernButton1.RightToLeft = RightToLeft.Yes;
            modernButton1.Size = new Size(94, 85);
            modernButton1.TabIndex = 15;
            modernButton1.Text = "Chat";
            // 
            // rtbxContent
            // 
            rtbxContent.BackColor = Color.Transparent;
            rtbxContent.LabelText = "محتوى الرسالة";
            rtbxContent.Location = new Point(109, 200);
            rtbxContent.Name = "rtbxContent";
            rtbxContent.Size = new Size(460, 220);
            rtbxContent.TabIndex = 14;
            rtbxContent.Variables = (List<string>)resources.GetObject("rtbxContent.Variables");
            // 
            // modernFabButton1
            // 
            modernFabButton1.BackColor = Color.Transparent;
            modernFabButton1.FabColor = Color.FromArgb(124, 111, 247);
            modernFabButton1.Font = new Font("IBM Plex Sans Arabic", 10F, FontStyle.Bold);
            modernFabButton1.Icon = null;
            modernFabButton1.Location = new Point(71, 98);
            modernFabButton1.Name = "modernFabButton1";
            modernFabButton1.Size = new Size(64, 64);
            modernFabButton1.TabIndex = 9;
            modernFabButton1.Text = "modernFabButton1";
            // 
            // modernNavButton1
            // 
            modernNavButton1.BackColor = Color.Transparent;
            modernNavButton1.Font = new Font("IBM Plex Sans Arabic", 9.5F, FontStyle.Bold);
            modernNavButton1.Icon = null;
            modernNavButton1.Location = new Point(208, 57);
            modernNavButton1.Name = "modernNavButton1";
            modernNavButton1.RightToLeft = RightToLeft.Yes;
            modernNavButton1.Size = new Size(220, 46);
            modernNavButton1.TabIndex = 10;
            modernNavButton1.Text = "modernNavButton1";
            // 
            // modernPaginationButton1
            // 
            modernPaginationButton1.BackColor = Color.Transparent;
            modernPaginationButton1.Font = new Font("IBM Plex Sans Arabic", 9F, FontStyle.Bold);
            modernPaginationButton1.Location = new Point(596, 86);
            modernPaginationButton1.Name = "modernPaginationButton1";
            modernPaginationButton1.Size = new Size(178, 76);
            modernPaginationButton1.TabIndex = 11;
            modernPaginationButton1.Text = "modernPaginationButton1";
            // 
            // modernPanel1
            // 
            modernPanel1.BorderColor = Color.FromArgb(230, 230, 230);
            modernPanel1.BorderRadius = 20;
            modernPanel1.BorderThickness = 1;
            modernPanel1.Font = new Font("Segoe UI", 9F);
            modernPanel1.ForeColor = Color.Black;
            modernPanel1.GlassTransparency = 30;
            modernPanel1.GlowColor = Color.FromArgb(100, 0, 120, 255);
            modernPanel1.GradientColor1 = Color.White;
            modernPanel1.GradientColor2 = Color.FromArgb(245, 247, 250);
            modernPanel1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            modernPanel1.Location = new Point(32, 158);
            modernPanel1.Name = "modernPanel1";
            modernPanel1.ShadowBlur = 8;
            modernPanel1.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            modernPanel1.ShadowDepth = 10;
            modernPanel1.ShadowOffsetX = 3;
            modernPanel1.ShadowOffsetY = 3;
            modernPanel1.ShadowSize = 10;
            modernPanel1.ShrinkContentWithShadow = true;
            modernPanel1.Size = new Size(300, 180);
            modernPanel1.TabIndex = 16;
            modernPanel1.UseGlass = true;
            modernPanel1.UseGlow = false;
            modernPanel1.UseGradient = true;
            modernPanel1.UseShadow = true;
            // 
            // modernToolbarButton1
            // 
            modernToolbarButton1.BackColor = Color.Transparent;
            modernToolbarButton1.Font = new Font("IBM Plex Sans Arabic", 10F, FontStyle.Bold);
            modernToolbarButton1.Icon = null;
            modernToolbarButton1.Location = new Point(495, 25);
            modernToolbarButton1.Name = "modernToolbarButton1";
            modernToolbarButton1.Size = new Size(225, 48);
            modernToolbarButton1.TabIndex = 16;
            modernToolbarButton1.Text = "modernToolbarButton1";
            // 
            // AddTemplateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(812, 851);
            Controls.Add(modernPanel1);
            Controls.Add(modernPaginationButton1);
            Controls.Add(modernNavButton1);
            Controls.Add(modernFabButton1);
            Controls.Add(pictureBox1);
            Controls.Add(btnAddTemplate);
            Controls.Add(picClose);
            Controls.Add(guna2CustomGradientPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "AddTemplateForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AddCustomerForm";
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            guna2CustomGradientPanel1.ResumeLayout(false);
            guna2CustomGradientPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private PictureBox picClose;
        private ReaLTaiizor.Controls.BigLabel bigLabel3;
        private ReaLTaiizor.Controls.BigLabel bigLabel2;
        private ReaLTaiizor.Controls.BigLabel lblCustomerName;
        private Guna.UI2.WinForms.Guna2Button btnAddTemplate;
        private PictureBox pictureBox1;
        private RichTextBox richTextBox1;
        private ReaLTaiizor.Controls.RichTextBoxEdit richTextBoxEdit1;
        private ReaLTaiizor.Controls.HopeRichTextBox hopeRichTextBox1;
        private ReaLTaiizor.Controls.BigLabel bigLabel1;
        private Controls.ModernTextBox modernTextBox1;
        private Controls.ModernComboBox modernComboBox1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private Controls.ModernRichEditor rtbxContent;
        private Controls.ModernTextBox txbTemplateName;
        private Controls.ModernComboBox cmbCategory;
        private Controls.ModernButton modernButton1;
        private ModernPanel modernPanel1;
        private Controls.ModernPaginationButton modernPaginationButton1;
        private Controls.ModernNavButton modernNavButton1;
        private Controls.ModernFabButton modernFabButton1;
        private Controls.ModernToolbarButton modernToolbarButton1;
    }
}