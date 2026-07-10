namespace ERP.Application.MasterData.Exceptions;

/// <summary>
/// Raised when a product references master data (category, unit of measure)
/// that does not exist. Maps to HTTP 422 Unprocessable Entity at the API
/// boundary — the request is well-formed but fails a business rule.
/// </summary>
public sealed class MasterDataReferenceNotFoundException : InvalidOperationException
{
    public MasterDataReferenceNotFoundException(string message)
        : base(message)
    {
    }
}
