using MarketBackend.Domain.Market_Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBackend.DAL.DTO
{
    [Table("Roles")]
    public class RoleDTO
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Stores")]
        public int storeId { get; }
        
        [Key, Column(Order = 1)]
        [ForeignKey("Members")]
        public string userName { get; }
        public int appointer { get; set; }
        public List <int> appointees { get; set; }

        public string roleName { get; set; }
        public List<string> permissions {get; set; }

        public RoleDTO(Role role)
        {
            storeId = role.storeId;
            userName = role.userName;
            appointer = role.appointer.Id;
            appointees = new List<int>();
            foreach (Member member in role.appointees)
                appointees.Add(new MemberDTO(member).Id);
            roleName = role.getRoleName().ToString();
            permissions = new List<string>();
            foreach (Permission permission in role.getPermissions())
                permissions.Add(permission.ToString());
        }

        public RoleDTO(){}

        public static Role ConvertToRole(RoleDTO roleDto)
        {
            // Assuming you have a method to convert `RoleTypeDTO` to `RoleType`
            RoleType roleType = ConvertToRoleType(roleDto);

            Member appointerMember = ClientRepositoryRAM.GetInstance().GetById(roleDto.appointer);

            Role role = new Role(roleType, appointerMember, roleDto.storeId, roleDto.userName);

            // Handling list of appointees
            foreach (int appDto in roleDto.appointees)
            {
                Member app = ClientRepositoryRAM.GetInstance().GetById(appDto);
                role.addAppointee(app);
            }

            return role;
        }

         public static RoleType ConvertToRoleType(RoleDTO roleDto)
        {
            RoleType roleType = RoleType.GetRoleTypeFromDescription(roleDto.roleName);
            if (roleType == null)
            {
                throw new ArgumentException("Invalid role name.");
            }

            // Clear existing permissions if any (considering the logic here depends on your specific implementation)
            foreach (var perm in roleType.getPermissions().ToList())
            {
                roleType.removePermission(perm);
            }

            // Add permissions from DTO
            foreach (string permString in roleDto.permissions)
            {
                if (Enum.TryParse<Permission>(permString, out Permission perm))
                {
                    roleType.addPermission(perm);
                }
                else
                {
                    throw new ArgumentException($"Invalid permission: {permString}");
                }
            }

            return roleType;
        }

    }
}
