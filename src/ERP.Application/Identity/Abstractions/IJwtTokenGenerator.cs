using ERP.Application.Identity.Models;
using ERP.Domain.Identity;

namespace ERP.Application.Identity.Abstractions;

public interface IJwtTokenGenerator
{
    AccessTokenResult Generate(User user);

    Guid? Validate(string accessToken);
}
