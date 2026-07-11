using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record CustomerListItemResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsActive)
{
    public static CustomerListItemResponse FromDto(CustomerListItemDto customer)
    {
        return new CustomerListItemResponse(customer.Id, customer.Code, customer.Name, customer.IsActive);
    }
}
