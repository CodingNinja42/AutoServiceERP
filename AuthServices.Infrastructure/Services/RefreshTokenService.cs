using AuthServices.Application.DTOs.Auth;
using AuthServices.Application.Interfaces;
using AuthServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text;
using System.Threading.Tasks;

namespace AuthServices.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AuthDbContext _context;
    private readonly JwtTokenService _jwtTokenService;

    public RefreshTokenService(
        AuthDbContext context,
        JwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<string> CreateAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive)
            ?? throw new InvalidOperationException("User not found or inactive.");

        var rawToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        var refreshToken = new AuthServices.Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // Return the raw token only to the client.
        return rawToken;
    }


    public async Task<RefreshResponse?> RefreshAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == tokenHash);

        if (storedToken is null ||
            !storedToken.IsActive ||
            !storedToken.User.IsActive)
        {
            return null;
        }

        // Revoke the old refresh token.
        storedToken.RevokedAt = DateTime.UtcNow;

        // Generate a new refresh token.
        var rawNewToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        var newToken = new AuthServices.Domain.Entities.RefreshToken
        {
            UserId = storedToken.UserId,
            Token = HashToken(rawNewToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(newToken);

        // Generate a new JWT access token.
        var accessToken = _jwtTokenService.GenerateToken(
            storedToken.User);

        await _context.SaveChangesAsync();

        return new RefreshResponse
        {
            AccessToken = accessToken,
            RefreshToken = rawNewToken
        };
    }

    public async Task RevokeAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == tokenHash);

        if (storedToken is not null && storedToken.IsActive)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
