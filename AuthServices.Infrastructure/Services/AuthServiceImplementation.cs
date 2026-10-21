
using AuthServices.Application.DTOs.Auth;
using AuthServices.Application.Interfaces;
using AuthServices.Domain.Entities;
using AuthServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthServices.Infrastructure.Services;

public class AuthServiceImplementation : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthServiceImplementation(
        AuthDbContext context,
        IPasswordService passwordService,
        JwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == email);

        if (existingUser)
            throw new InvalidOperationException(
                "Email is already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            Role = "Customer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var response = new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = _jwtTokenService.GenerateToken(user)
        };

        response.RefreshToken =
            await _refreshTokenService.CreateAsync(user.Id);

        return response;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null ||
            !_passwordService.VerifyPassword(
                request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is inactive.");

        var response = new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = _jwtTokenService.GenerateToken(user)
        };

        response.RefreshToken =
            await _refreshTokenService.CreateAsync(user.Id);

        return response;
    }
}
