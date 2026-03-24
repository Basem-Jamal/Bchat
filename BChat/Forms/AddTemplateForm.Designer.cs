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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTemplateForm));
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            picClose = new PictureBox();
            lblCustomerName = new ReaLTaiizor.Controls.BigLabel();
            pictureBox1 = new PictureBox();
            bigLabel1 = new ReaLTaiizor.Controls.BigLabel();
            bigLabel2 = new ReaLTaiizor.Controls.BigLabel();
            txbTemplateName = new BChat.Controls.ModernTextBox();
            cmbCategory = new BChat.Controls.ModernComboBox();
            guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            rtbxContent = new BChat.Controls.ModernRichEditor();
            btnAddTemplate = new BChat.Controls.ModernButton();
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
            lblCustomerName.Location = new Point(631, 66);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Size = new Size(141, 37);
            lblCustomerName.TabIndex = 2;
            lblCustomerName.Text = "اسم القالب";
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.add_Template;
            pictureBox1.Location = new Point(338, 43);
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
            bigLabel1.Location = new Point(602, 200);
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
            bigLabel2.Location = new Point(602, 466);
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
            txbTemplateName.Location = new Point(109, 66);
            txbTemplateName.MaxLength = 32767;
            txbTemplateName.Name = "txbTemplateName";
            txbTemplateName.PlaceholderText = "";
            txbTemplateName.RightToLeft = RightToLeft.Yes;
            txbTemplateName.Size = new Size(460, 70);
            txbTemplateName.TabIndex = 1;
            // 
            // cmbCategory
            // 
            cmbCategory.BackColor = Color.Transparent;
            cmbCategory.BorderRadius = 14;
            cmbCategory.Font = new Font("IBM Plex Sans Arabic", 12F);
            cmbCategory.LabelText = "";
            cmbCategory.Location = new Point(349, 466);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.PlaceholderText = "";
            cmbCategory.RightToLeft = RightToLeft.Yes;
            cmbCategory.SelectedIndex = -1;
            cmbCategory.SelectedValue = null;
            cmbCategory.Size = new Size(220, 70);
            cmbCategory.TabIndex = 3;
            cmbCategory.Text = "modernComboBox1";
            // 
            // guna2CustomGradientPanel1
            // 
            guna2CustomGradientPanel1.Controls.Add(rtbxContent);
            guna2CustomGradientPanel1.Controls.Add(cmbCategory);
            guna2CustomGradientPanel1.Controls.Add(txbTemplateName);
            guna2CustomGradientPanel1.Controls.Add(lblCustomerName);
            guna2CustomGradientPanel1.Controls.Add(bigLabel2);
            guna2CustomGradientPanel1.Controls.Add(bigLabel1);
            guna2CustomGradientPanel1.CustomizableEdges = customizableEdges1;
            guna2CustomGradientPanel1.Location = new Point(12, 169);
            guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            guna2CustomGradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2CustomGradientPanel1.Size = new Size(788, 569);
            guna2CustomGradientPanel1.TabIndex = 0;
            // 
            // rtbxContent
            // 
            rtbxContent.BackColor = Color.Transparent;
            rtbxContent.Font = new Font("IBM Plex Sans Arabic", 12F);
            rtbxContent.LabelText = "";
            rtbxContent.Location = new Point(109, 200);
            rtbxContent.Name = "rtbxContent";
            rtbxContent.Size = new Size(460, 220);
            rtbxContent.TabIndex = 2;
            rtbxContent.Variables = (List<string>)resources.GetObject("rtbxContent.Variables");
            // 
            // btnAddTemplate
            // 
            btnAddTemplate.BackColor = Color.Transparent;
            btnAddTemplate.BorderRadius = 23;
            btnAddTemplate.Font = new Font("IBM Plex Sans Arabic", 12F, FontStyle.Bold);
            btnAddTemplate.Icon = Properties.Resources.plus;
            btnAddTemplate.Location = new Point(26, 762);
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
            ClientSize = new Size(812, 851);
            Controls.Add(btnAddTemplate);
            Controls.Add(pictureBox1);
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
        private Controls.ModernButton btnAddTemplate;
    }
}