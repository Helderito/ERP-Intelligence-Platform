using ERP.Api.Contracts.MasterData;
using ERP.Application.MasterData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize]
[Route("countries")]
public sealed class CountriesController : ControllerBase
{
    private readonly SharedReferenceDataService _sharedReferenceDataService;

    public CountriesController(SharedReferenceDataService sharedReferenceDataService)
        => _sharedReferenceDataService = sharedReferenceDataService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CountryResponse>>> GetCountries(
        CancellationToken cancellationToken)
    {
        var countries = await _sharedReferenceDataService.ListCountriesAsync(cancellationToken);
        return Ok(countries.Select(CountryResponse.FromDto).ToArray());
    }
}
