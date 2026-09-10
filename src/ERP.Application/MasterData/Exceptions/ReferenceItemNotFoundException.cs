namespace ERP.Application.MasterData.Exceptions;

public sealed class ReferenceItemNotFoundException : InvalidOperationException
{
    public ReferenceItemNotFoundException(string referenceType)
        : base($"{referenceType} was not found.")
    {
    }
}
