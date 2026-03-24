namespace BChat.Forms
{
    partial class AddCustomerForm
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
            picClose = new PictureBox();
            lblCustomerName = new ReaLTaiizor.Controls.BigLabel();
            lblCustomerPhone = new ReaLTaiizor.Controls.BigLabel();
            pictureBox1 = new PictureBox();
            btnAddCustomer = new BChat.Controls.ModernButton();
            guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            txbCustomerPhone = new BChat.Controls.ModernTextBox();
            txbCustomerName = new BChat.Controls.ModernTextBox();
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
            lblCustomerName.Font = new Font("IBM Plex Sans Arabic", 20F);
            lblCustomerName.ForeColor = Color.FromArgb(80, 80, 80);
            lblCustomerName.Location = new Point(605, 158);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Size = new Size(136, 46);
            lblCustomerName.TabIndex = 2;
            lblCustomerName.Text = "اسم العميل";
            // 
            // lblCustomerPhone
            // 
            lblCustomerPhone.AutoSize = true;
            lblCustomerPhone.BackColor = Color.Transparent;
            lblCustomerPhone.Font = new Font("IBM Plex Sans Arabic", 20F);
            lblCustomerPhone.ForeColor = Color.FromArgb(80, 80, 80);
            lblCustomerPhone.Location = new Point(605, 358);
            lblCustomerPhone.Name = "lblCustomerPhone";
            lblCustomerPhone.Size = new Size(129, 46);
            lblCustomerPhone.TabIndex = 7;
            lblCustomerPhone.Text = "رقم العميل";
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.add_f;
            pictureBox1.Location = new Point(338, 43);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(135, 98);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // btnAddCustomer
            // 
            btnAddCustomer.BackColor = Color.Transparent;
            btnAddCustomer.BorderRadius = 23;
            btnAddCustomer.Font = new Font("IBM Plex Sans Arabic", 12F, FontStyle.Bold);
            btnAddCustomer.Icon = Properties.Resources.plus;
            btnAddCustomer.Location = new Point(26, 762);
            btnAddCustomer.Name = "btnAddCustomer";
            btnAddCustomer.RightToLeft = RightToLeft.Yes;
            btnAddCustomer.Size = new Size(162, 62);
            btnAddCustomer.TabIndex = 10;
            btnAddCustomer.Text = "اضافة ";
            btnAddCustomer.Click += btnAddCustomer_Click;
            // 
            // guna2CustomGradientPanel1
            // 
            guna2CustomGradientPanel1.Controls.Add(lblCustomerPhone);
            guna2CustomGradientPanel1.Controls.Add(txbCustomerPhone);
            guna2CustomGradientPanel1.Controls.Add(txbCustomerName);
            guna2CustomGradientPanel1.Controls.Add(lblCustomerName);
            guna2CustomGradientPanel1.CustomizableEdges = customizableEdges1;
            guna2CustomGradientPanel1.Location = new Point(12, 169);
            guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            guna2CustomGradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2CustomGradientPanel1.Size = new Size(788, 569);
            guna2CustomGradientPanel1.TabIndex = 9;
            // 
            // txbCustomerPhone
            // 
            txbCustomerPhone.BackColor = Color.Transparent;
            txbCustomerPhone.BackColorEx = Color.FromArgb(237, 235, 255);
            txbCustomerPhone.BorderRadius = 14;
            txbCustomerPhone.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbCustomerPhone.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbCustomerPhone.LabelText = "";
            txbCustomerPhone.Location = new Point(115, 358);
            txbCustomerPhone.MaxLength = 32767;
            txbCustomerPhone.Name = "txbCustomerPhone";
            txbCustomerPhone.PlaceholderText = "";
            txbCustomerPhone.RightToLeft = RightToLeft.Yes;
            txbCustomerPhone.Size = new Size(460, 70);
            txbCustomerPhone.TabIndex = 2;
            // 
            // txbCustomerName
            // 
            txbCustomerName.BackColor = Color.Transparent;
            txbCustomerName.BackColorEx = Color.FromArgb(237, 235, 255);
            txbCustomerName.BorderRadius = 14;
            txbCustomerName.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbCustomerName.Font = new Font("IBM Plex Sans Arabic", 12F);
            txbCustomerName.LabelText = "";
            txbCustomerName.Location = new Point(115, 158);
            txbCustomerName.MaxLength = 32767;
            txbCustomerName.Name = "txbCustomerName";
            txbCustomerName.PlaceholderText = "";
            txbCustomerName.RightToLeft = RightToLeft.Yes;
            txbCustomerName.Size = new Size(460, 70);
            txbCustomerName.TabIndex = 1;
            // 
            // AddCustomerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(812, 851);
            Controls.Add(pictureBox1);
            Controls.Add(picClose);
            Controls.Add(btnAddCustomer);
            Controls.Add(guna2CustomGradientPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "AddCustomerForm";
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
        private ReaLTaiizor.Controls.BigLabel lblCustomerPhone;
        private PictureBox pictureBox1;
        private Controls.ModernButton btnAddCustomer;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private Controls.ModernTextBox txbCustomerName;
        private Controls.ModernTextBox txbCustomerPhone;
        private Guna.UI2.WinForms.Guna2Button btnAddTemplate;
    }
}