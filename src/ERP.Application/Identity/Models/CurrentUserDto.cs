namespace ERP.Application.Identity.Models;

public sealed record CurrentUserDto(Guid UserId, string Email);
