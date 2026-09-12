using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class PaymentTerm : Entity<Guid>
{
    private PaymentTerm() : base(Guid.Empty)
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public PaymentTerm(Guid id, string code, string name, int netDays) : base(id)
    {
        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        NetDays = ValidateNetDays(netDays);
    }

    public string Code { get; private set; }
    public string Name { get; private set; }
    public int NetDays { get; private set; }

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Payment term code is required.", nameof(code));
        }

        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Payment term name is required.", nameof(name));
        }

        return name.Trim();
    }

    private static int ValidateNetDays(int netDays)
    {
        if (netDays < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(netDays), netDays, "Payment term net days cannot be negative.");
        }

        return netDays;
    }
}
