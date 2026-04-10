namespace BChat.UserControls
{
    partial class CustomerGroupsControl
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
            groupsWrapPanel1 = new BChat.Custom_Controal.GroupsWrapPanel();
            crdAddNewGroup = new BChat.Custom_Controal.AddGroupCard();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlContent.SuspendLayout();
            pnlContainer.SuspendLayout();
            groupsWrapPanel1.SuspendLayout();
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
            pnlContainer.Controls.Add(groupsWrapPanel1);
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
            // groupsWrapPanel1
            // 
            groupsWrapPanel1.AutoScroll = true;
            groupsWrapPanel1.AutoScrollMinSize = new Size(0, 316);
            groupsWrapPanel1.BackColor = Color.FromArgb(245, 247, 255);
            groupsWrapPanel1.CardHeight = 260;
            groupsWrapPanel1.CardWidth = 280;
            groupsWrapPanel1.Controls.Add(crdAddNewGroup);
            groupsWrapPanel1.Dock = DockStyle.Fill;
            groupsWrapPanel1.HorizontalGap = 20;
            groupsWrapPanel1.Location = new Point(0, 0);
            groupsWrapPanel1.Name = "groupsWrapPanel1";
            groupsWrapPanel1.PanelPaddingH = 20;
            groupsWrapPanel1.PanelPaddingV = 40;
            groupsWrapPanel1.RightToLeft = RightToLeft.Yes;
            groupsWrapPanel1.ShowAddCard = true;
            groupsWrapPanel1.Size = new Size(1306, 653);
            groupsWrapPanel1.TabIndex = 1;
            groupsWrapPanel1.VerticalGap = 16;
            // 
            // crdAddNewGroup
            // 
            crdAddNewGroup.BackColor = Color.Transparent;
            crdAddNewGroup.Location = new Point(1006, 40);
            crdAddNewGroup.Name = "crdAddNewGroup";
            crdAddNewGroup.Size = new Size(280, 260);
            crdAddNewGroup.TabIndex = 0;
            crdAddNewGroup.Text = "addGroupCard2";
            crdAddNewGroup.Click += crdAddNewGroup_Click;
            // 
            // CustomerGroupsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 247, 255);
            Controls.Add(btnAddCustomer);
            Controls.Add(pictureBox1);
            Controls.Add(pnlContent);
            Name = "CustomerGroupsControl";
            Size = new Size(1312, 890);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlContent.ResumeLayout(false);
            pnlContainer.ResumeLayout(false);
            groupsWrapPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Controls.ModernButton btnAddCustomer;
        private PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlContent;
        private Custom_Controal.AddGroupCard addGroupCard1;
        private Car_Rental_System.CustomControls.CustomPanel pnlContainer;
        private Custom_Controal.GroupsWrapPanel groupsWrapPanel1;
        private Custom_Controal.AddGroupCard crdAddNewGroup;
    }
}
