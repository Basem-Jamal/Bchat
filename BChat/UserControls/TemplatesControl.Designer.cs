namespace BChat.UserControls
{
    partial class TemplatesControl
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pictureBox1 = new PictureBox();
            btnCountTemplates = new Guna.UI2.WinForms.Guna2Button();
            btnAddTemplate = new Guna.UI2.WinForms.Guna2Button();
            pnlContent = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.resize;
            pictureBox1.Location = new Point(623, 47);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(135, 98);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            // 
            // btnCountTemplates
            // 
            btnCountTemplates.BorderRadius = 10;
            btnCountTemplates.Cursor = Cursors.Hand;
            btnCountTemplates.CustomizableEdges = customizableEdges1;
            btnCountTemplates.DisabledState.BorderColor = Color.DarkGray;
            btnCountTemplates.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCountTemplates.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCountTemplates.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCountTemplates.Font = new Font("Segoe UI", 13F);
            btnCountTemplates.ForeColor = Color.White;
            btnCountTemplates.Location = new Point(-15, 57);
            btnCountTemplates.Name = "btnCountTemplates";
            btnCountTemplates.ShadowDecoration.BorderRadius = 1;
            btnCountTemplates.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnCountTemplates.Size = new Size(283, 51);
            btnCountTemplates.TabIndex = 11;
            btnCountTemplates.Text = "عدد القوالب: 0";
            // 
            // btnAddTemplate
            // 
            btnAddTemplate.BorderRadius = 20;
            btnAddTemplate.Cursor = Cursors.Hand;
            btnAddTemplate.CustomizableEdges = customizableEdges3;
            btnAddTemplate.DisabledState.BorderColor = Color.DarkGray;
            btnAddTemplate.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAddTemplate.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAddTemplate.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAddTemplate.Font = new Font("Segoe UI", 13F);
            btnAddTemplate.ForeColor = Color.White;
            btnAddTemplate.Location = new Point(1113, 57);
            btnAddTemplate.Name = "btnAddTemplate";
            btnAddTemplate.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnAddTemplate.Size = new Size(180, 45);
            btnAddTemplate.TabIndex = 10;
            btnAddTemplate.Text = "اضافة قالب";
            btnAddTemplate.Click += btnAddTemplate_Click;
            // 
            // pnlContent
            // 
            pnlContent.CustomizableEdges = customizableEdges5;
            pnlContent.Location = new Point(0, 175);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowDecoration.CustomizableEdges = customizableEdges6;
            pnlContent.Size = new Size(1306, 572);
            pnlContent.TabIndex = 13;
            // 
            // TemplatesControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(pnlContent);
            Controls.Add(pictureBox1);
            Controls.Add(btnCountTemplates);
            Controls.Add(btnAddTemplate);
            Name = "TemplatesControl";
            Size = new Size(1306, 890);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2Button btnCountTemplates;
        private Guna.UI2.WinForms.Guna2Button btnAddTemplate;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlContent;
    }
}
