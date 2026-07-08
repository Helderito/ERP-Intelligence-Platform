using ERP.Domain.Identity;

namespace ERP.Application.Identity.Authorization;

public static class PermissionPolicies
{
    public const string RolesManage = PermissionCodes.RolesManage;

    public const string UsersManage = PermissionCodes.UsersManage;
}
