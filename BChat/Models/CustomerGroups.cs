using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models
{
    public enum statusGroups
    {
        Active = 1,
        Inactive = 0
    }
    public class CustomerGroups
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public statusGroups Status { get; set; } = statusGroups.Active;
        public string StatOneValue { get; set; }  // "2.4k"
        public string StatOneLabel { get; set; }  // "عضو"
        public string StatTwoValue { get; set; }  // "25 فبراير"
        public string StatTwoLabel { get; set; }  // "آخر نشاط"
        public string Icon { get; set; }
        public string IconBoxColor { get; set; }

    }
}
