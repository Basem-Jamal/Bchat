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
            customPanel2 = new Car_Rental_System.CustomControls.CustomPanel();
            customPanel1 = new Car_Rental_System.CustomControls.CustomPanel();
            txtPassword = new BChat.Controls.ModernTextBox();
            txtEmail = new BChat.Controls.ModernTextBox();
            lbl2 = new ReaLTaiizor.Controls.BigLabel();
            lbl1 = new ReaLTaiizor.Controls.BigLabel();
            btnLogin = new BChat.Controls.ModernButton();
            ((System.ComponentModel.ISupportInitialize)picClose).BeginInit();
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
            picClose.Location = new Point(727, 12);
            picClose.Name = "picClose";
            picClose.Size = new Size(56, 49);
            picClose.SizeMode = PictureBoxSizeMode.Zoom;
            picClose.TabIndex = 29;
            picClose.TabStop = false;
            // 
            // customPanel2
            // 
            customPanel2.BackColor = Color.FromArgb(85, 69, 205);
            customPanel2.BackColorEx = Color.FromArgb(85, 69, 205);
            customPanel2.BorderColor = Color.Transparent;
            customPanel2.BorderRadius = 15;
            customPanel2.BorderThickness = 1;
            customPanel2.Font = new Font("Segoe UI", 10F);
            customPanel2.ForeColor = Color.Black;
            customPanel2.Location = new Point(639, 548);
            customPanel2.Name = "customPanel2";
            customPanel2.ShadowColor = Color.Transparent;
            customPanel2.ShadowSize = 6;
            customPanel2.Size = new Size(214, 100);
            customPanel2.TabIndex = 28;
            customPanel2.UseShadow = true;
            // 
            // customPanel1
            // 
            customPanel1.BackColor = Color.FromArgb(85, 69, 205);
            customPanel1.BackColorEx = Color.FromArgb(85, 69, 205);
            customPanel1.BorderColor = Color.Transparent;
            customPanel1.BorderRadius = 15;
            customPanel1.BorderThickness = 1;
            customPanel1.Font = new Font("Segoe UI", 10F);
            customPanel1.ForeColor = Color.Black;
            customPanel1.Location = new Point(-59, -37);
            customPanel1.Name = "customPanel1";
            customPanel1.ShadowColor = Color.Transparent;
            customPanel1.ShadowSize = 6;
            customPanel1.Size = new Size(550, 86);
            customPanel1.TabIndex = 27;
            customPanel1.UseShadow = true;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.Transparent;
            txtPassword.BackColorEx = Color.FromArgb(237, 235, 255);
            txtPassword.BorderRadius = 14;
            txtPassword.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txtPassword.Font = new Font("Microsoft Sans Serif", 12F);
            txtPassword.LabelText = "";
            txtPassword.Location = new Point(267, 302);
            txtPassword.MaxLength = 32767;
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "";
            txtPassword.RightToLeft = RightToLeft.No;
            txtPassword.Size = new Size(367, 51);
            txtPassword.TabIndex = 26;
            // 
            // txtEmail
            // 
            txtEmail.BackColor = Color.Transparent;
            txtEmail.BackColorEx = Color.FromArgb(237, 235, 255);
            txtEmail.BorderRadius = 14;
            txtEmail.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txtEmail.Font = new Font("Microsoft Sans Serif", 12F);
            txtEmail.LabelText = "";
            txtEmail.Location = new Point(267, 177);
            txtEmail.MaxLength = 32767;
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "";
            txtEmail.RightToLeft = RightToLeft.No;
            txtEmail.Size = new Size(367, 51);
            txtEmail.TabIndex = 25;
            // 
            // lbl2
            // 
            lbl2.AutoSize = true;
            lbl2.BackColor = Color.Transparent;
            lbl2.Font = new Font("Microsoft Sans Serif", 16F);
            lbl2.ForeColor = Color.FromArgb(80, 80, 80);
            lbl2.Location = new Point(66, 302);
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
            lbl1.Location = new Point(66, 177);
            lbl1.Name = "lbl1";
            lbl1.Size = new Size(68, 26);
            lbl1.TabIndex = 30;
            lbl1.Text = "Email";
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.Transparent;
            btnLogin.BorderRadius = 15;
            btnLogin.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnLogin.Icon = (Image)resources.GetObject("btnLogin.Icon");
            btnLogin.Location = new Point(378, 445);
            btnLogin.Name = "btnLogin";
            btnLogin.RightToLeft = RightToLeft.Yes;
            btnLogin.Size = new Size(145, 55);
            btnLogin.TabIndex = 32;
            btnLogin.Text = "Loign";
            btnLogin.Click += btnLogin_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(795, 611);
            Controls.Add(btnLogin);
            Controls.Add(lbl2);
            Controls.Add(lbl1);
            Controls.Add(picClose);
            Controls.Add(customPanel2);
            Controls.Add(customPanel1);
            Controls.Add(txtPassword);
            Controls.Add(txtEmail);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private PictureBox picClose;
        private Car_Rental_System.CustomControls.CustomPanel customPanel2;
        private Car_Rental_System.CustomControls.CustomPanel customPanel1;
        private Controls.ModernTextBox txtPassword;
        private Controls.ModernTextBox txtEmail;
        private ReaLTaiizor.Controls.BigLabel lbl2;
        private ReaLTaiizor.Controls.BigLabel lbl1;
        private Controls.ModernButton btnLogin;
    }
}