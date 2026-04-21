namespace BChat.UserControls
{
    partial class GroupsControl
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
            btnAddCustomer = new BChat.Controls.ModernButton();
            pictureBox1 = new PictureBox();
            pnlContent = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            pnlContainer = new Car_Rental_System.CustomControls.CustomPanel();
            groupsWrapPanel = new BChat.Custom_Controal.GroupsWrapPanel();
            btnRefreshData = new BChat.Controls.ModernButton();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlContent.SuspendLayout();
            pnlContainer.SuspendLayout();
            SuspendLayout();
            // 
            // btnAddCustomer
            // 
            btnAddCustomer.BackColor = Color.Transparent;
            btnAddCustomer.BorderRadius = 20;
            btnAddCustomer.Font = new Font("IBM Plex Sans Arabic", 10F, FontStyle.Bold);
            btnAddCustomer.Icon = Properties.Resources.plus;
            btnAddCustomer.Location = new Point(1063, 63);
            btnAddCustomer.Name = "btnAddCustomer";
            btnAddCustomer.RightToLeft = RightToLeft.Yes;
            btnAddCustomer.Size = new Size(209, 70);
            btnAddCustomer.TabIndex = 16;
            btnAddCustomer.Text = "اضافة مجموعة";
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.Customers1;
            pictureBox1.Location = new Point(84, 35);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(135, 98);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 14;
            pictureBox1.TabStop = false;
            // 
            // pnlContent
            // 
            pnlContent.Controls.Add(pnlContainer);
            pnlContent.CustomizableEdges = customizableEdges1;
            pnlContent.FillColor = Color.FromArgb(248, 247, 255);
            pnlContent.FillColor2 = Color.FromArgb(248, 247, 255);
            pnlContent.FillColor3 = Color.FromArgb(248, 247, 255);
            pnlContent.FillColor4 = Color.FromArgb(248, 247, 255);
            pnlContent.Location = new Point(0, 172);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlContent.Size = new Size(1312, 659);
            pnlContent.TabIndex = 13;
            // 
            // pnlContainer
            // 
            pnlContainer.BackColor = Color.White;
            pnlContainer.BackColorEx = Color.FromArgb(245, 247, 255);
            pnlContainer.BorderColor = Color.LightGray;
            pnlContainer.BorderRadius = 10;
            pnlContainer.BorderThickness = 1;
            pnlContainer.Controls.Add(groupsWrapPanel);
            pnlContainer.Font = new Font("Segoe UI", 10F);
            pnlContainer.ForeColor = Color.Black;
            pnlContainer.Location = new Point(3, 3);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            pnlContainer.ShadowSize = 6;
            pnlContainer.Size = new Size(1306, 653);
            pnlContainer.TabIndex = 4;
            pnlContainer.UseShadow = true;
            // 
            // groupsWrapPanel
            // 
            groupsWrapPanel.AutoScroll = true;
            groupsWrapPanel.AutoScrollMinSize = new Size(0, 316);
            groupsWrapPanel.BackColor = Color.White;
            groupsWrapPanel.CardHeight = 260;
            groupsWrapPanel.CardWidth = 280;
            groupsWrapPanel.Dock = DockStyle.Fill;
            groupsWrapPanel.HorizontalGap = 20;
            groupsWrapPanel.Location = new Point(0, 0);
            groupsWrapPanel.Name = "groupsWrapPanel";
            groupsWrapPanel.PanelPaddingH = 20;
            groupsWrapPanel.PanelPaddingV = 40;
            groupsWrapPanel.RightToLeft = RightToLeft.Yes;
            groupsWrapPanel.ShowAddCard = true;
            groupsWrapPanel.Size = new Size(1306, 653);
            groupsWrapPanel.TabIndex = 1;
            groupsWrapPanel.VerticalGap = 16;
            groupsWrapPanel.CardDeleteClicked += groupsWrapPanel_CardDeleteClicked;
            groupsWrapPanel.CardEditClicked += groupsWrapPanel_CardEditClicked;
            groupsWrapPanel.AddCardClicked += groupsWrapPanel_AddCardClicked;
            // 
            // btnRefreshData
            // 
            btnRefreshData.BackColor = Color.Transparent;
            btnRefreshData.BorderRadius = 20;
            btnRefreshData.Font = new Font("IBM Plex Sans Arabic", 10F, FontStyle.Bold);
            btnRefreshData.Icon = Properties.Resources.refersh;
            btnRefreshData.Location = new Point(255, 50);
            btnRefreshData.Name = "btnRefreshData";
            btnRefreshData.RightToLeft = RightToLeft.Yes;
            btnRefreshData.Size = new Size(110, 66);
            btnRefreshData.TabIndex = 17;
            btnRefreshData.Text = "تحديث";
            btnRefreshData.Click += btnRefreshData_Click;
            // 
            // GroupsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 247, 255);
            Controls.Add(btnRefreshData);
            Controls.Add(btnAddCustomer);
            Controls.Add(pictureBox1);
            Controls.Add(pnlContent);
            Name = "GroupsControl";
            Size = new Size(1312, 890);
            Load += CustomerGroupsControl_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlContent.ResumeLayout(false);
            pnlContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Controls.ModernButton btnAddCustomer;
        private PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlContent;
        private Custom_Controal.AddGroupCard addGroupCard1;
        private Car_Rental_System.CustomControls.CustomPanel pnlContainer;
        private Custom_Controal.GroupsWrapPanel groupsWrapPanel;
        private Controls.ModernButton btnRefreshData;
    }
}
