using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BChat.Models;
using System.Collections.Generic;
using BChat.Models.Users;


namespace BChat.Global
{
    public static class AppCache
    {
        //----------Users------------
        
        public static User? CurrentUser { get; set; }
        public static List<User> Users { get; set; } = new List<User>();
        //----------------------
        // ── Data ──────────────────────────────
        public static List <Groups> Groups { get; set; } = new List<Groups> ();
        public static List<Customer> Customers { get; set; } = new List<Customer>();
        public static List<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public static List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

        public static BChat.WhatsApp.WhatsAppWebhookListener? WhatsAppListener { get; set; }


        // ── GroupMember Helpers ───────────────
        public static List<int> GetGroupIdsByCustomer(int customerId)
        {
            return GroupMembers.Where(gm=> gm.CustomerId == customerId).Select(gm => gm.GroupId).ToList();
        }


        // ── ChatMessage Helpers ───────────────
        public static List<ChatMessage> GetMessagesByCustomer(int customerId)
        {
            return ChatMessages.Where(m => m.CustomerId == customerId).OrderBy(m => m.SentAt).ToList();
        }

        public static int GetUnreadCount(int customerId)
        {
            return ChatMessages.Count(m => m.CustomerId == customerId && !m.IsRead && !m.IsSent);
        }




    }
}
