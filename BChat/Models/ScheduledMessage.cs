using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models
{
    internal class ScheduledMessage
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int? TemplateId { get; set; }
        public DateTime SendAt { get; set; }
        public string Status { get; set; }       // pending / sent / cancelled
        public string TriggerEvent { get; set; } // abandoned_cart / review
        public DateTime CreatedAt { get; set; }

    }
}
