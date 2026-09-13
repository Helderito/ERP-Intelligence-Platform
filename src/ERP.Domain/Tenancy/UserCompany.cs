using ERP.SharedKernel;

namespace ERP.Domain.Tenancy;

public sealed class UserCompany : Entity<Guid>
{
    private UserCompany() : base(Guid.Empty)
    {
    }

    private UserCompany(Guid id, Guid userId, Guid companyId, DateTime assignedAtUtc) : base(id)
    {
        UserId = EnsureRequiredId(userId, nameof(userId));
        CompanyId = EnsureRequiredId(companyId, nameof(companyId));
        AssignedAtUtc = assignedAtUtc;
    }

    public Guid UserId { get; private set; }
    public Guid CompanyId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }

    public static UserCompany Create(Guid userId, Guid companyId, DateTime assignedAtUtc)
    {
        return new UserCompany(Guid.NewGuid(), userId, companyId, assignedAtUtc);
    }

    private static Guid EnsureRequiredId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Identifier is required.", parameterName);
        }

        return id;
    }
}
