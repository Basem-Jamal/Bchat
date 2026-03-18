using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models
{
    internal class ChatMessage
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int? TemplateId { get; set; }
        public string Status { get; set; }       // sent / failed / pending
        public string TriggerType { get; set; }  // manual / abandoned_cart / order_status / review
        public DateTime SentAt { get; set; }

    }
}
