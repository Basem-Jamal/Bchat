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
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            picClose = new PictureBox();
            lblCustomerName = new ReaLTaiizor.Controls.BigLabel();
            customTextBox1 = new CustomTextBox();
            customTextBox2 = new CustomTextBox();
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
            lblCustomerName.Location = new Point(587, 246);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Size = new Size(145, 37);
            lblCustomerName.TabIndex = 2;
            lblCustomerName.Text = "اسم العميل";
            // 
            // 
            // customTextBox1
            // 
            customTextBox1.BackColorEx = Color.White;
            customTextBox1.BorderColor = Color.Silver;
            customTextBox1.BorderRadius = 10;
            customTextBox1.BorderThickness = 1;
            customTextBox1.FocusBorderColor = Color.DeepSkyBlue;
            customTextBox1.Font = new Font("Segoe UI", 10F);
            customTextBox1.Icon = null;
            customTextBox1.IconPadding = 6;
            customTextBox1.IconSize = 18;
            customTextBox1.Location = new Point(268, 307);
            customTextBox1.Name = "customTextBox1";
            customTextBox1.PlaceholderColor = Color.Gray;
            customTextBox1.PlaceholderText = "";
            customTextBox1.Size = new Size(444, 70);
            customTextBox1.TabIndex = 4;
            customTextBox1.TextColor = Color.Black;
            // 
            // customTextBox2
            // 
            customTextBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            customTextBox2.BackColorEx = Color.White;
            customTextBox2.BorderColor = Color.Silver;
            customTextBox2.BorderRadius = 10;
            customTextBox2.BorderThickness = 1;
            customTextBox2.FocusBorderColor = Color.DeepSkyBlue;
            customTextBox2.Font = new Font("Segoe UI", 10F);
            customTextBox2.Icon = null;
            customTextBox2.IconPadding = 6;
            customTextBox2.IconSize = 18;
            customTextBox2.Location = new Point(268, 493);
            customTextBox2.Name = "customTextBox2";
            customTextBox2.PlaceholderColor = Color.Gray;
            customTextBox2.PlaceholderText = "";
            customTextBox2.Size = new Size(444, 70);
            customTextBox2.TabIndex = 5;
            customTextBox2.TextColor = Color.Black;
            // 
            // AddCustomerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(795, 763);
            Controls.Add(customTextBox2);
            Controls.Add(customTextBox1);
            Controls.Add(lblCustomerName);
            Controls.Add(picClose);
            FormBorderStyle = FormBorderStyle.None;
            Name = "AddCustomerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AddCustomerForm";
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private PictureBox picClose;
        private ReaLTaiizor.Controls.BigLabel bigLabel3;
        private ReaLTaiizor.Controls.BigLabel bigLabel2;
        private ReaLTaiizor.Controls.BigLabel lblCustomerName;
        private CustomTextBox customTextBox2;
        private CustomTextBox customTextBox1;
    }
}