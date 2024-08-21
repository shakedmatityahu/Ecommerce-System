using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Market_Client
{
    public class StoreManagerRole : RoleType
    {
        public StoreManagerRole(RoleName roleName) : base(roleName) { }

        public StoreManagerRole(RoleName roleName, HashSet<Permission> permissions) : base(roleName, permissions) { }

        public override bool canOpenStore() { return false; }

        public override bool canCloseStore() { return false; }

        public override bool canAddProduct()
        {
            return hasPermission(Permission.addProduct);
        }

        public override bool canRemoveProduct()
        {
            return hasPermission(Permission.removeProduct);
        }

        public override bool canUpdateProductPrice()
        {
            return hasPermission(Permission.updateProductPrice);
        }

        public override bool canUpdateProductQuantity()
        {
            return hasPermission(Permission.updateProductQuantity);
        }

        public override bool canUpdateProductDiscount()
        {
            return hasPermission(Permission.updateProductDiscount);
        }

        public override bool canEditPermissions() 
        { 
            return hasPermission(Permission.editPermissions); 
        }

        public override bool canAddStaffMember(RoleName roleName) { return false; }

        public override bool canRemoveStaffMember(RoleName roleName) { return false; }
        public override bool canEditPermissionsToOthers() { return false; }

    }
}
