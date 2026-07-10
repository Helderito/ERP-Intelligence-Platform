namespace ERP.Application.MasterData.Exceptions;

/// <summary>
/// Raised when an operation targets a product that does not exist.
/// Maps to HTTP 404 Not Found at the API boundary.
/// </summary>
public sealed class ProductNotFoundException : InvalidOperationException
{
    public ProductNotFoundException()
        : base("Product was not found.")
    {
    }
}
