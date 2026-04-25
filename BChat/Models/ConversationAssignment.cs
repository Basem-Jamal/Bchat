using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models
{
    public class ConversationAssignment
    {
        public int Id { get; set; }
        public int CusotmerId { get; set; }
        public int AssignedToUserId { get; set; }
        public int AssignedByUserId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; }

    }
}
