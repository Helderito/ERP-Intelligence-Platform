using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record CountryResponse(Guid Id, string Code, string Name)
{
    public static CountryResponse FromDto(CountryDto country)
        => new(country.Id, country.Code, country.Name);
}
