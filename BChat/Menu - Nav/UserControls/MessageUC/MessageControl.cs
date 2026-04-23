using BChat.Custom_Controal.Custom_Bchat.Message_Controls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BChat.UserControls
{
    public partial class MessageControl : UserControl
    {
        public MessageControl()
        {
            InitializeComponent();
            LoadTestData();
        }

        // في أعلى الكلاس
        private Dictionary<int, List<ChatMessageData>> _conversations = new();
        private Dictionary<int, ChatListItemData> _contactsMap = new();
        private int _activeContactId = -1;

        private void LoadTestData()
        {
            // 1. قائمة المحادثات
            var chats = new List<ChatListItemData>
    {
        new() { ContactId = 1, ContactName = "سارة أحمد", LastMessage = "جاري كتابة رسالة...", Timestamp = "12:45 م", IsOnline = true, UnreadCount = 0 },
        new() { ContactId = 2, ContactName = "محمد خالد", LastMessage = "شكراً لك، سأقوم بمراجعة الملفات", Timestamp = "أمس", IsOnline = false, UnreadCount = 2 },
        new() { ContactId = 3, ContactName = "ليلى يوسف", LastMessage = "تم تأكيد الموعد غداً", Timestamp = "أمس", IsOnline = false, UnreadCount = 0 },
    };

            foreach (var c in chats)
                _contactsMap[c.ContactId] = c;

            chatSidebar1.LoadChats(chats);

            // 2. رسائل وهمية لكل محادثة
            _conversations[1] = new List<ChatMessageData>
    {
        new() { MessageId=1, Text="مرحباً! كيف حالك؟", Timestamp="10:30 ص", SentAt=DateTime.Today.AddHours(-2), IsSent=false },
        new() { MessageId=2, Text="الحمدلله بخير، وأنت؟", Timestamp="10:32 ص", SentAt=DateTime.Today.AddHours(-1), IsSent=true },
        new() { MessageId=3, Text="بخير، هل راجعت العرض؟", Timestamp="10:35 ص", SentAt=DateTime.Today, IsSent=false },
    };

            _conversations[2] = new List<ChatMessageData>
    {
        new() { MessageId=10, Text="السلام عليكم", Timestamp="أمس", SentAt=DateTime.Today.AddDays(-1), IsSent=false },
        new() { MessageId=11, Text="وعليكم السلام", Timestamp="أمس", SentAt=DateTime.Today.AddDays(-1), IsSent=true },
    };

            _conversations[3] = new List<ChatMessageData>();

            // 3. ربط الأحداث
            chatSidebar1.ChatSelected += OnChatSelected;
            chatConversation2.MessageSent += OnMessageSent;
        }

        private void OnChatSelected(object sender, int contactId)
        {
            // ✅ إذا نفس العميل المفتوح، لا تعيد التحميل
            if (_activeContactId == contactId) return;

            _activeContactId = contactId;

            if (!_contactsMap.ContainsKey(contactId)) return;

            var contact = _contactsMap[contactId];

            chatConversation2.SetContact(contact.ContactName,
                                         contact.IsOnline ? "متصل الآن" : "غير متصل",
                                         contact.IsOnline,
                                         null);

            chatContactInfo1.ContactName = contact.ContactName;
            chatContactInfo1.ContactRole = "عميل";
            chatContactInfo1.ContactPhone = "+966 50 000 0000";
            chatContactInfo1.ContactEmail = "customer@example.com";

            if (_conversations.ContainsKey(contactId))
                chatConversation2.LoadMessages(_conversations[contactId]);
            else
                chatConversation2.ClearMessages();
        }
        private void OnMessageSent(object sender, string text)
        {
            if (_activeContactId < 0) return;

            var newMessage = new ChatMessageData
            {
                MessageId = new Random().Next(1000, 9999),
                Text = text,
                Timestamp = DateTime.Now.ToString("hh:mm tt"),
                SentAt = DateTime.Now,
                IsSent = true
            };

            if (!_conversations.ContainsKey(_activeContactId))
                _conversations[_activeContactId] = new List<ChatMessageData>();

            _conversations[_activeContactId].Add(newMessage);

            // ✅ تحديث آخر رسالة في السايدبار
            if (_contactsMap.ContainsKey(_activeContactId))
            {
                _contactsMap[_activeContactId].LastMessage = text;
                _contactsMap[_activeContactId].Timestamp = DateTime.Now.ToString("hh:mm tt");
                _contactsMap[_activeContactId].IsLastMessageSent = true; // ✅

                chatSidebar1.RefreshItem(_activeContactId); // ✅ إعادة رسم العنصر فقط
            }

            chatConversation2.AppendMessage(newMessage);
        }
    }
}