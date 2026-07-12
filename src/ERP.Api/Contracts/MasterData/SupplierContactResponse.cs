using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record SupplierContactResponse(Guid Id, string Name, string? Email, string? Phone)
{
    public static SupplierContactResponse FromDto(SupplierContactDto contact)
    {
        return new SupplierContactResponse(contact.Id, contact.Name, contact.Email, contact.Phone);
    }
}
