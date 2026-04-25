using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models.Apis
{
    public class ApiSetting
    {
    
        public int Id { get; set; } 
        public string Provider { get; set; } = "";
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
