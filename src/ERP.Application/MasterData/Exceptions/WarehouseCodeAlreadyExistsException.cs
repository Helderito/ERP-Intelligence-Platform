namespace ERP.Application.MasterData.Exceptions;

public sealed class WarehouseCodeAlreadyExistsException : InvalidOperationException
{
    public WarehouseCodeAlreadyExistsException(string code)
        : base($"A warehouse with code '{code}' already exists.")
    {
    }
}
