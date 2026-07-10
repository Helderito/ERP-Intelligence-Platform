using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record UnitOfMeasureResponse(Guid Id, string Code, string Name)
{
    public static UnitOfMeasureResponse FromDto(UnitOfMeasureDto unitOfMeasure)
    {
        return new UnitOfMeasureResponse(unitOfMeasure.Id, unitOfMeasure.Code, unitOfMeasure.Name);
    }
}
