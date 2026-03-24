using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models
{
    public class MessageDetail
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string TemplateName { get; set; }
        public string Status { get; set; }
        public string TriggerType { get; set; }
        public DateTime SentAt { get; set; }
    }
}