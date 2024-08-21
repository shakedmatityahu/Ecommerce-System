using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Market_Client
{
    public class Founder : RoleType
    {
        public Founder(RoleName roleName) : base(roleName) {
            permissions.Add(Permission.all);
        }

        public Founder(RoleName roleName, HashSet<Permission> permissions) : base(roleName, permissions) { }

        public override void addPermission(Permission permission)
        {
            throw new Exception("can't change founder's permissions");
        }

        public override void removePermission(Permission permission)
        {
            throw new Exception("can't change founder's permissions");
        }

    }
}
