using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record CurrencyResponse(Guid Id, string Code, string Name)
{
    public static CurrencyResponse FromDto(CurrencyDto currency)
        => new(currency.Id, currency.Code, currency.Name);
}
