namespace ERP.Application.MasterData.Exceptions;

public sealed class CustomerCodeAlreadyExistsException : InvalidOperationException
{
    public CustomerCodeAlreadyExistsException(string code)
        : base($"Customer code '{code}' already exists.")
    {
    }
}
