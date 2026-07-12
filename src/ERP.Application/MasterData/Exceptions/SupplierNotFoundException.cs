namespace ERP.Application.MasterData.Exceptions;

public sealed class SupplierNotFoundException : InvalidOperationException
{
    public SupplierNotFoundException()
        : base("Supplier was not found.")
    {
    }
}
