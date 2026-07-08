using System.Security.Claims;
using ERP.Api.Contracts.Authentication;
using ERP.Application.Identity.Commands;
using ERP.Application.Identity.Queries;
using ERP.Application.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;

    public AuthController(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthenticationResponse>> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationService.RegisterAsync(
                new RegisterUserCommand(request.Email, request.Password),
                cancellationToken);

            return Ok(AuthenticationResponse.FromResult(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse(ex.Message));
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationService.LoginAsync(
                new LoginCommand(request.Email, request.Password),
                cancellationToken);

            return Ok(AuthenticationResponse.FromResult(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse(ex.Message));
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        await _authenticationService.LogoutAsync(new LogoutCommand(request.RefreshToken), cancellationToken);

        return NoContent();
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthenticationResponse>> Refresh(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationService.RefreshAsync(
                new RefreshTokenCommand(request.RefreshToken),
                cancellationToken);

            return Ok(AuthenticationResponse.FromResult(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse(ex.Message));
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserResponse>> Me(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(new ErrorResponse("Invalid access token."));
        }

        var currentUser = await _authenticationService.GetCurrentUserAsync(
            new GetCurrentUserQuery(userId),
            cancellationToken);

        return Ok(new CurrentUserResponse(
            currentUser.UserId,
            currentUser.Email,
            currentUser.Roles,
            currentUser.Permissions));
    }
}
