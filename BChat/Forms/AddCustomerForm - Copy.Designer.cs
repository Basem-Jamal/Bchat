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
            txbCustomerName = new CustomTextBox();
            txbCustomerPhone = new CustomTextBox();
            btnAddCustomer = new Guna.UI2.WinForms.Guna2Button();
            lblCustomerPhone = new ReaLTaiizor.Controls.BigLabel();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picClose).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
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
            lblCustomerName.Location = new Point(567, 245);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Size = new Size(145, 37);
            lblCustomerName.TabIndex = 2;
            lblCustomerName.Text = "اسم العميل";
            // 
            // txbCustomerName
            // 
            txbCustomerName.BackColorEx = Color.White;
            txbCustomerName.BorderColor = Color.Silver;
            txbCustomerName.BorderRadius = 10;
            txbCustomerName.BorderThickness = 1;
            txbCustomerName.FocusBorderColor = Color.DeepSkyBlue;
            txbCustomerName.Font = new Font("Segoe UI", 10F);
            txbCustomerName.Icon = null;
            txbCustomerName.IconPadding = 6;
            txbCustomerName.IconSize = 18;
            txbCustomerName.Location = new Point(268, 307);
            txbCustomerName.MaxLength = 32767;
            txbCustomerName.Name = "txbCustomerName";
            txbCustomerName.PlaceholderColor = Color.Gray;
            txbCustomerName.PlaceholderText = "";
            txbCustomerName.Size = new Size(444, 70);
            txbCustomerName.TabIndex = 4;
            txbCustomerName.TextAlign = HorizontalAlignment.Left;
            txbCustomerName.TextColor = Color.Black;
            // 
            // txbCustomerPhone
            // 
            txbCustomerPhone.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txbCustomerPhone.BackColorEx = Color.White;
            txbCustomerPhone.BorderColor = Color.Silver;
            txbCustomerPhone.BorderRadius = 10;
            txbCustomerPhone.BorderThickness = 1;
            txbCustomerPhone.FocusBorderColor = Color.DeepSkyBlue;
            txbCustomerPhone.Font = new Font("Segoe UI", 10F);
            txbCustomerPhone.Icon = null;
            txbCustomerPhone.IconPadding = 6;
            txbCustomerPhone.IconSize = 18;
            txbCustomerPhone.Location = new Point(268, 468);
            txbCustomerPhone.MaxLength = 32767;
            txbCustomerPhone.Name = "txbCustomerPhone";
            txbCustomerPhone.PlaceholderColor = Color.Gray;
            txbCustomerPhone.PlaceholderText = "";
            txbCustomerPhone.Size = new Size(444, 70);
            txbCustomerPhone.TabIndex = 5;
            txbCustomerPhone.TextAlign = HorizontalAlignment.Left;
            txbCustomerPhone.TextColor = Color.Black;
            txbCustomerPhone.KeyPress += txbCustomerPhone_KeyPress;
            // 
            // btnAddCustomer
            // 
            btnAddCustomer.BorderRadius = 20;
            btnAddCustomer.Cursor = Cursors.Hand;
            btnAddCustomer.CustomizableEdges = customizableEdges1;
            btnAddCustomer.DisabledState.BorderColor = Color.DarkGray;
            btnAddCustomer.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAddCustomer.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAddCustomer.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAddCustomer.Font = new Font("Segoe UI", 13F);
            btnAddCustomer.ForeColor = Color.White;
            btnAddCustomer.Location = new Point(309, 656);
            btnAddCustomer.Name = "btnAddCustomer";
            btnAddCustomer.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnAddCustomer.Size = new Size(180, 45);
            btnAddCustomer.TabIndex = 6;
            btnAddCustomer.Text = "اضافة";
            btnAddCustomer.Click += btnAddCustomer_Click;
            // 
            // lblCustomerPhone
            // 
            lblCustomerPhone.AutoSize = true;
            lblCustomerPhone.BackColor = Color.Transparent;
            lblCustomerPhone.Font = new Font("Segoe UI", 20F);
            lblCustomerPhone.ForeColor = Color.FromArgb(80, 80, 80);
            lblCustomerPhone.Location = new Point(574, 412);
            lblCustomerPhone.Name = "lblCustomerPhone";
            lblCustomerPhone.Size = new Size(138, 37);
            lblCustomerPhone.TabIndex = 7;
            lblCustomerPhone.Text = "رقم العميل";
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
            // AddCustomerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(795, 763);
            Controls.Add(pictureBox1);
            Controls.Add(lblCustomerPhone);
            Controls.Add(btnAddCustomer);
            Controls.Add(txbCustomerPhone);
            Controls.Add(txbCustomerName);
            Controls.Add(lblCustomerName);
            Controls.Add(picClose);
            FormBorderStyle = FormBorderStyle.None;
            Name = "AddCustomerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AddCustomerForm";
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private PictureBox picClose;
        private ReaLTaiizor.Controls.BigLabel bigLabel3;
        private ReaLTaiizor.Controls.BigLabel bigLabel2;
        private ReaLTaiizor.Controls.BigLabel lblCustomerName;
        private CustomTextBox txbCustomerPhone;
        private CustomTextBox txbCustomerName;
        private Guna.UI2.WinForms.Guna2Button btnAddCustomer;
        private ReaLTaiizor.Controls.BigLabel lblCustomerPhone;
        private PictureBox pictureBox1;
    }
}