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
            iconTop = new PictureBox();
            btnAddCustomer = new BChat.Controls.ModernButton();
            guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            lblNotGroupFound = new ReaLTaiizor.Controls.BigLabel();
            bigLabel1 = new ReaLTaiizor.Controls.BigLabel();
            groupSelector = new BChat.Custom_Controal.Custom_Bchat.GroupSelectorPanel();
            txbCustomerPhone = new BChat.Controls.ModernTextBox();
            txbCustomerName = new BChat.Controls.ModernTextBox();
            ((System.ComponentModel.ISupportInitialize)picClose).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconTop).BeginInit();
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
            picClose.Location = new Point(853, 12);
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
            lblCustomerName.Font = new Font("Microsoft Sans Serif", 20F);
            lblCustomerName.ForeColor = Color.FromArgb(80, 80, 80);
            lblCustomerName.Location = new Point(790, 110);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Size = new Size(107, 31);
            lblCustomerName.TabIndex = 2;
            lblCustomerName.Text = "اسم العميل";
            // 
            // lblCustomerPhone
            // 
            lblCustomerPhone.AutoSize = true;
            lblCustomerPhone.BackColor = Color.Transparent;
            lblCustomerPhone.Font = new Font("Microsoft Sans Serif", 20F);
            lblCustomerPhone.ForeColor = Color.FromArgb(80, 80, 80);
            lblCustomerPhone.Location = new Point(791, 271);
            lblCustomerPhone.Name = "lblCustomerPhone";
            lblCustomerPhone.Size = new Size(106, 31);
            lblCustomerPhone.TabIndex = 7;
            lblCustomerPhone.Text = "رقم العميل";
            // 
            // iconTop
            // 
            iconTop.Cursor = Cursors.Hand;
            iconTop.Image = Properties.Resources.add_user1;
            iconTop.Location = new Point(422, 30);
            iconTop.Name = "iconTop";
            iconTop.Size = new Size(135, 98);
            iconTop.SizeMode = PictureBoxSizeMode.Zoom;
            iconTop.TabIndex = 8;
            iconTop.TabStop = false;
            // 
            // btnAddCustomer
            // 
            btnAddCustomer.BackColor = Color.Transparent;
            btnAddCustomer.BorderColor = Color.FromArgb(200, 196, 214);
            btnAddCustomer.BorderHoverColor = Color.FromArgb(85, 69, 205);
            btnAddCustomer.BorderRadius = 23;
            btnAddCustomer.CustomBackground = Color.DodgerBlue;
            btnAddCustomer.CustomBackgroundHover = Color.SteelBlue;
            btnAddCustomer.CustomForeground = Color.White;
            btnAddCustomer.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnAddCustomer.Icon = null;
            btnAddCustomer.IconChar = FontAwesome.Sharp.IconChar.DAndDBeyond;
            btnAddCustomer.IconSize = 30;
            btnAddCustomer.Image = null;
            btnAddCustomer.Location = new Point(26, 630);
            btnAddCustomer.Name = "btnAddCustomer";
            btnAddCustomer.RightToLeft = RightToLeft.Yes;
            btnAddCustomer.Size = new Size(162, 62);
            btnAddCustomer.TabIndex = 10;
            btnAddCustomer.Text = "اضافة ";
            btnAddCustomer.UseCustomColors = true;
            btnAddCustomer.Click += btnAddCustomer_Click;
            // 
            // guna2CustomGradientPanel1
            // 
            guna2CustomGradientPanel1.Controls.Add(lblNotGroupFound);
            guna2CustomGradientPanel1.Controls.Add(bigLabel1);
            guna2CustomGradientPanel1.Controls.Add(groupSelector);
            guna2CustomGradientPanel1.Controls.Add(lblCustomerPhone);
            guna2CustomGradientPanel1.Controls.Add(btnAddCustomer);
            guna2CustomGradientPanel1.Controls.Add(txbCustomerPhone);
            guna2CustomGradientPanel1.Controls.Add(txbCustomerName);
            guna2CustomGradientPanel1.Controls.Add(lblCustomerName);
            guna2CustomGradientPanel1.CustomizableEdges = customizableEdges1;
            guna2CustomGradientPanel1.Location = new Point(12, 169);
            guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            guna2CustomGradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2CustomGradientPanel1.Size = new Size(918, 718);
            guna2CustomGradientPanel1.TabIndex = 9;
            // 
            // lblNotGroupFound
            // 
            lblNotGroupFound.AutoSize = true;
            lblNotGroupFound.BackColor = Color.IndianRed;
            lblNotGroupFound.Font = new Font("IBM Plex Sans Arabic", 14F);
            lblNotGroupFound.ForeColor = Color.White;
            lblNotGroupFound.Location = new Point(711, 470);
            lblNotGroupFound.Name = "lblNotGroupFound";
            lblNotGroupFound.Size = new Size(195, 32);
            lblNotGroupFound.TabIndex = 12;
            lblNotGroupFound.Text = "لم يتم العثور على مجموعة";
            lblNotGroupFound.Visible = false;
            // 
            // bigLabel1
            // 
            bigLabel1.AutoSize = true;
            bigLabel1.BackColor = Color.Transparent;
            bigLabel1.Font = new Font("Microsoft Sans Serif", 20F);
            bigLabel1.ForeColor = Color.FromArgb(80, 80, 80);
            bigLabel1.Location = new Point(715, 421);
            bigLabel1.Name = "bigLabel1";
            bigLabel1.Size = new Size(182, 31);
            bigLabel1.TabIndex = 11;
            bigLabel1.Text = "اضافة الى مجموعة";
            // 
            // groupSelector
            // 
            groupSelector.AutoScroll = true;
            groupSelector.BackColor = Color.White;
            groupSelector.Location = new Point(244, 421);
            groupSelector.Name = "groupSelector";
            groupSelector.Padding = new Padding(8);
            groupSelector.Size = new Size(462, 231);
            groupSelector.TabIndex = 8;
            // 
            // txbCustomerPhone
            // 
            txbCustomerPhone.BackColor = Color.Transparent;
            txbCustomerPhone.BackColorEx = Color.White;
            txbCustomerPhone.BorderColor = Color.FromArgb(220, 215, 250);
            txbCustomerPhone.BorderRadius = 14;
            txbCustomerPhone.Direction = BChat.Controls.TextDirection.Auto;
            txbCustomerPhone.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbCustomerPhone.Font = new Font("Microsoft Sans Serif", 12F);
            txbCustomerPhone.LabelText = "";
            txbCustomerPhone.Location = new Point(249, 271);
            txbCustomerPhone.MaxLength = 32767;
            txbCustomerPhone.Name = "txbCustomerPhone";
            txbCustomerPhone.PlaceholderText = "";
            txbCustomerPhone.RightToLeft = RightToLeft.Yes;
            txbCustomerPhone.Size = new Size(460, 70);
            txbCustomerPhone.TabIndex = 2;
            txbCustomerPhone.TextPadding = 14;
            txbCustomerPhone.UsePasswordChar = false;
            // 
            // txbCustomerName
            // 
            txbCustomerName.BackColor = Color.Transparent;
            txbCustomerName.BackColorEx = Color.White;
            txbCustomerName.BorderColor = Color.FromArgb(220, 215, 250);
            txbCustomerName.BorderRadius = 14;
            txbCustomerName.Direction = BChat.Controls.TextDirection.Auto;
            txbCustomerName.FocusBorderColor = Color.FromArgb(124, 111, 247);
            txbCustomerName.Font = new Font("Microsoft Sans Serif", 12F);
            txbCustomerName.LabelText = "";
            txbCustomerName.Location = new Point(247, 110);
            txbCustomerName.MaxLength = 32767;
            txbCustomerName.Name = "txbCustomerName";
            txbCustomerName.PlaceholderText = "";
            txbCustomerName.RightToLeft = RightToLeft.Yes;
            txbCustomerName.Size = new Size(460, 70);
            txbCustomerName.TabIndex = 1;
            txbCustomerName.TextPadding = 14;
            txbCustomerName.UsePasswordChar = false;
            // 
            // AddCustomerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(942, 907);
            Controls.Add(iconTop);
            Controls.Add(picClose);
            Controls.Add(guna2CustomGradientPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "AddCustomerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "AddCustomerForm";
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconTop).EndInit();
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
        private PictureBox iconTop;
        private Controls.ModernButton btnAddCustomer;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private Controls.ModernTextBox txbCustomerName;
        private Controls.ModernTextBox txbCustomerPhone;
        private Guna.UI2.WinForms.Guna2Button btnAddTemplate;
        private Custom_Controal.Custom_Bchat.GroupSelectorPanel groupSelector;
        private ReaLTaiizor.Controls.BigLabel bigLabel1;
        private ReaLTaiizor.Controls.BigLabel lblNotGroupFound;
    }
}