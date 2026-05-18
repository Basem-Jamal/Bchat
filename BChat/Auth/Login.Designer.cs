

namespace BChat.Auth
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            picClose = new PictureBox();
            txtPassword = new BChat.Controls.ModernTextBox();
            lbl2 = new ReaLTaiizor.Controls.BigLabel();
            lbl1 = new ReaLTaiizor.Controls.BigLabel();
            btnLogin = new BChat.Controls.ModernButton();
            pictureBox1 = new PictureBox();
            txtEmail = new BChat.Controls.ModernTextBox();
            gradientPanel3 = new GradientPanel();
            gradientPanel1 = new GradientPanel();
            pnlContent = new GradientPanel();
            ((System.ComponentModel.ISupportInitialize)picClose).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            gradientPanel3.SuspendLayout();
            gradientPanel1.SuspendLayout();
            pnlContent.SuspendLayout();
            SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.AnimateWindow = true;
            guna2BorderlessForm1.BorderRadius = 45;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // picClose
            // 
            picClose.BackColor = Color.Transparent;
            picClose.Cursor = Cursors.Hand;
            picClose.Image = Properties.Resources.close;
            picClose.Location = new Point(757, 33);
            picClose.Name = "picClose";
            picClose.Size = new Size(56, 49);
            picClose.SizeMode = PictureBoxSizeMode.Zoom;
            picClose.TabIndex = 29;
            picClose.TabStop = false;
            picClose.Click += picClose_Click;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.Transparent;
            txtPassword.BackColorEx = Color.FromArgb(237, 235, 255);
            txtPassword.BorderColor = Color.FromArgb(220, 215, 250);
            txtPassword.BorderRadius = 14;
            txtPassword.Direction = BChat.Controls.TextDirection.LTR;
            txtPassword.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txtPassword.Font = new Font("Microsoft Sans Serif", 12F);
            txtPassword.LabelText = "";
            txtPassword.Location = new Point(246, 298);
            txtPassword.MaxLength = 32767;
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Password";
            txtPassword.RightToLeft = RightToLeft.No;
            txtPassword.Size = new Size(367, 51);
            txtPassword.TabIndex = 2;
            txtPassword.TextPadding = 14;
            txtPassword.UsePasswordChar = true;
            // 
            // lbl2
            // 
            lbl2.AutoSize = true;
            lbl2.BackColor = Color.Transparent;
            lbl2.Font = new Font("Microsoft Sans Serif", 16F);
            lbl2.ForeColor = Color.FromArgb(80, 80, 80);
            lbl2.Location = new Point(74, 312);
            lbl2.Name = "lbl2";
            lbl2.Size = new Size(108, 26);
            lbl2.TabIndex = 31;
            lbl2.Text = "Passwrod";
            // 
            // lbl1
            // 
            lbl1.AutoSize = true;
            lbl1.BackColor = Color.Transparent;
            lbl1.Font = new Font("Microsoft Sans Serif", 16F);
            lbl1.ForeColor = Color.FromArgb(80, 80, 80);
            lbl1.Location = new Point(99, 185);
            lbl1.Name = "lbl1";
            lbl1.Size = new Size(68, 26);
            lbl1.TabIndex = 30;
            lbl1.Text = "Email";
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.Transparent;
            btnLogin.BorderColor = Color.FromArgb(200, 196, 214);
            btnLogin.BorderHoverColor = Color.FromArgb(85, 69, 205);
            btnLogin.BorderRadius = 15;
            btnLogin.CustomBackground = Color.FromArgb(85, 69, 205);
            btnLogin.CustomBackgroundHover = Color.FromArgb(63, 43, 184);
            btnLogin.CustomForeground = Color.White;
            btnLogin.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnLogin.Icon = (Image)resources.GetObject("btnLogin.Icon");
            btnLogin.Location = new Point(345, 404);
            btnLogin.Name = "btnLogin";
            btnLogin.RightToLeft = RightToLeft.Yes;
            btnLogin.Size = new Size(145, 55);
            btnLogin.TabIndex = 3;
            btnLogin.Text = "Loign";
            btnLogin.UseCustomColors = true;
            btnLogin.Variant = BChat.Controls.ButtonVariant.Secondary;
            btnLogin.Click += btnLogin_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.Logo_Blue1;
            pictureBox1.Location = new Point(345, 33);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(114, 109);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 30;
            pictureBox1.TabStop = false;
            // 
            // txtEmail
            // 
            txtEmail.BackColor = Color.Transparent;
            txtEmail.BackColorEx = Color.FromArgb(237, 235, 255);
            txtEmail.BorderColor = Color.FromArgb(220, 215, 250);
            txtEmail.BorderRadius = 14;
            txtEmail.Direction = BChat.Controls.TextDirection.LTR;
            txtEmail.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txtEmail.Font = new Font("Microsoft Sans Serif", 12F);
            txtEmail.LabelText = "";
            txtEmail.Location = new Point(246, 174);
            txtEmail.MaxLength = 32767;
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Email";
            txtEmail.RightToLeft = RightToLeft.Yes;
            txtEmail.Size = new Size(367, 51);
            txtEmail.TabIndex = 1;
            txtEmail.TextPadding = 14;
            txtEmail.UsePasswordChar = false;
            // 
            // gradientPanel3
            // 
            gradientPanel3.BackColor = Color.Transparent;
            gradientPanel3.Controls.Add(picClose);
            gradientPanel3.CornerRadius = 15;
            gradientPanel3.CornerRadiusEx.BottomLeft = 15;
            gradientPanel3.CornerRadiusEx.BottomRight = 15;
            gradientPanel3.CornerRadiusEx.TopLeft = 15;
            gradientPanel3.CornerRadiusEx.TopRight = 15;
            gradientPanel3.GradientEndColor = Color.White;
            gradientPanel3.GradientMidColor = Color.Violet;
            gradientPanel3.GradientStartColor = Color.White;
            gradientPanel3.HoverGlowColor = Color.Transparent;
            gradientPanel3.HoverGlowRadius = 0;
            gradientPanel3.Location = new Point(-7, -26);
            gradientPanel3.Name = "gradientPanel3";
            gradientPanel3.Padding = new Padding(6);
            gradientPanel3.ShadowColor = Color.Transparent;
            gradientPanel3.ShadowOffsetY = 0;
            gradientPanel3.ShadowRadius = 0;
            gradientPanel3.ShowGlassBorder = false;
            gradientPanel3.ShowShadow = false;
            gradientPanel3.ShowShimmer = false;
            gradientPanel3.Size = new Size(870, 91);
            gradientPanel3.TabIndex = 30;
            gradientPanel3.UseBlur = true;
            // 
            // gradientPanel1
            // 
            gradientPanel1.BackColor = Color.Transparent;
            gradientPanel1.Controls.Add(pictureBox1);
            gradientPanel1.Controls.Add(txtEmail);
            gradientPanel1.Controls.Add(lbl1);
            gradientPanel1.Controls.Add(btnLogin);
            gradientPanel1.Controls.Add(lbl2);
            gradientPanel1.Controls.Add(txtPassword);
            gradientPanel1.CornerRadius = 15;
            gradientPanel1.CornerRadiusEx.BottomLeft = 15;
            gradientPanel1.CornerRadiusEx.BottomRight = 15;
            gradientPanel1.CornerRadiusEx.TopLeft = 15;
            gradientPanel1.CornerRadiusEx.TopRight = 15;
            gradientPanel1.GlassBorderAlpha = 29;
            gradientPanel1.GradientEndColor = Color.FromArgb(100, 255, 255, 255);
            gradientPanel1.GradientMidColor = Color.Violet;
            gradientPanel1.GradientStartColor = Color.FromArgb(100, 255, 255, 255);
            gradientPanel1.HoverGlowColor = Color.Transparent;
            gradientPanel1.HoverGlowRadius = 0;
            gradientPanel1.Location = new Point(33, 65);
            gradientPanel1.Name = "gradientPanel1";
            gradientPanel1.Padding = new Padding(6);
            gradientPanel1.ShadowColor = Color.Transparent;
            gradientPanel1.ShadowOffsetY = 0;
            gradientPanel1.ShadowRadius = 0;
            gradientPanel1.ShimmerOpacity = 0;
            gradientPanel1.ShowShadow = false;
            gradientPanel1.ShowShimmer = false;
            gradientPanel1.Size = new Size(771, 540);
            gradientPanel1.TabIndex = 32;
            gradientPanel1.UseBlur = true;
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.Transparent;
            pnlContent.Controls.Add(gradientPanel1);
            pnlContent.Controls.Add(gradientPanel3);
            pnlContent.CornerRadius = 15;
            pnlContent.CornerRadiusEx.BottomLeft = 15;
            pnlContent.CornerRadiusEx.BottomRight = 15;
            pnlContent.CornerRadiusEx.TopLeft = 15;
            pnlContent.CornerRadiusEx.TopRight = 15;
            pnlContent.GradientEndColor = Color.Teal;
            pnlContent.GradientMidColor = Color.Violet;
            pnlContent.GradientStartColor = Color.FromArgb(85, 69, 205);
            pnlContent.HoverGlowColor = Color.Transparent;
            pnlContent.HoverGlowRadius = 0;
            pnlContent.Location = new Point(-21, -6);
            pnlContent.Name = "pnlContent";
            pnlContent.Padding = new Padding(6);
            pnlContent.ShadowColor = Color.Transparent;
            pnlContent.ShadowOffsetY = 0;
            pnlContent.ShadowRadius = 0;
            pnlContent.ShowGlassBorder = false;
            pnlContent.ShowShadow = false;
            pnlContent.ShowShimmer = false;
            pnlContent.Size = new Size(826, 629);
            pnlContent.TabIndex = 31;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(795, 611);
            Controls.Add(pnlContent);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            gradientPanel3.ResumeLayout(false);
            gradientPanel1.ResumeLayout(false);
            gradientPanel1.PerformLayout();
            pnlContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private PictureBox picClose;
        private Controls.ModernTextBox txtPassword;
        private Controls.ModernTextBox txtEmail;
        private ReaLTaiizor.Controls.BigLabel lbl2;
        private ReaLTaiizor.Controls.BigLabel lbl1;
        private Controls.ModernButton btnLogin;
        private PictureBox pictureBox1;
        private GradientPanel gradientPanel1;
        private GradientPanel gradientPanel3;
        private GradientPanel pnlContent;
    }
}