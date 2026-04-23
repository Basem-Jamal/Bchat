namespace BChat.Menu___Nav.UserControls.Show_Group_Members_Control
{
    partial class GroupMembersControl
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
            btnBack = new BChat.Controls.ModernButton();
            btnGroupName = new BChat.Controls.ModernButton();
            stcdCoustomers = new BChat.Controls.StatCard();
            SuspendLayout();
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.White;
            pnlContent.CustomizableEdges = customizableEdges1;
            pnlContent.Location = new Point(0, 171);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlContent.Size = new Size(1312, 531);
            pnlContent.TabIndex = 2;
            // 
            // btnBack
            // 
            btnBack.BackColor = Color.Transparent;
            btnBack.BorderRadius = 20;
            btnBack.Font = new Font("IBM Plex Sans Arabic", 10F, FontStyle.Bold);
            btnBack.Icon = Properties.Resources.refersh;
            btnBack.Location = new Point(51, 33);
            btnBack.Name = "btnBack";
            btnBack.RightToLeft = RightToLeft.Yes;
            btnBack.Size = new Size(110, 66);
            btnBack.TabIndex = 18;
            btnBack.Text = "رجوع";
            btnBack.Variant = BChat.Controls.ButtonVariant.CustomBasem;
            btnBack.Click += btnBack_Click;
            // 
            // btnGroupName
            // 
            btnGroupName.BackColor = Color.Transparent;
            btnGroupName.BorderRadius = 20;
            btnGroupName.Font = new Font("IBM Plex Sans Arabic", 16F, FontStyle.Bold);
            btnGroupName.Icon = null;
            btnGroupName.Location = new Point(912, 33);
            btnGroupName.Name = "btnGroupName";
            btnGroupName.RightToLeft = RightToLeft.Yes;
            btnGroupName.Size = new Size(376, 66);
            btnGroupName.TabIndex = 20;
            btnGroupName.Text = "اسم المجموعة";
            // 
            // stcdCoustomers
            // 
            stcdCoustomers.AccentColor = Color.FromArgb(32, 201, 151);
            stcdCoustomers.BackColor = Color.Transparent;
            stcdCoustomers.CardColor = Color.White;
            stcdCoustomers.IconBgColor = Color.FromArgb(220, 245, 235);
            stcdCoustomers.IconColor = Color.FromArgb(32, 201, 151);
            stcdCoustomers.Location = new Point(814, 708);
            stcdCoustomers.Name = "stcdCoustomers";
            stcdCoustomers.ShadowColor = Color.FromArgb(30, 0, 0, 0);
            stcdCoustomers.Size = new Size(425, 135);
            stcdCoustomers.TabIndex = 1;
            stcdCoustomers.Text = "statCard1";
            stcdCoustomers.Title = "العملاء النشطون";
            stcdCoustomers.TitleColor = Color.FromArgb(150, 160, 175);
            stcdCoustomers.Value = "0";
            stcdCoustomers.ValueColor = Color.FromArgb(25, 35, 60);
            // 
            // GroupMembersControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 247, 255);
            Controls.Add(stcdCoustomers);
            Controls.Add(btnGroupName);
            Controls.Add(btnBack);
            Controls.Add(pnlContent);
            Name = "GroupMembersControl";
            Size = new Size(1312, 890);
            ResumeLayout(false);
        }

        #endregion
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlContent;
        private Controls.ModernButton btnBack;
        private Controls.ModernButton btnGroupName;
        private Controls.StatCard stcdCoustomers;
    }
}
