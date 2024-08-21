using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketBackend.Domain.Market_Client;

namespace MarketBackend.Services.Models
{
    public class RoleResultDto
    {
        public string Role { get; set; }
        public string Username { get; set; }
        public string Appointer { get; set; }
        public List<string> Appointees { get; set; }

        public List<string> Permissions { get; set; }

        public RoleResultDto(Role role)
        {
            Role = role.getRoleName().ToString(); 
            Username = role.userName; 
            Appointer = role.getAppointer()?.UserName; 
            Appointees = role.getAppointees().Select(appointee => appointee.UserName).ToList(); 
            Permissions = role.getPermissions().Select(permission => permission.PermissionToString()).ToList();
        }
    }
}
