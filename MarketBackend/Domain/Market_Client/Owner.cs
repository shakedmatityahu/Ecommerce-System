using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Market_Client
{
    public class Owner : RoleType
    {
        public Owner(RoleName roleName) : base(roleName) { }

        public Owner(RoleName roleName, HashSet<Permission> permissions) : base(roleName, permissions) { }

        public override bool canCloseStore() { return false; }

        public override void addPermission(Permission permission)
        {
            throw new Exception("can't change owner's permissions");
        }

        public override void removePermission(Permission permission)
        {
            throw new Exception("can't change owner's permissions");
        }
    }
}
