namespace BChat.UserControls
{
    partial class MessageControl
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
            chatSidebar1 = new BChat.Custom_Controal.Custom_Bchat.Message_Controls.ChatSidebar();
            chatConversation2 = new BChat.Custom_Controal.Custom_Bchat.Message_Controls.ChatConversation();
            chatContactInfo1 = new BChat.Custom_Controal.Custom_Bchat.Message_Controls.ChatContactInfo();
            customPanel1 = new Car_Rental_System.CustomControls.CustomPanel();
            customPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // chatSidebar1
            // 
            chatSidebar1.BackColor = Color.Black;
            chatSidebar1.Dock = DockStyle.Right;
            chatSidebar1.Location = new Point(934, 0);
            chatSidebar1.MinimumSize = new Size(260, 400);
            chatSidebar1.Name = "chatSidebar1";
            chatSidebar1.RightToLeft = RightToLeft.Yes;
            chatSidebar1.Size = new Size(384, 816);
            chatSidebar1.TabIndex = 0;
            // 
            // chatConversation2
            // 
            chatConversation2.AccentColor = Color.FromArgb(124, 111, 247);
            chatConversation2.AutoScroll = true;
            chatConversation2.BackColor = Color.FromArgb(248, 247, 255);
            chatConversation2.BorderColor = Color.FromArgb(226, 232, 240);
            chatConversation2.ComposerBackColor = Color.White;
            chatConversation2.ComposerPlaceholder = "اكتب رسالتك هنا...";
            chatConversation2.ContactIsOnline = true;
            chatConversation2.ContactName = "اسم المحادثة";
            chatConversation2.ContactStatus = "متصل الآن";
            chatConversation2.DatePillBackColor = Color.FromArgb(226, 232, 240);
            chatConversation2.DatePillForeColor = Color.FromArgb(100, 116, 139);
            chatConversation2.Dock = DockStyle.Right;
            chatConversation2.FontFamily = "IBM Plex Sans Arabic";
            chatConversation2.HeaderBackColor = Color.White;
            chatConversation2.HeaderHeight = 72;
            chatConversation2.Location = new Point(338, 0);
            chatConversation2.MutedColor = Color.FromArgb(148, 163, 184);
            chatConversation2.Name = "chatConversation2";
            chatConversation2.OnlineColor = Color.FromArgb(16, 185, 129);
            chatConversation2.PageBackColor = Color.FromArgb(248, 247, 255);
            chatConversation2.ReceivedBubbleColor = Color.White;
            chatConversation2.ReceivedTextColor = Color.FromArgb(15, 23, 42);
            chatConversation2.RightToLeft = RightToLeft.Yes;
            chatConversation2.SentBubbleColor = Color.FromArgb(124, 111, 247);
            chatConversation2.SentTextColor = Color.White;
            chatConversation2.Size = new Size(596, 816);
            chatConversation2.TabIndex = 2;
            // 
            // chatContactInfo1
            // 
            chatContactInfo1.BackColor = Color.FromArgb(255, 255, 255);
            chatContactInfo1.ContactAvatar = null;
            chatContactInfo1.ContactEmail = "user@example.com";
            chatContactInfo1.ContactName = "اسم المستخدم";
            chatContactInfo1.ContactPhone = "+966 50 000 0000";
            chatContactInfo1.ContactRole = "عميل";
            chatContactInfo1.Dock = DockStyle.Left;
            chatContactInfo1.Location = new Point(0, 0);
            chatContactInfo1.MinimumSize = new Size(260, 400);
            chatContactInfo1.Name = "chatContactInfo1";
            chatContactInfo1.RightToLeft = RightToLeft.Yes;
            chatContactInfo1.Size = new Size(320, 816);
            chatContactInfo1.TabIndex = 3;
            chatContactInfo1.TotalMediaCount = 0;
            // 
            // customPanel1
            // 
            customPanel1.BackColorEx = Color.White;
            customPanel1.BorderColor = Color.LightGray;
            customPanel1.BorderRadius = 1;
            customPanel1.BorderThickness = 1;
            customPanel1.Controls.Add(chatContactInfo1);
            customPanel1.Dock = DockStyle.Fill;
            customPanel1.Font = new Font("Segoe UI", 10F);
            customPanel1.ForeColor = Color.Black;
            customPanel1.Location = new Point(0, 0);
            customPanel1.Name = "customPanel1";
            customPanel1.ShadowColor = Color.Transparent;
            customPanel1.ShadowSize = 0;
            customPanel1.Size = new Size(1318, 816);
            customPanel1.TabIndex = 4;
            customPanel1.UseShadow = true;
            // 
            // MessageControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(chatConversation2);
            Controls.Add(chatSidebar1);
            Controls.Add(customPanel1);
            Name = "MessageControl";
            Size = new Size(1318, 816);
            customPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Custom_Controal.Custom_Bchat.Message_Controls.ChatSidebar chatSidebar1;
        private Custom_Controal.Custom_Bchat.Message_Controls.ChatConversation chatConversation2;
        private Custom_Controal.Custom_Bchat.Message_Controls.ChatContactInfo chatContactInfo1;
        private Car_Rental_System.CustomControls.CustomPanel customPanel1;
    }
}
