namespace BChat.UserControls
{
    partial class CustomersControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlContent = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            pictureBox1 = new PictureBox();
            stcdCoustomers = new BChat.Controls.StatCard();
            btnAddCustomer = new BChat.Controls.ModernButton();
            btnRefreshData = new BChat.Controls.ModernButton();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.White;
            pnlContent.CustomizableEdges = customizableEdges1;
            pnlContent.Location = new Point(0, 190);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlContent.Size = new Size(1306, 461);
            pnlContent.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.users2;
            pictureBox1.Location = new Point(84, 22);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(135, 98);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            // 
            // stcdCoustomers
            // 
            stcdCoustomers.AccentColor = Color.FromArgb(32, 201, 151);
            stcdCoustomers.BackColor = Color.Transparent;
            stcdCoustomers.CardColor = Color.White;
            stcdCoustomers.IconBgColor = Color.FromArgb(220, 245, 235);
            stcdCoustomers.IconColor = Color.FromArgb(32, 201, 151);
            stcdCoustomers.Location = new Point(868, 667);
            stcdCoustomers.Name = "stcdCoustomers";
            stcdCoustomers.ShadowColor = Color.FromArgb(30, 0, 0, 0);
            stcdCoustomers.Size = new Size(425, 135);
            stcdCoustomers.TabIndex = 0;
            stcdCoustomers.Text = "statCard1";
            stcdCoustomers.Title = "العملاء النشطون";
            stcdCoustomers.TitleColor = Color.FromArgb(150, 160, 175);
            stcdCoustomers.Value = "0";
            stcdCoustomers.ValueColor = Color.FromArgb(25, 35, 60);
            // 
            // btnAddCustomer
            // 
            btnAddCustomer.BackColor = Color.Transparent;
            btnAddCustomer.BorderRadius = 20;
            btnAddCustomer.Font = new Font("IBM Plex Sans Arabic", 10F, FontStyle.Bold);
            btnAddCustomer.Icon = Properties.Resources.plus;
            btnAddCustomer.Location = new Point(1068, 50);
            btnAddCustomer.Name = "btnAddCustomer";
            btnAddCustomer.RightToLeft = RightToLeft.Yes;
            btnAddCustomer.Size = new Size(209, 70);
            btnAddCustomer.TabIndex = 11;
            btnAddCustomer.Text = "اضافة عميل";
            btnAddCustomer.Click += btnAddCustomer_Click;
            // 
            // btnRefreshData
            // 
            btnRefreshData.BackColor = Color.Transparent;
            btnRefreshData.BorderRadius = 20;
            btnRefreshData.Font = new Font("IBM Plex Sans Arabic", 10F, FontStyle.Bold);
            btnRefreshData.Icon = Properties.Resources.refersh;
            btnRefreshData.Location = new Point(245, 41);
            btnRefreshData.Name = "btnRefreshData";
            btnRefreshData.RightToLeft = RightToLeft.Yes;
            btnRefreshData.Size = new Size(110, 66);
            btnRefreshData.TabIndex = 12;
            btnRefreshData.Text = "تحديث";
            btnRefreshData.Click += btnRefreshData_Click;
            // 
            // CustomersControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 247, 255);
            Controls.Add(btnRefreshData);
            Controls.Add(btnAddCustomer);
            Controls.Add(stcdCoustomers);
            Controls.Add(pictureBox1);
            Controls.Add(pnlContent);
            Name = "CustomersControl";
            Size = new Size(1318, 816);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlContent;
        private PictureBox pictureBox1;
        private Controls.StatCard stcdCoustomers;
        private Controls.ModernButton btnAddCustomer;
        private Controls.ModernButton btnRefreshData;
    }
}
