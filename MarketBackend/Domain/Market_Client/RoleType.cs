using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.DAL.DTO;

namespace MarketBackend.Domain.Market_Client
{
    public abstract class RoleType
    {
        public RoleName roleName { get; private set; }
        protected HashSet<Permission> permissions;

        public RoleType(RoleName roleName)
        {
            this.roleName = roleName;
            permissions = new HashSet<Permission>();
        }

        public RoleType(RoleName roleName, HashSet<Permission> permissions)
        {
            this.roleName = roleName;
            this.permissions = permissions;
        }

        //default is *can* do, and according to specific roles, false will be returned from override
        public virtual bool canAddProduct() { return true; }
        public virtual bool canRemoveProduct() { return true; }
        public virtual bool canOpenStore() { return true; }
        public virtual bool canCloseStore() { return true; }
        public virtual bool canUpdateProductPrice() { return true; }
        public virtual bool canUpdateProductDiscount() { return true; }
        public virtual bool canUpdateProductQuantity() { return true; }
        public virtual bool canAddStaffMember(RoleName roleName)
        {
            switch (roleName)
            {
                case RoleName.Founder: return false;
                case RoleName.Owner: return true;
                case RoleName.Manager: return true;
                default: return false;
            }
        }
        public virtual bool canRemoveStaffMember(RoleName roleName)
        {
            switch (roleName)
            {
                case RoleName.Founder: return false;
                case RoleName.Owner: return true;
                case RoleName.Manager: return true;
                default: return false;
            }
        }
        public virtual bool canEditPermissionsToOthers() { return true; }
        public virtual bool canGetHistory() { return true; }
        public virtual bool canEditPermissions() { return true; }
   
        public virtual bool hasPermission(Permission permission)
        {
            return permissions.Contains(permission);
        }

        public virtual void addPermission(Permission permission)
        {
            permissions.Add(permission);
        }
        public virtual void removePermission(Permission permission)
        {
            permissions.Remove(permission);
        }

        public virtual IReadOnlyCollection<Permission> getPermissions() {
            return permissions;
        }

        public static RoleType GetRoleTypeFromDescription(string description) {
            foreach (RoleName roleName in Enum.GetValues(typeof(RoleName))) {
                if (roleName.GetDescription().Equals(description, StringComparison.OrdinalIgnoreCase)) {
                    switch (roleName) {
                        case RoleName.Founder:
                            return new Founder(roleName);
                        case RoleName.Owner:
                            return new Owner(roleName);
                        case RoleName.Manager:
                            return new StoreManagerRole(roleName);
                    }
                }
            }
            return null;  // or throw an exception if appropriate
        }
    }
}
