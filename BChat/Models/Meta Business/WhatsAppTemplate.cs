using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models.Meta_Business
{
    public class WhatsAppTemplate
    {
        public string Name { get; set; }
        public string Language { get; set; }
        public string Category { get; set; }
        public string BodyText { get; set; }
        public string ComponentsJson { get; set; }

        // Model
        public string HeaderType { get; set; } // "IMAGE", "TEXT", "NONE"
        public string HeaderText { get; set; }

    }
}
