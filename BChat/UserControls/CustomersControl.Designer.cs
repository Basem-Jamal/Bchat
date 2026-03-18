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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlContent = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            btnAddCustomer = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // pnlContent
            // 
            pnlContent.CustomizableEdges = customizableEdges1;
            pnlContent.Location = new Point(0, 175);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlContent.Size = new Size(1429, 572);
            pnlContent.TabIndex = 0;
            // 
            // btnAddCustomer
            // 
            btnAddCustomer.BorderRadius = 20;
            btnAddCustomer.Cursor = Cursors.Hand;
            btnAddCustomer.CustomizableEdges = customizableEdges3;
            btnAddCustomer.DisabledState.BorderColor = Color.DarkGray;
            btnAddCustomer.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAddCustomer.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAddCustomer.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAddCustomer.Font = new Font("Segoe UI", 13F);
            btnAddCustomer.ForeColor = Color.White;
            btnAddCustomer.Location = new Point(1185, 57);
            btnAddCustomer.Name = "btnAddCustomer";
            btnAddCustomer.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnAddCustomer.Size = new Size(180, 45);
            btnAddCustomer.TabIndex = 1;
            btnAddCustomer.Text = "اضافة عميل";
            btnAddCustomer.Click += btnAddCustomer_Click;
            // 
            // CustomersControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(btnAddCustomer);
            Controls.Add(pnlContent);
            Name = "CustomersControl";
            Size = new Size(1429, 747);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlContent;
        private Guna.UI2.WinForms.Guna2Button btnAddCustomer;
    }
}
