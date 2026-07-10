namespace ERP.Application.MasterData.Exceptions;

public sealed class CustomerNotFoundException : InvalidOperationException
{
    public CustomerNotFoundException()
        : base("Customer was not found.")
    {
    }
}
