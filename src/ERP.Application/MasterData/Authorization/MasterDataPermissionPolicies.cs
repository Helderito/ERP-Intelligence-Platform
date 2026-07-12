using ERP.Domain.Identity;

namespace ERP.Application.MasterData.Authorization;

public static class MasterDataPermissionPolicies
{
    public const string CatalogManage = PermissionCodes.CatalogManage;

    public const string CustomersManage = PermissionCodes.CustomersManage;

    public const string SuppliersManage = PermissionCodes.SuppliersManage;
}
