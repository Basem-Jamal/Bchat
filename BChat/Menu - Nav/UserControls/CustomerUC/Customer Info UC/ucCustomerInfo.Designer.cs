namespace BChat.Menu___Nav.UserControls.CustomerUC.Customer_Info_UC
{
    partial class ucCustomerInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucCustomerInfo));
            pnlContent = new Car_Rental_System.CustomControls.CustomPanel();
            modernPanel1 = new AdvancedPanel();
            proTable1 = new BChat.Custom_Controal.Custom_Bchat.ProTable();
            btnBack = new BChat.Controls.ModernButton();
            customPanel3 = new Car_Rental_System.CustomControls.CustomPanel();
            customPanel2 = new Car_Rental_System.CustomControls.CustomPanel();
            customPanel1 = new Car_Rental_System.CustomControls.CustomPanel();
            avatarControl1 = new BChat.Custom_Controal.Custom_Bchat.AvatarControl();
            pnlContent.SuspendLayout();
            modernPanel1.SuspendLayout();
            customPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.Transparent;
            pnlContent.BackColorEx = Color.FromArgb(248, 247, 255);
            pnlContent.BorderColor = Color.LightGray;
            pnlContent.BorderRadius = 1;
            pnlContent.BorderThickness = 1;
            pnlContent.Controls.Add(modernPanel1);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Font = new Font("Segoe UI", 10F);
            pnlContent.ForeColor = Color.Black;
            pnlContent.Location = new Point(0, 0);
            pnlContent.Name = "pnlContent";
            pnlContent.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            pnlContent.ShadowSize = 0;
            pnlContent.Size = new Size(1315, 808);
            pnlContent.TabIndex = 0;
            pnlContent.UseShadow = true;
            // 
            // modernPanel1
            // 
            modernPanel1.BorderColor = Color.FromArgb(230, 230, 230);
            modernPanel1.BorderRadius = 1;
            modernPanel1.BorderThickness = 1;
            modernPanel1.Controls.Add(proTable1);
            modernPanel1.Controls.Add(btnBack);
            modernPanel1.Controls.Add(customPanel3);
            modernPanel1.Controls.Add(customPanel2);
            modernPanel1.Controls.Add(customPanel1);
            modernPanel1.Font = new Font("Segoe UI", 9F);
            modernPanel1.ForeColor = Color.Black;
            modernPanel1.GlassTransparency = 30;
            modernPanel1.GlowColor = Color.Black;
            modernPanel1.GradientColor1 = Color.SlateGray;
            modernPanel1.GradientColor2 = Color.FromArgb(245, 247, 250);
            modernPanel1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            modernPanel1.Location = new Point(0, 0);
            modernPanel1.Name = "modernPanel1";
            modernPanel1.ShadowBlur = 8;
            modernPanel1.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            modernPanel1.ShadowDepth = 10;
            modernPanel1.ShadowOffsetX = 3;
            modernPanel1.ShadowOffsetY = 3;
            modernPanel1.ShadowSize = 10;
            modernPanel1.ShrinkContentWithShadow = false;
            modernPanel1.Size = new Size(1315, 808);
            modernPanel1.TabIndex = 2;
            modernPanel1.UseGlass = false;
            modernPanel1.UseGlow = false;
            modernPanel1.UseGradient = true;
            modernPanel1.UseShadow = false;
            // 
            // proTable1
            // 
            proTable1.AccentColor = Color.FromArgb(41, 128, 185);
            proTable1.AnimationSpeed = Custom_Controal.Custom_Bchat.ProAnimationSpeed.Fast;
            proTable1.AutoValidate = AutoValidate.EnablePreventFocusChange;
            proTable1.BackColor = Color.White;
            proTable1.EmptyIcon = null;
            proTable1.EmptyText = "لا توجد بيانات للعرض";
            proTable1.Font = new Font("Segoe UI", 10F);
            proTable1.ForeColor = Color.Black;
            proTable1.HeaderBackground = Color.FromArgb(22, 45, 90);
            proTable1.HeaderBackground2 = Color.FromArgb(41, 82, 163);
            proTable1.HeaderFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            proTable1.HeaderForeground = Color.White;
            proTable1.IsRtl = true;
            proTable1.Location = new Point(79, 3);
            proTable1.Name = "proTable1";
            proTable1.OuterBorderColor = Color.Transparent;
            proTable1.RowEven = Color.FromArgb(20, 255, 255, 255);
            proTable1.RowFont = new Font("Segoe UI", 10F);
            proTable1.RowHover = Color.FromArgb(230, 241, 255);
            proTable1.RowOdd = Color.FromArgb(10, 255, 255, 255);
            proTable1.RowSelected = Color.FromArgb(210, 228, 255);
            proTable1.RowTextColor = Color.FromArgb(45, 45, 65);
            proTable1.SearchPlaceholder = "بحث...";
            proTable1.SeparatorColor = Color.FromArgb(230, 235, 248);
            proTable1.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            proTable1.ShadowDepth = 0;
            proTable1.ShowOuterBorder = false;
            proTable1.Size = new Size(1064, 326);
            proTable1.SortArrowColor = Color.White;
            proTable1.TabIndex = 0;
            proTable1.TableStyle = Custom_Controal.Custom_Bchat.ProTableStyle.Minimal;
            // 
            // btnBack
            // 
            btnBack.BackColor = Color.Tomato;
            btnBack.BorderRadius = 20;
            btnBack.Font = new Font("IBM Plex Sans Arabic", 10F, FontStyle.Bold);
            btnBack.Icon = Properties.Resources.refersh;
            btnBack.Location = new Point(16, 12);
            btnBack.Name = "btnBack";
            btnBack.RightToLeft = RightToLeft.Yes;
            btnBack.Size = new Size(110, 66);
            btnBack.TabIndex = 19;
            btnBack.Text = "رجوع";
            btnBack.Variant = BChat.Controls.ButtonVariant.CustomBasem;
            btnBack.Click += btnBack_Click;
            // 
            // customPanel3
            // 
            customPanel3.BackColor = Color.Transparent;
            customPanel3.BackColorEx = Color.FromArgb(248, 247, 255);
            customPanel3.BorderColor = Color.LightGray;
            customPanel3.BorderRadius = 30;
            customPanel3.BorderThickness = 1;
            customPanel3.Font = new Font("Segoe UI", 10F);
            customPanel3.ForeColor = Color.Black;
            customPanel3.Location = new Point(16, 344);
            customPanel3.Name = "customPanel3";
            customPanel3.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            customPanel3.ShadowSize = 0;
            customPanel3.Size = new Size(1286, 449);
            customPanel3.TabIndex = 5;
            customPanel3.UseShadow = true;
            // 
            // customPanel2
            // 
            customPanel2.BackColor = Color.Transparent;
            customPanel2.BackColorEx = Color.FromArgb(248, 247, 255);
            customPanel2.BorderColor = Color.LightGray;
            customPanel2.BorderRadius = 30;
            customPanel2.BorderThickness = 1;
            customPanel2.Font = new Font("Segoe UI", 10F);
            customPanel2.ForeColor = Color.Black;
            customPanel2.Location = new Point(16, 93);
            customPanel2.Name = "customPanel2";
            customPanel2.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            customPanel2.ShadowSize = 0;
            customPanel2.Size = new Size(830, 202);
            customPanel2.TabIndex = 4;
            customPanel2.UseShadow = true;
            // 
            // customPanel1
            // 
            customPanel1.BackColor = Color.Transparent;
            customPanel1.BackColorEx = Color.FromArgb(248, 247, 255);
            customPanel1.BorderColor = Color.LightGray;
            customPanel1.BorderRadius = 30;
            customPanel1.BorderThickness = 1;
            customPanel1.Controls.Add(avatarControl1);
            customPanel1.Font = new Font("Segoe UI", 10F);
            customPanel1.ForeColor = Color.Black;
            customPanel1.Location = new Point(1010, 96);
            customPanel1.Name = "customPanel1";
            customPanel1.ShadowColor = Color.FromArgb(80, 0, 0, 0);
            customPanel1.ShadowSize = 0;
            customPanel1.Size = new Size(292, 199);
            customPanel1.TabIndex = 3;
            customPanel1.UseShadow = true;
            // 
            // avatarControl1
            // 
            avatarControl1.AvatarImage = (Image)resources.GetObject("avatarControl1.AvatarImage");
            avatarControl1.BackColor = Color.Transparent;
            avatarControl1.BorderColor = Color.FromArgb(100, 180, 255);
            avatarControl1.BorderColor2 = Color.FromArgb(100, 180, 255);
            avatarControl1.BorderRadius = 50;
            avatarControl1.BorderStyle = Custom_Controal.Custom_Bchat.AvatarBorderStyle.Dashed;
            avatarControl1.BorderThickness = 2;
            avatarControl1.DashSize = 4;
            avatarControl1.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            avatarControl1.FontSize = 14F;
            avatarControl1.FullName = "User";
            avatarControl1.GlowColor = Color.FromArgb(80, 100, 180, 255);
            avatarControl1.GlowSize = 8;
            avatarControl1.GradientAngle = 135F;
            avatarControl1.ImageFit = Custom_Controal.Custom_Bchat.AvatarImageFit.Cover;
            avatarControl1.ImageOffset = (PointF)resources.GetObject("avatarControl1.ImageOffset");
            avatarControl1.Location = new Point(93, 28);
            avatarControl1.Name = "avatarControl1";
            avatarControl1.Size = new Size(125, 128);
            avatarControl1.TabIndex = 0;
            avatarControl1.Text = "avatarControl1";
            avatarControl1.ZoomFactor = 1F;
            // 
            // ucCustomerInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(pnlContent);
            Name = "ucCustomerInfo";
            Size = new Size(1315, 808);
            pnlContent.ResumeLayout(false);
            modernPanel1.ResumeLayout(false);
            customPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Car_Rental_System.CustomControls.CustomPanel pnlContent;
        private AdvancedPanel modernPanel1;
        private Car_Rental_System.CustomControls.CustomPanel customPanel1;
        private Car_Rental_System.CustomControls.CustomPanel customPanel3;
        private Car_Rental_System.CustomControls.CustomPanel customPanel2;
        private Controls.ModernButton btnBack;
        private Custom_Controal.Custom_Bchat.AvatarControl avatarControl1;
        private Custom_Controal.Custom_Bchat.ProTable proTable1;
    }
}
