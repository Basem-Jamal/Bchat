using BChat.Custom_Controal.Custom_Bchat.Message_Controls;
using BChat.Data.DataStore;
using BChat.Global;
using BChat.Models;
using BChat.WhatsApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BChat.UserControls
{
    public partial class MessageControl : UserControl
    {
        // ─── State ────────────────────────────────────────────────────────────
        private Dictionary<int, ChatListItemData> _contactsMap = new();
        private int _activeContactId = -1;

        // ─── Constructor ──────────────────────────────────────────────────────
        public MessageControl()
        {
            InitializeComponent();
            LoadFromCache();
        }

        // ─── الخطوة 1: تحميل البيانات الحقيقية من AppCache ───────────────────
        private void LoadFromCache()
        {
            // 1. بناء قائمة المحادثات من AppCache.Customers
            var chats = new List<ChatListItemData>();

            foreach (var customer in AppCache.Customers)
            {
                var messages = AppCache.GetMessagesByCustomer(customer.Id);
                var lastMsg = messages.LastOrDefault();

                var item = new ChatListItemData
                {
                    ContactId = customer.Id,
                    ContactName = customer.Name,
                    LastMessage = lastMsg?.Text ?? "",
                    Timestamp = lastMsg != null ? FormatTimestamp(lastMsg.SentAt) : "",
                    Avatar = null,                           // يمكن ربطه لاحقاً بصورة العميل
                    IsOnline = false,
                    UnreadCount = AppCache.GetUnreadCount(customer.Id),
                    IsGroup = false,
                    IsLastMessageSent = lastMsg?.IsSent ?? false,

                    LastMessageAt = lastMsg?.SentAt ?? DateTime.MinValue

                };

                _contactsMap[customer.Id] = item;
                chats.Add(item);
            }

            // ترتيب: الأحدث رسالة أولاً
            chats = chats
                .OrderByDescending(c => c.LastMessageAt)
                                            
                .ToList();

            chatSidebar1.LoadChats(chats);

            // 2. ربط الأحداث
            chatSidebar1.ChatSelected += OnChatSelected;
            chatConversation2.MessageSent += OnMessageSent;

            // ─── إضافة جديدة: Transfer Bar ───────────────────────────────
            // تمرير أسماء الموظفين من AppCache.Users
            var agentNames = AppCache.Users
                .Select(u => u.Name ?? u.Email ?? "موظف")
                .ToList();
            chatConversation2.SetTransferUsers(agentNames);

            // ربط حدث التحويل
            chatConversation2.ConversationTransferred += OnConversationTransferred;
            // ─────────────────────────────────────────────────────────────

            if (AppCache.WhatsAppListener != null)
                AppCache.WhatsAppListener.MessageReceived += OnWhatsAppMessageReceived;



            // ربط استقبال رسائل واتساب الواردة
            if (AppCache.WhatsAppListener != null)
                AppCache.WhatsAppListener.MessageReceived += OnWhatsAppMessageReceived;
        }

        private void OnConversationTransferred(object sender, string agentName)
        {
            if (_activeContactId < 0) return;

            // ابحث عن بيانات الموظف المختار
            var targetAgent = AppCache.Users
                .FirstOrDefault(u => (u.Name ?? u.Email) == agentName);

            if (targetAgent == null) return;

            // ① أرسل رسالة نظام داخلية تُخبر بالتحويل (اختياري)
            var systemMsg = new ChatMessage
            {
                CustomerId = _activeContactId,
                Text = $"📌 تم تحويل المحادثة إلى: {agentName}",
                SentAt = DateTime.Now,
                IsSent = true,
                IsRead = false,
                HasAttachment = false,
                Status = "system",
            };
            systemMsg.Id = ChatMessageRepository.Add(systemMsg);
            AppCache.ChatMessages.Add(systemMsg);
            chatConversation2.AppendMessage(MapToUiMessage(systemMsg));

            // ② سجّل التحويل في DB (إذا عندك جدول AssignedAgent أو ما شابه)
            // CustomerRepository.UpdateAssignedAgent(_activeContactId, targetAgent.Id);

            // ③ أخبر المستخدم
            MessageBox.Show(
                $"تم تحويل المحادثة إلى {agentName} بنجاح",
                "تحويل المحادثة",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }



        private void OnWhatsAppMessageReceived(BChat.WhatsApp.IncomingWhatsAppMessage msg)
        {
            try
            {
                // البحث عن العميل بالجوال
                var customer = AppCache.Customers
                    .FirstOrDefault(c => c.Phone != null &&
                                         c.Phone.Replace("+", "").Replace(" ", "") ==
                                         msg.Phone.Replace("+", "").Replace(" ", ""));

                if (customer == null)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ عميل غير موجود: {msg.Phone}");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"✅ عميل موجود: {customer.Name}");

                // ① بناء الرسالة
                var dbMessage = new ChatMessage
                {
                    CustomerId = customer.Id,
                    Text = msg.Text,
                    SentAt = msg.SentAt,
                    IsSent = false,
                    IsRead = false,
                    HasAttachment = false,
                    WhatsAppMessageId = msg.WhatsAppMessageId,
                    Status = "received",
                };

                // ② حفظ في DB
                dbMessage.Id = ChatMessageRepository.Add(dbMessage);
                System.Diagnostics.Debug.WriteLine($"✅ حفظ في DB: Id={dbMessage.Id}");

                // ③ إضافة للكاش
                AppCache.ChatMessages.Add(dbMessage);

                // ④ تحديث الـ UI على UI Thread
                if (!IsHandleCreated) return;

                this.Invoke((Action)(() =>
                {
                    try
                    {
                        if (!_contactsMap.TryGetValue(customer.Id, out var contact)) return;

                        contact.LastMessage = msg.Text;
                        contact.Timestamp = FormatTimestamp(msg.SentAt);
                        contact.IsLastMessageSent = false;
                        contact.LastMessageAt = msg.SentAt;

                        // إذا المحادثة مفتوحة → أضف الفقاعة
                        if (_activeContactId == customer.Id)
                        {
                            chatConversation2.AppendMessage(MapToUiMessage(dbMessage));
                        }
                        else
                        {
                            // إذا مغلقة → زد العداد
                            contact.UnreadCount++;
                        }

                        chatSidebar1.MoveItemToTop(customer.Id);
                        chatSidebar1.RefreshItem(customer.Id);

                        System.Diagnostics.Debug.WriteLine($"✅ UI محدّث");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ UI Error: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack: {ex.StackTrace}");
            }
        }



        // ─── الخطوة 2: اختيار محادثة ─────────────────────────────────────────
        private void OnChatSelected(object sender, int contactId)
        {
            if (_activeContactId == contactId) return;
            _activeContactId = contactId;

            if (!_contactsMap.TryGetValue(contactId, out var contact)) return;

            // تحديث هيدر المحادثة
            chatConversation2.SetContact(
                contact.ContactName,
                contact.IsOnline ? "متصل الآن" : "غير متصل",
                contact.IsOnline,
                contact.Avatar);

            // تحديث لوحة معلومات العميل
            var customer = AppCache.Customers.FirstOrDefault(c => c.Id == contactId);
            if (customer != null)
            {
                chatContactInfo1.ContactName = customer.Name;
                chatContactInfo1.ContactRole = "عميل";
                chatContactInfo1.ContactPhone = customer.Phone ?? "";
                //chatContactInfo1.ContactEmail = customer.Email ?? "";
            }

            // تعليم الرسائل كمقروءة في DB + Cache
            ChatMessageRepository.MarkAsRead(contactId);
            foreach (var m in AppCache.ChatMessages.Where(m => m.CustomerId == contactId && !m.IsSent))
                m.IsRead = true;

            // إعادة حساب العداد في السايدبار
            contact.UnreadCount = 0;
            chatSidebar1.RefreshItem(contactId);

            // تحميل رسائل العميل من الكاش → UI
            var messages = AppCache.GetMessagesByCustomer(contactId)
                                   .Select(MapToUiMessage)
                                   .ToList();
            chatConversation2.LoadMessages(messages);
        }

        // ─── الخطوة 3: إرسال رسالة — Triple Update ───────────────────────────
        private async void OnMessageSent(object sender, string text)
        {
            if (_activeContactId < 0) return;

            var dbMessage = new ChatMessage
            {
                CustomerId = _activeContactId,
                Text = text,
                SentAt = DateTime.Now,
                IsSent = true,
                IsRead = false,
                HasAttachment = false,
                Status = "pending",
            };

            dbMessage.Id = ChatMessageRepository.Add(dbMessage);
            AppCache.ChatMessages.Add(dbMessage);

            // ── إرسال عبر Meta ────────────────────────────────
            var customer = AppCache.Customers.FirstOrDefault(c => c.Id == _activeContactId);
            if (customer != null)
            {
                var success = await MetaSender.SendTextAsync(customer.Phone!, text);
                dbMessage.Status = success ? "sent" : "failed";
                System.Diagnostics.Debug.WriteLine(success ? "✅ أُرسلت لـ Meta" : "❌ فشل الإرسال");
            }
            // ──────────────────────────────────────────────────

            if (_contactsMap.TryGetValue(_activeContactId, out var contact))
            {
                contact.LastMessage = text;
                contact.Timestamp = FormatTimestamp(DateTime.Now);
                contact.IsLastMessageSent = true;
                contact.LastMessageAt = DateTime.Now;
                chatSidebar1.MoveItemToTop(_activeContactId);
            }

            chatConversation2.AppendMessage(MapToUiMessage(dbMessage));
        }
        // ─── Mapper: ChatMessage (DB) → ChatMessageData (UI) ─────────────────
        private static ChatMessageData MapToUiMessage(ChatMessage m) => new()
        {
            MessageId = m.Id,
            Text = m.Text ?? "",
            Timestamp = FormatTimestamp(m.SentAt),
            SentAt = m.SentAt,
            IsSent = m.IsSent,
            SenderAvatar = null,
            HasAttachment = m.HasAttachment,
            AttachmentName = m.AttachmentName ?? "",
            AttachmentSize = FormatSize(m.AttachmentSize),
            AttachmentType = m.AttachmentType ?? "",
        };

        // ─── مساعدات ──────────────────────────────────────────────────────────
        private static string FormatTimestamp(DateTime dt)
        {
            var today = DateTime.Today;
            if (dt.Date == today) return dt.ToString("hh:mm tt");
            if (dt.Date == today.AddDays(-1)) return "أمس";
            return dt.ToString("d/M/yyyy");
        }

        private static string FormatSize(long? bytes)
        {
            if (bytes == null || bytes == 0) return "";
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024):F1} MB";
        }
    }
}