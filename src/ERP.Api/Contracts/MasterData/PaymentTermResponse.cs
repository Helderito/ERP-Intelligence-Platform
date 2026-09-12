using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record PaymentTermResponse(Guid Id, string Code, string Name, int NetDays)
{
    public static PaymentTermResponse FromDto(PaymentTermDto paymentTerm)
        => new(paymentTerm.Id, paymentTerm.Code, paymentTerm.Name, paymentTerm.NetDays);
}
