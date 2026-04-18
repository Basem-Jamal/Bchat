namespace BChat.Forms
{
    partial class fmAddGroup
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
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            adColorPicker = new AdvancedColorPicker();
            bigLabel2 = new ReaLTaiizor.Controls.BigLabel();
            bigLabel1 = new ReaLTaiizor.Controls.BigLabel();
            btnUplaodFileIcon = new BChat.Controls.ModernNavButton();
            lblSelectedBG = new Label();
            lblFileTextIcon = new Label();
            lblCustomerPhone = new ReaLTaiizor.Controls.BigLabel();
            txbGroupDescription = new BChat.Controls.ModernTextBox();
            txbGroupName = new BChat.Controls.ModernTextBox();
            lblCustomerName = new ReaLTaiizor.Controls.BigLabel();
            pictureBox1 = new PictureBox();
            picClose = new PictureBox();
            btnAddOrEditCustomerGroups = new BChat.Controls.ModernButton();
            guna2CustomGradientPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picClose).BeginInit();
            SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.BorderRadius = 20;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2CustomGradientPanel1
            // 
            guna2CustomGradientPanel1.Controls.Add(adColorPicker);
            guna2CustomGradientPanel1.Controls.Add(bigLabel2);
            guna2CustomGradientPanel1.Controls.Add(bigLabel1);
            guna2CustomGradientPanel1.Controls.Add(btnUplaodFileIcon);
            guna2CustomGradientPanel1.Controls.Add(lblSelectedBG);
            guna2CustomGradientPanel1.Controls.Add(lblFileTextIcon);
            guna2CustomGradientPanel1.Controls.Add(lblCustomerPhone);
            guna2CustomGradientPanel1.Controls.Add(txbGroupDescription);
            guna2CustomGradientPanel1.Controls.Add(txbGroupName);
            guna2CustomGradientPanel1.Controls.Add(lblCustomerName);
            guna2CustomGradientPanel1.CustomizableEdges = customizableEdges1;
            guna2CustomGradientPanel1.Location = new Point(12, 169);
            guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            guna2CustomGradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2CustomGradientPanel1.Size = new Size(788, 683);
            guna2CustomGradientPanel1.TabIndex = 10;
            // 
            // adColorPicker
            // 
            adColorPicker.Hex = "4285F4";
            adColorPicker.Location = new Point(238, 401);
            adColorPicker.Name = "adColorPicker";
            adColorPicker.SelectedColor = Color.FromArgb(66, 133, 244);
            adColorPicker.ShowAlpha = false;
            adColorPicker.ShowHex = true;
            adColorPicker.ShowRgb = true;
            adColorPicker.Size = new Size(325, 285);
            adColorPicker.TabIndex = 24;
            // 
            // bigLabel2
            // 
            bigLabel2.AutoSize = true;
            bigLabel2.BackColor = Color.Transparent;
            bigLabel2.Font = new Font("IBM Plex Sans Arabic", 16F);
            bigLabel2.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel2.Location = new Point(616, 480);
            bigLabel2.Name = "bigLabel2";
            bigLabel2.Size = new Size(149, 38);
            bigLabel2.TabIndex = 23;
            bigLabel2.Text = "اختر لون الخلفية";
            // 
            // bigLabel1
            // 
            bigLabel1.AutoSize = true;
            bigLabel1.BackColor = Color.Transparent;
            bigLabel1.Font = new Font("IBM Plex Sans Arabic", 16F);
            bigLabel1.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel1.Location = new Point(616, 349);
            bigLabel1.Name = "bigLabel1";
            bigLabel1.Size = new Size(101, 38);
            bigLabel1.TabIndex = 22;
            bigLabel1.Text = "اختر ايقونة";
            // 
            // btnUplaodFileIcon
            // 
            btnUplaodFileIcon.ActiveBackground = Color.Thistle;
            btnUplaodFileIcon.ActiveBarColor = Color.Thistle;
            btnUplaodFileIcon.ActiveBarFullHeight = true;
            btnUplaodFileIcon.ActiveBarPadding = 0;
            btnUplaodFileIcon.ActiveBarWidth = 4;
            btnUplaodFileIcon.ActiveTextColor = Color.White;
            btnUplaodFileIcon.BackColor = Color.Transparent;
            btnUplaodFileIcon.BaseBackground = Color.FromArgb(124, 111, 247);
            btnUplaodFileIcon.BorderRadius = 8;
            btnUplaodFileIcon.CardPadding = 0;
            btnUplaodFileIcon.ContentPadding = 12;
            btnUplaodFileIcon.Font = new Font("IBM Plex Sans Arabic", 9.5F, FontStyle.Bold);
            btnUplaodFileIcon.HoverBackground = Color.Transparent;
            btnUplaodFileIcon.HoverTextColor = Color.White;
            btnUplaodFileIcon.Icon = null;
            btnUplaodFileIcon.IconSize = 20;
            btnUplaodFileIcon.IsActive = false;
            btnUplaodFileIcon.Location = new Point(355, 349);
            btnUplaodFileIcon.Name = "btnUplaodFileIcon";
            btnUplaodFileIcon.NormalTextColor = Color.White;
            btnUplaodFileIcon.RightToLeft = RightToLeft.Yes;
            btnUplaodFileIcon.Size = new Size(220, 46);
            btnUplaodFileIcon.TabIndex = 18;
            btnUplaodFileIcon.Text = "ارفق الملف";
            btnUplaodFileIcon.UseActiveEffect = true;
            btnUplaodFileIcon.UseHoverEffect = true;
            btnUplaodFileIcon.Click += btnUplaodFileIcon_Click;
            // 
            // lblSelectedBG
            // 
            lblSelectedBG.AutoSize = true;
            lblSelectedBG.Font = new Font("Segoe UI", 12F);
            lblSelectedBG.Location = new Point(23, 630);
            lblSelectedBG.Name = "lblSelectedBG";
            lblSelectedBG.Size = new Size(52, 21);
            lblSelectedBG.TabIndex = 21;
            lblSelectedBG.Text = "label1";
            // 
            // lblFileTextIcon
            // 
            lblFileTextIcon.AutoSize = true;
            lblFileTextIcon.Font = new Font("Segoe UI", 12F);
            lblFileTextIcon.Location = new Point(23, 366);
            lblFileTextIcon.Name = "lblFileTextIcon";
            lblFileTextIcon.Size = new Size(52, 21);
            lblFileTextIcon.TabIndex = 20;
            lblFileTextIcon.Text = "label1";
            // 
            // lblCustomerPhone
            // 
            lblCustomerPhone.AutoSize = true;
            lblCustomerPhone.BackColor = Color.Transparent;
            lblCustomerPhone.Font = new Font("IBM Plex Sans Arabic", 16F);
            lblCustomerPhone.ForeColor = Color.FromArgb(80, 80, 80);
            lblCustomerPhone.Location = new Point(616, 224);
            lblCustomerPhone.Name = "lblCustomerPhone";
            lblCustomerPhone.Size = new Size(153, 38);
            lblCustomerPhone.TabIndex = 7;
            lblCustomerPhone.Text = "وصف المجموعة";
            // 
            // txbGroupDescription
            // 
            txbGroupDescription.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txbGroupDescription.BackColor = Color.Transparent;
            txbGroupDescription.BackColorEx = Color.FromArgb(237, 235, 255);
            txbGroupDescription.BorderRadius = 14;
            txbGroupDescription.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbGroupDescription.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbGroupDescription.LabelText = "";
            txbGroupDescription.Location = new Point(115, 214);
            txbGroupDescription.MaxLength = 32767;
            txbGroupDescription.Name = "txbGroupDescription";
            txbGroupDescription.PlaceholderText = "";
            txbGroupDescription.RightToLeft = RightToLeft.Yes;
            txbGroupDescription.Size = new Size(460, 95);
            txbGroupDescription.TabIndex = 2;
            // 
            // txbGroupName
            // 
            txbGroupName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txbGroupName.BackColor = Color.Transparent;
            txbGroupName.BackColorEx = Color.FromArgb(237, 235, 255);
            txbGroupName.BorderRadius = 14;
            txbGroupName.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbGroupName.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbGroupName.LabelText = "";
            txbGroupName.Location = new Point(115, 93);
            txbGroupName.MaxLength = 32767;
            txbGroupName.Name = "txbGroupName";
            txbGroupName.PlaceholderText = "";
            txbGroupName.RightToLeft = RightToLeft.Yes;
            txbGroupName.Size = new Size(460, 58);
            txbGroupName.TabIndex = 1;
            // 
            // lblCustomerName
            // 
            lblCustomerName.AutoSize = true;
            lblCustomerName.BackColor = Color.Transparent;
            lblCustomerName.Font = new Font("IBM Plex Sans Arabic", 16F);
            lblCustomerName.ForeColor = Color.FromArgb(80, 80, 80);
            lblCustomerName.Location = new Point(616, 102);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Size = new Size(138, 38);
            lblCustomerName.TabIndex = 2;
            lblCustomerName.Text = "اسم المجموعة";
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.add_group;
            pictureBox1.Location = new Point(338, 43);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(135, 98);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            // 
            // picClose
            // 
            picClose.Cursor = Cursors.Hand;
            picClose.Image = Properties.Resources.close;
            picClose.Location = new Point(741, 12);
            picClose.Name = "picClose";
            picClose.Size = new Size(56, 49);
            picClose.SizeMode = PictureBoxSizeMode.Zoom;
            picClose.TabIndex = 11;
            picClose.TabStop = false;
            picClose.Click += picClose_Click;
            // 
            // btnAddOrEditCustomerGroups
            // 
            btnAddOrEditCustomerGroups.BackColor = Color.Transparent;
            btnAddOrEditCustomerGroups.BorderRadius = 23;
            btnAddOrEditCustomerGroups.Font = new Font("IBM Plex Sans Arabic", 12F, FontStyle.Bold);
            btnAddOrEditCustomerGroups.Icon = Properties.Resources.plus;
            btnAddOrEditCustomerGroups.Location = new Point(12, 871);
            btnAddOrEditCustomerGroups.Name = "btnAddOrEditCustomerGroups";
            btnAddOrEditCustomerGroups.RightToLeft = RightToLeft.Yes;
            btnAddOrEditCustomerGroups.Size = new Size(162, 62);
            btnAddOrEditCustomerGroups.TabIndex = 13;
            btnAddOrEditCustomerGroups.Text = "اضافة ";
            btnAddOrEditCustomerGroups.Click += btnAddOrEditCustomerGroups_Click;
            // 
            // fmAddGroup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(818, 955);
            Controls.Add(btnAddOrEditCustomerGroups);
            Controls.Add(pictureBox1);
            Controls.Add(picClose);
            Controls.Add(guna2CustomGradientPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "fmAddGroup";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AddGroup";
            Load += fmAddGroup_Load;
            guna2CustomGradientPanel1.ResumeLayout(false);
            guna2CustomGradientPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private ReaLTaiizor.Controls.BigLabel lblCustomerPhone;
        private Controls.ModernTextBox txbGroupDescription;
        private Controls.ModernTextBox txbGroupName;
        private ReaLTaiizor.Controls.BigLabel lblCustomerName;
        private PictureBox pictureBox1;
        private PictureBox picClose;
        private Controls.ModernButton btnAddOrEditCustomerGroups;
        private Controls.ModernNavButton btnUplaodFileIcon;
        private Label lblFileTextIcon;
        private ReaLTaiizor.Controls.BigLabel bigLabel2;
        private ReaLTaiizor.Controls.BigLabel bigLabel1;
        private Label lblSelectedBG;
        private AdvancedColorPicker adColorPicker;
    }
}