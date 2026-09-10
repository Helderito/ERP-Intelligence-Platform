namespace ERP.Application.MasterData.Exceptions;

public sealed class WarehouseNotFoundException : InvalidOperationException
{
    public WarehouseNotFoundException()
        : base("Warehouse was not found.")
    {
    }
}
