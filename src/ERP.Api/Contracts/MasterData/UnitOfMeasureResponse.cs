using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record UnitOfMeasureResponse(Guid Id, string Code, string Name, bool IsActive)
{
    public static UnitOfMeasureResponse FromDto(UnitOfMeasureDto unitOfMeasure)
    {
        return new UnitOfMeasureResponse(unitOfMeasure.Id, unitOfMeasure.Code, unitOfMeasure.Name, unitOfMeasure.IsActive);
    }

    public static UnitOfMeasureResponse FromDto(ReferenceDataItemDto unitOfMeasure)
        => new(unitOfMeasure.Id, unitOfMeasure.Code, unitOfMeasure.Name, unitOfMeasure.IsActive);
}
