namespace ERP.Application.Tenancy.Exceptions;

public sealed class CompanyNotFoundException : InvalidOperationException
{
    public CompanyNotFoundException() : base("Company was not found.")
    {
    }
}

public sealed class EstablishmentCodeAlreadyExistsException : InvalidOperationException
{
    public EstablishmentCodeAlreadyExistsException(string code)
        : base($"Establishment code '{code}' already exists for this company.")
    {
    }
}
