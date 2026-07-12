namespace ERP.Application.MasterData.Exceptions;

public sealed class SupplierCodeAlreadyExistsException : InvalidOperationException
{
    public SupplierCodeAlreadyExistsException(string code)
        : base($"Supplier code '{code}' already exists.")
    {
    }
}
