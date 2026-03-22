using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Events
{
    public static class AppEvents
    {
        public static event Action OnRefreshCustomersTable;

        public static void ChangeRefreshCustomesTable()
        {
            OnRefreshCustomersTable?.Invoke();
        }

        public static event Action OnRefreshTemplatesTable;

        public static void ChangeRefreshTemplatesTable()
        {
            OnRefreshTemplatesTable?.Invoke();
        }
    }
}
