using ERP.SharedKernel;

namespace ERP.Domain.Identity;

public sealed class Permission : Entity<Guid>
{
    private Permission()
        : base(Guid.Empty)
    {
        Code = string.Empty;
        Description = string.Empty;
    }

    public Permission(Guid id, string code, string description)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Permission code is required.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Permission description is required.", nameof(description));
        }

        Code = code.Trim().ToLowerInvariant();
        Description = description.Trim();
    }

    public string Code { get; private set; }

    public string Description { get; private set; }
}
