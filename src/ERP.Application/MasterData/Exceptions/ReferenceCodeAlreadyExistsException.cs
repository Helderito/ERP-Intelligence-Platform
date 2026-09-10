namespace ERP.Application.MasterData.Exceptions;

public sealed class ReferenceCodeAlreadyExistsException : InvalidOperationException
{
    public ReferenceCodeAlreadyExistsException(string referenceType, string code)
        : base($"A {referenceType.ToLowerInvariant()} with code '{code}' already exists.")
    {
    }
}
