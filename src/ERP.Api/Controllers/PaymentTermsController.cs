using ERP.Api.Contracts.MasterData;
using ERP.Application.MasterData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize]
[Route("payment-terms")]
public sealed class PaymentTermsController : ControllerBase
{
    private readonly SharedReferenceDataService _sharedReferenceDataService;

    public PaymentTermsController(SharedReferenceDataService sharedReferenceDataService)
        => _sharedReferenceDataService = sharedReferenceDataService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PaymentTermResponse>>> GetPaymentTerms(
        CancellationToken cancellationToken)
    {
        var paymentTerms = await _sharedReferenceDataService.ListPaymentTermsAsync(cancellationToken);
        return Ok(paymentTerms.Select(PaymentTermResponse.FromDto).ToArray());
    }
}
