using BChat.Custom_Controal.Custom_Bchat.Message_Controls;
using BChat.Data.DataStore;
using BChat.Data.DataStore.Chat_Messages_DB;
using BChat.Data.DataStore.Customers_Repository;
using BChat.Events;
using BChat.Global;
using BChat.Models;
using BChat.WhatsApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BChat.UserControls
{
    public partial class ucMessageControl : UserControl
    {
        // ─── State ────────────────────────────────────────────────────────────
        private Dictionary<int, ChatListItemData> _contactsMap = new();
        private int _activeContactId = -1;

        // ─── Constructor ──────────────────────────────────────────────────────
        public ucMessageControl()
        {
            InitializeComponent();
            chatContactInfo1.BlockClicked += OnBlockClicked;
            this.HandleCreated += (s, e) => LoadFromCacheAsync();
        }

        // ─── تحميل البيانات بشكل غير متزامن ─────────────────────────────────
        private async void LoadFromCacheAsync()
        {
            // ربط الأحداث على UI Thread
            AppEvents.OnCustomerAdded += OnCustomerAdded;
            AppEvents.OnCustomerDeleted += OnCustomerDeleted;
            chatSidebar1.ChatSelected += OnChatSelected;
            chatConversation2.MessageSent += OnMessageSent;
            chatConversation2.ConversationTransferred += OnConversationTransferred;

            if (AppCache.WhatsAppListener != null)
                AppCache.WhatsAppListener.MessageReceived += OnWhatsAppMessageReceived;

            // تشغيل العمليات الثقيلة في Background Thread
            var (chats, agentNames, map) = await Task.Run(() => BuildChatsOffThread());

            // تطبيق النتائج على UI Thread
            _contactsMap = map;
            chatConversation2.SetTransferUsers(agentNames);
            chatSidebar1.LoadChats(chats);
        }

        // ─── يعمل على Background Thread ──────────────────────────────────────
        private (List<ChatListItemData> chats, List<string> agentNames, Dictionary<int, ChatListItemData> map)
            BuildChatsOffThread()
        {
            // أسماء الموظفين
            var agentNames = AppCache.Users
                .Where(u => u.Id != AppCache.CurrentUser?.Id)
                .Select(u => u.Name ?? u.Email ?? "موظف")
                .ToList();

            // تحديد العملاء المرئيين
            IEnumerable<Customer> visibleCustomers = AppCache.Customers;

            if (AppCache.CurrentUser?.Role == "Agent")
            {
                var assignedIds = ConversationAssignmentRepository
                    .GetAssignedCustomerIds(AppCache.CurrentUser.Id);

                visibleCustomers = AppCache.Customers
                    .Where(c => assignedIds.Contains(c.Id))
                    .ToList();
            }

            // جمع كل الرسائل دفعة واحدة
            var allMessages = AppCache.ChatMessages
                .GroupBy(m => m.CustomerId)
                .ToDictionary(g => g.Key, g => g.OrderBy(m => m.SentAt).ToList());

            var chats = new List<ChatListItemData>();
            var localMap = new Dictionary<int, ChatListItemData>();

            foreach (var customer in visibleCustomers)
            {
                allMessages.TryGetValue(customer.Id, out var messages);
                var lastMsg = messages?.LastOrDefault();

                var item = new ChatListItemData
                {
                    ContactId = customer.Id,
                    ContactName = customer.Name,
                    LastMessage = lastMsg?.Text ?? "",
                    Timestamp = lastMsg != null ? FormatTimestamp(lastMsg.SentAt) : "",
                    Avatar = null,
                    IsOnline = false,
                    UnreadCount = messages?.Count(m => !m.IsSent && !m.IsRead) ?? 0,
                    IsGroup = false,
                    IsLastMessageSent = lastMsg?.IsSent ?? false,
                    LastMessageAt = lastMsg?.SentAt ?? DateTime.MinValue
                };

                localMap[customer.Id] = item;
                chats.Add(item);
            }

            return (chats.OrderByDescending(c => c.LastMessageAt).ToList(), agentNames, localMap);
        }

        // ─── زر حجب العميل ───────────────────────────────────────────────────
        private void OnBlockClicked(object sender, EventArgs e)
        {
            if (_activeContactId < 0) return;

            var customer = AppCache.Customers.FirstOrDefault(c => c.Id == _activeContactId);
            if (customer == null) return;

            customer.IsBlocked = !customer.IsBlocked;
            CustomerRepository.Block(customer);

            string msg = customer.IsBlocked ? "تم حجب العميل ✅" : "تم رفع الحجب ✅";
            MessageBox.Show(msg, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ─── إضافة عميل جديد ─────────────────────────────────────────────────
        private void OnCustomerAdded(Customer customer)
        {
            if (!IsHandleCreated) return;
            this.Invoke((Action)(() =>
            {
                var newItem = new ChatListItemData
                {
                    ContactId = customer.Id,
                    ContactName = customer.Name,
                    LastMessage = "",
                    Timestamp = "",
                    IsOnline = false,
                    UnreadCount = 0,
                    IsGroup = false,
                    IsLastMessageSent = false,
                    LastMessageAt = DateTime.MinValue
                };

                _contactsMap[customer.Id] = newItem;
                var chats = _contactsMap.Values.OrderByDescending(c => c.LastMessageAt).ToList();
                chatSidebar1.LoadChats(chats);
            }));
        }

        // ─── حذف عميل ────────────────────────────────────────────────────────
        private void OnCustomerDeleted(int customerId)
        {
            if (!IsHandleCreated) return;
            this.Invoke((Action)(() =>
            {
                _contactsMap.Remove(customerId);

                if (_activeContactId == customerId)
                {
                    _activeContactId = -1;
                    chatConversation2.ClearMessages();
                }

                var chats = _contactsMap.Values
                    .OrderByDescending(c => c.LastMessageAt)
                    .ToList();
                chatSidebar1.LoadChats(chats);
            }));
        }

        // ─── تحويل المحادثة ───────────────────────────────────────────────────
        private void OnConversationTransferred(object sender, string agentName)
        {
            if (_activeContactId < 0) return;

            var targetAgent = AppCache.Users
                .FirstOrDefault(u => (u.Name ?? u.Email) == agentName);
            if (targetAgent == null) return;

            ConversationAssignmentRepository.Assign(
                _activeContactId,
                targetAgent.Id,
                AppCache.CurrentUser!.Id);

            var systemMsg = new ChatMessage
            {
                CustomerId = _activeContactId,
                Text = $"تم تحويل المحادثة إلى: {agentName}",
                SentAt = DateTime.Now,
                IsSent = true,
                IsRead = false,
                HasAttachment = false,
                Status = "system",
            };

            systemMsg.Id = ChatMessageRepository.Add(systemMsg);
            AppCache.ChatMessages.Add(systemMsg);
            chatConversation2.AppendMessage(MapToUiMessage(systemMsg));

            MessageBox.Show(
                $"تم تحويل المحادثة إلى {agentName} بنجاح",
                "تحويل المحادثة",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ─── استقبال رسالة واتساب ─────────────────────────────────────────────
        private void OnWhatsAppMessageReceived(IncomingWhatsAppMessage msg)
        {
            try
            {
                var customer = AppCache.Customers
                    .FirstOrDefault(c => c.Phone != null &&
                        c.Phone.Replace("+", "").Replace(" ", "") ==
                        msg.Phone.Replace("+", "").Replace(" ", ""));

                if (customer == null)
                {
                    var newCustomer = new Customer
                    {
                        Name = msg.SenderName,
                        Phone = msg.Phone,
                    };
                    newCustomer.Id = CustomerRepository.Add(newCustomer);
                    AppCache.Customers.Add(newCustomer);
                    customer = newCustomer;

                    this.Invoke((Action)(() =>
                    {
                        var newItem = new ChatListItemData
                        {
                            ContactId = customer.Id,
                            ContactName = customer.Name,
                            LastMessage = "",
                            Timestamp = "",
                            IsOnline = false,
                            UnreadCount = 0,
                            IsGroup = false,
                            IsLastMessageSent = false,
                            LastMessageAt = DateTime.MinValue
                        };
                        _contactsMap[customer.Id] = newItem;
                        var currentChats = _contactsMap.Values
                            .OrderByDescending(c => c.LastMessageAt).ToList();
                        chatSidebar1.LoadChats(currentChats);
                    }));
                }

                // تحقق من التكرار
                bool alreadyExists = AppCache.ChatMessages
                    .Any(m => m.WhatsAppMessageId == msg.WhatsAppMessageId
                           && !string.IsNullOrEmpty(msg.WhatsAppMessageId))
                    || ChatMessageRepository.ExistsByWhatsAppId(msg.WhatsAppMessageId);

                if (alreadyExists) return;

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

                dbMessage.Id = ChatMessageRepository.Add(dbMessage);
                AppCache.ChatMessages.Add(dbMessage);

                if (!IsHandleCreated)
                {
                    this.HandleCreated += (s, e) => UpdateUI(customer, msg, dbMessage);
                    return;
                }

                UpdateUI(customer, msg, dbMessage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack: {ex.StackTrace}");
            }
        }

        // ─── تحديث UI بعد استقبال رسالة ──────────────────────────────────────
        private void UpdateUI(Customer customer, IncomingWhatsAppMessage msg, ChatMessage dbMessage)
        {
            this.Invoke((Action)(() =>
            {
                try
                {
                    if (!_contactsMap.TryGetValue(customer.Id, out var contact)) return;

                    contact.LastMessage = msg.Text;
                    contact.Timestamp = FormatTimestamp(msg.SentAt);
                    contact.IsLastMessageSent = false;
                    contact.LastMessageAt = msg.SentAt;

                    if (_activeContactId == customer.Id)
                        chatConversation2.AppendMessage(MapToUiMessage(dbMessage));
                    else
                        contact.UnreadCount++;

                    chatSidebar1.MoveItemToTop(customer.Id);
                    chatSidebar1.RefreshItem(customer.Id);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ UI Error: {ex.Message}");
                }
            }));
        }

        // ─── اختيار محادثة ───────────────────────────────────────────────────
        private void OnChatSelected(object sender, int contactId)
        {
            if (_activeContactId == contactId) return;
            _activeContactId = contactId;

            if (!_contactsMap.TryGetValue(contactId, out var contact)) return;

            chatConversation2.SetContact(
                contact.ContactName,
                contact.IsOnline ? "متصل الآن" : "غير متصل",
                contact.IsOnline,
                contact.Avatar);

            var customer = AppCache.Customers.FirstOrDefault(c => c.Id == contactId);
            if (customer != null)
            {
                chatContactInfo1.ContactName = customer.Name;
                chatContactInfo1.ContactRole = "عميل";
                chatContactInfo1.ContactPhone = customer.Phone ?? "";
            }

            // تعليم الرسائل كمقروءة
            ChatMessageRepository.MarkAsRead(contactId);
            foreach (var m in AppCache.ChatMessages.Where(m => m.CustomerId == contactId && !m.IsSent))
                m.IsRead = true;

            contact.UnreadCount = 0;
            chatSidebar1.RefreshItem(contactId);

            var messages = AppCache.GetMessagesByCustomer(contactId)
                                   .Select(MapToUiMessage)
                                   .ToList();
            chatConversation2.LoadMessages(messages);
        }

        // ─── إرسال رسالة ─────────────────────────────────────────────────────
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

            var customer = AppCache.Customers.FirstOrDefault(c => c.Id == _activeContactId);
            if (customer != null)
            {
                var success = await MetaSender.SendTextAsync(customer.Phone!, text);
                dbMessage.Status = success ? "sent" : "failed";
                System.Diagnostics.Debug.WriteLine(success ? "✅ أُرسلت لـ Meta" : "❌ فشل الإرسال");
            }

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

        // ─── Mapper: ChatMessage → ChatMessageData ────────────────────────────
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
            IsSystemMessage = m.Status == "system",
            SentByName = m.SentByUserId.HasValue
                ? AppCache.Users.FirstOrDefault(u => u.Id == m.SentByUserId)?.Name ?? ""
                : "",
            SenderName = AppCache.Customers
                .FirstOrDefault(c => c.Id == m.CustomerId)?.Name ?? "",
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