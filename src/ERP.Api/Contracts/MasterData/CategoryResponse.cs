using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record CategoryResponse(Guid Id, string Code, string Name)
{
    public static CategoryResponse FromDto(CategoryDto category)
    {
        return new CategoryResponse(category.Id, category.Code, category.Name);
    }
}
