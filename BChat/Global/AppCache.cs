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
        public static List <Groups> Groups { get; set; } = new List<Groups> ();
    }
}
