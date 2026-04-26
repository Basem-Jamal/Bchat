using BChat.Models;
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


        // Message Action
        public static event Action OnRefreshMessagesTable;

        public static event Action<Customer>? CustomerAdded;
        public static void NotifyCustomerAdded(Customer customer)
        {
            CustomerAdded?.Invoke(customer);
        }
            

        public static event Action<int>? CustomerDeleted;
        public static void NotifyCustomerDeleted(int customerId)
        {
            CustomerDeleted?.Invoke(customerId);
        }
            

        public static void ChangeRefreshMessagesTable()
        {
            OnRefreshMessagesTable?.Invoke();
        }

        public static class Groups
        {
            public static event Action<Models.Groups> OnGroupAdded;
            public static event Action<Models.Groups> OnGroupUpdated;

            public static void ChangeGroupAdded(Models.Groups group)
            {
                OnGroupAdded?.Invoke(group);
            }

            public static void ChangeGroupUpdated(Models.Groups group)
            {
                OnGroupUpdated?.Invoke(group);
            }

        }
    }
}
