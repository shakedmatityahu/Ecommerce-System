// using MarketBackend.Domain.Market_Client;
// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.ComponentModel.DataAnnotations;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using System.Reflection;

// namespace MarketBackend.DAL.DTO
// {
//     [Table("RoleTypes")]
//     public class RoleTypeDTO
//     {
//         [Key]
//         [ForeignKey("MemberDTO")]
//         public string userName { get; set; }

//         [Key]
//         [ForeignKey("StoreDTO")]
//         public int storeId { get; set; }

//         public string roleName { get; set; }
//         public List<string> permissions {get; set; }

//         public RoleTypeDTO(string username, int storeid ,RoleType roleType)
//         {
//             storeId = storeid;
//             userName = username;
//             roleName = roleType.roleName.ToString();
//             permissions = new List<string>();
//             foreach (Permission permission in roleType.getPermissions())
//                 permissions.Add(permission.ToString());
//         }

//         // todo: olga check
//         public RoleTypeDTO(RoleType role)
//         {
//         }

//         public static RoleType ConvertToRoleType(RoleTypeDTO roleDto)
//         {
//             RoleType roleType = RoleType.GetRoleTypeFromDescription(roleDto.roleName);
//             if (roleType == null)
//             {
//                 throw new ArgumentException("Invalid role name.");
//             }

//             // Clear existing permissions if any (considering the logic here depends on your specific implementation)
//             foreach (var perm in roleType.getPermissions().ToList())
//             {
//                 roleType.removePermission(perm);
//             }

//             // Add permissions from DTO
//             foreach (string permString in roleDto.permissions)
//             {
//                 if (Enum.TryParse<Permission>(permString, out Permission perm))
//                 {
//                     roleType.addPermission(perm);
//                 }
//                 else
//                 {
//                     throw new ArgumentException($"Invalid permission: {permString}");
//                 }
//             }

//             return roleType;
//         }
//     }
// }
