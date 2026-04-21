using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BChat.Models;
using System.Collections.Generic;


namespace BChat.Global
{
    public static class AppCache
    {
        // ── Data ──────────────────────────────
        public static List <Groups> Groups { get; set; } = new List<Groups> ();
        public static List<Customer> Customers { get; set; } = new List<Customer>();
        public static List<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();


        // ── GroupMember Helpers ───────────────
        public static List<int> GetGroupIdsByCustomer(int customerId)
        {
            return GroupMembers.Where(gm=> gm.CustomerId == customerId).Select(gm => gm.GroupId).ToList();
        }
    }
}
