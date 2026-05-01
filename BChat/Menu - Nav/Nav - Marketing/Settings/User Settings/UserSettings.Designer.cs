namespace BChat.Menu___Nav.Nav___Marketing.Settings.User_Settings
{
    partial class UserSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserSettings));
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            pictureBox1 = new PictureBox();
            label1 = new Label();
            pnlHeader = new Car_Rental_System.CustomControls.CustomPanel();
            picClose = new PictureBox();
            gradientPanel3 = new GradientPanel();
            pnlContent = new Car_Rental_System.CustomControls.CustomPanel();
            btnAddUser = new BChat.Controls.ModernButton();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picClose).BeginInit();
            SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.AnimateWindow = true;
            guna2BorderlessForm1.BorderRadius = 70;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.permission22;
            pictureBox1.Location = new Point(543, 82);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(50, 41);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
            label1.ForeColor = Color.DimGray;
            label1.Location = new Point(390, 89);
            label1.Name = "label1";
            label1.Size = new Size(146, 25);
            label1.TabIndex = 1;
            label1.Text = "صلاحية المستخدم";
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.White;
            pnlHeader.BackColorEx = Color.White;
            pnlHeader.BorderColor = Color.LightGray;
            pnlHeader.BorderRadius = 10;
            pnlHeader.BorderThickness = 1;
            pnlHeader.Controls.Add(picClose);
            pnlHeader.Font = new Font("Segoe UI", 10F);
            pnlHeader.ForeColor = Color.Black;
            pnlHeader.Location = new Point(18, -8);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            pnlHeader.ShadowSize = 6;
            pnlHeader.Size = new Size(893, 62);
            pnlHeader.TabIndex = 25;
            pnlHeader.UseShadow = true;
            // 
            // picClose
            // 
            picClose.BackColor = Color.Transparent;
            picClose.Cursor = Cursors.Hand;
            picClose.Image = (Image)resources.GetObject("picClose.Image");
            picClose.Location = new Point(837, 8);
            picClose.Name = "picClose";
            picClose.Size = new Size(40, 40);
            picClose.SizeMode = PictureBoxSizeMode.Zoom;
            picClose.TabIndex = 2;
            picClose.TabStop = false;
            picClose.Click += picClose_Click;
            // 
            // gradientPanel3
            // 
            gradientPanel3.BackColor = Color.Transparent;
            gradientPanel3.GlassBorderAlpha = 122;
            gradientPanel3.GradientEndColor = Color.Purple;
            gradientPanel3.GradientMidColor = Color.FromArgb(85, 69, 205);
            gradientPanel3.GradientStartColor = SystemColors.ActiveCaptionText;
            gradientPanel3.HoverGlow = false;
            gradientPanel3.HoverGlowColor = Color.Transparent;
            gradientPanel3.HoverGlowRadius = 0;
            gradientPanel3.Location = new Point(29, 736);
            gradientPanel3.Name = "gradientPanel3";
            gradientPanel3.Padding = new Padding(6, 6, 6, 8);
            gradientPanel3.ShadowColor = Color.Transparent;
            gradientPanel3.ShadowRadius = 0;
            gradientPanel3.ShowGlassBorder = false;
            gradientPanel3.ShowShadow = false;
            gradientPanel3.ShowShimmer = false;
            gradientPanel3.Size = new Size(882, 61);
            gradientPanel3.TabIndex = 25;
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.White;
            pnlContent.BackColorEx = Color.White;
            pnlContent.BorderColor = Color.LightGray;
            pnlContent.BorderRadius = 10;
            pnlContent.BorderThickness = 1;
            pnlContent.Font = new Font("Segoe UI", 10F);
            pnlContent.ForeColor = Color.Black;
            pnlContent.Location = new Point(29, 149);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            pnlContent.ShadowSize = 6;
            pnlContent.Size = new Size(899, 564);
            pnlContent.TabIndex = 26;
            pnlContent.UseShadow = true;
            // 
            // btnAddUser
            // 
            btnAddUser.BackColor = Color.Transparent;
            btnAddUser.BorderRadius = 10;
            btnAddUser.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnAddUser.Icon = Properties.Resources.plus;
            btnAddUser.Location = new Point(747, 73);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.RightToLeft = RightToLeft.Yes;
            btnAddUser.Size = new Size(181, 50);
            btnAddUser.TabIndex = 18;
            btnAddUser.Text = "اضافة مستخدم جديد";
            btnAddUser.Click += btnAddUser_Click;
            // 
            // UserSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(956, 774);
            Controls.Add(btnAddUser);
            Controls.Add(label1);
            Controls.Add(gradientPanel3);
            Controls.Add(pictureBox1);
            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.None;
            Name = "UserSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "UserSettings";
            Load += UserSettings_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private PictureBox picClose;
        private Car_Rental_System.CustomControls.CustomPanel pnlHeader;
        private GradientPanel gradientPanel3;
        private Car_Rental_System.CustomControls.CustomPanel pnlContent;
        private Label label1;
        private PictureBox pictureBox1;
        private Controls.ModernButton btnAddUser;
    }
}