namespace ERP.Application.MasterData.Exceptions;

/// <summary>
/// Raised when a product is created with a code that already exists.
/// Maps to HTTP 409 Conflict at the API boundary.
/// Inherits <see cref="InvalidOperationException"/> so callers can still
/// catch the broader type.
/// </summary>
public sealed class ProductCodeAlreadyExistsException : InvalidOperationException
{
    public ProductCodeAlreadyExistsException(string code)
        : base($"A product with code '{code}' already exists.")
    {
    }
}
