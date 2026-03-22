using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models
{
    public enum TemplateStatus
    {
        Add , Update
    }

    public class Template
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }

        public TemplateStatus Status { get; set; }
    }
}
