using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.DAL;
using MarketBackend.DAL.DTO;

namespace MarketBackend.Domain.Market_Client
{
    public class Role
    {
        public RoleType role { get; }
        public int storeId { get; }
        public string userName { get; }
        public Member appointer;
        public List<Member> appointees;

        public Role(RoleType role, Member appointer, int storeId, string userName) { 
            this.role = role;
            this.appointer = appointer;
            this.storeId = storeId;
            this.userName = userName;
            appointees = new List<Member>();
        }

        public Role(RoleDTO roleDTO)
        {
            role = createRoleType(roleDTO);
            appointer = ClientRepositoryRAM.GetInstance().GetById(roleDTO.appointer);
            appointees = new List<Member>();
            foreach (int memberDTOId in roleDTO.appointees)
            {
                appointees.Add(ClientRepositoryRAM.GetInstance().GetById(memberDTOId));
            }
        }

        public RoleType createRoleType(RoleDTO roleDTO)
        {
            RoleName roleName = CastNameOperator(roleDTO.roleName);
            HashSet<Permission> permissions = new HashSet<Permission>();
            foreach (string permission in roleDTO.permissions)
            {
                permissions.Add(CastPermissionOperator(permission));
            }

            switch (roleName)
            {
                case RoleName.Owner:
                    return new Owner(roleName, permissions);
                case RoleName.Founder:
                    return new Founder(roleName, permissions);
                case RoleName.Manager:
                    return new StoreManagerRole(roleName, permissions);
                default:
                    throw new Exception("Invalid role name");
            }
        }

        private RoleName CastNameOperator(string roleName)
        {
            try
            {
                return (RoleName)Enum.Parse(typeof(RoleName), roleName);
            }
            catch { throw new Exception("Invalid operator name"); }
        }

        private Permission CastPermissionOperator(string permission)
        {
            try
            {
                return (Permission)Enum.Parse(typeof(Permission), permission);
            }
            catch { throw new Exception("Invalid operator name"); }
        }



        public Member getAppointer() { return appointer; }

        public IReadOnlyList<Member> getAppointees() {  return appointees; }

        public void addAppointee(Member appToAdd) {  appointees.Add(appToAdd);}
        public void removeAppointee(Member appToRem) { appointees.Add(appToRem); }

        public bool canAddProduct() { return role.canAddProduct(); }
        public bool canRemoveProduct() { return role.canRemoveProduct(); }
        public bool canOpenStore() { return role.canOpenStore(); }
        public bool canCloseStore() { return role.canCloseStore(); }
        public bool canUpdateProductPrice() { return role.canUpdateProductPrice(); }
        public bool canUpdateProductDiscount() { return role.canUpdateProductDiscount(); }
        public bool canUpdateProductQuantity() { return role.canUpdateProductQuantity(); }
        public bool canAddStaffMember(RoleName roleName) { return role.canAddStaffMember(roleName); }
        public bool canRemoveStaffMember(RoleName roleName) { return role.canRemoveStaffMember(roleName); }
        public bool canEditPermissions() { return role.canEditPermissions(); }

        public bool canGetHistory() { return role.canGetHistory(); }
        public bool hasPermission(Permission permission)
        {
            return role.hasPermission(permission);
        }

        public void addPermission(Permission permission)
        {
            role.addPermission(permission);

        }
        public void removePermission(Permission permission)
        {
            role.removePermission(permission);
        }

        public IReadOnlyCollection<Permission> getPermissions()
        {
            return role.getPermissions();
        }

        public RoleName getRoleName()
        {
            return role.roleName;
        }
    }
}
