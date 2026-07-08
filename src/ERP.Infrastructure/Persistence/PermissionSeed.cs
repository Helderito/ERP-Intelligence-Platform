using ERP.Domain.Identity;

namespace ERP.Infrastructure.Persistence;

public static class PermissionSeed
{
    public static readonly Guid RolesManageId = Guid.Parse("6fe633a8-dbed-4dc1-a572-f0e42f7d0d3a");

    public static readonly Guid UsersManageId = Guid.Parse("97388676-8f9c-4f9c-a2f1-9c972dd4c19d");

    public static readonly Permission[] Permissions =
    [
        new Permission(RolesManageId, PermissionCodes.RolesManage, "Manage roles and permissions"),
        new Permission(UsersManageId, PermissionCodes.UsersManage, "Manage user role assignments")
    ];
}
