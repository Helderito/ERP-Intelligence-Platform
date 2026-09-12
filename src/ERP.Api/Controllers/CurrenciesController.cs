using ERP.Api.Contracts.MasterData;
using ERP.Application.MasterData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize]
[Route("currencies")]
public sealed class CurrenciesController : ControllerBase
{
    private readonly SharedReferenceDataService _sharedReferenceDataService;

    public CurrenciesController(SharedReferenceDataService sharedReferenceDataService)
        => _sharedReferenceDataService = sharedReferenceDataService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CurrencyResponse>>> GetCurrencies(
        CancellationToken cancellationToken)
    {
        var currencies = await _sharedReferenceDataService.ListCurrenciesAsync(cancellationToken);
        return Ok(currencies.Select(CurrencyResponse.FromDto).ToArray());
    }
}
