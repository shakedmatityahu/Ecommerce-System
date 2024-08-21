using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Market_Client
{
    public enum Permission
    {
        addProduct,
        removeProduct,
        updateProductPrice,
        updateProductDiscount,
        updateProductQuantity,
        editPermissions,
        all = addProduct | removeProduct | updateProductPrice | updateProductDiscount | updateProductQuantity | editPermissions
    }

    public static class PermissionExtensions
    {
        // Converts enum to string (utilizing the default ToString())
        public static string PermissionToString(this Permission permission)
        {
            return permission.ToString();
        }

        // Converts a string to the corresponding Permission enum
        public static Permission StringToPermission(this string permissionString)
        {
            if (string.IsNullOrEmpty(permissionString))
                throw new ArgumentException("Input string is null or empty", nameof(permissionString));

            if (Enum.TryParse<Permission>(permissionString, true, out Permission result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"Invalid permission: {permissionString}");
            }
        }
    }
}
