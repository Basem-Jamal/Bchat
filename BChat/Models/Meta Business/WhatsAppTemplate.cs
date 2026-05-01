using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models.Meta_Business
{

    public enum TemplateStatus
    {
        Add, Update
    }

    public class WhatsAppTemplate
    {
        public int Id { get; set; } // DB Id
        public string MetaTemplateId { get; set; } // Meta Id
        public string Name { get; set; }
        public string Language { get; set; }
        public string Category { get; set; }
        public string Status { get; set; } // APPROVED / PENDING / REJECTED

        public string BodyText { get; set; }

        public string ComponentsJson { get; set; }

        // Model
        public string HeaderType { get; set; } // "IMAGE", "TEXT", "NONE"
        public string HeaderText { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public TemplateStatus AddOrUpdate { get; set; }

    }
}
