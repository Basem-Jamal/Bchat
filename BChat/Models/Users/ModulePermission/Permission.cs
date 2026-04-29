using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models.Users.ModulePermission
{
    public static class Permission
    {
        public enum PermissionType
        {
            None,
            Agent,
            Admin,
            FullAccess
        }
    }
}
