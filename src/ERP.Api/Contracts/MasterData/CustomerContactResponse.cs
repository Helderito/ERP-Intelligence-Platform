using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record CustomerContactResponse(Guid Id, string Name, string? Email, string? Phone)
{
    public static CustomerContactResponse FromDto(CustomerContactDto contact)
    {
        return new CustomerContactResponse(contact.Id, contact.Name, contact.Email, contact.Phone);
    }
}
