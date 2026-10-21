using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using AuthServices.Application.DTOs.Auth;

namespace AuthServices.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<string> CreateAsync(Guid userId);

    Task<RefreshResponse?> RefreshAsync(string refreshToken);

    Task RevokeAsync(string refreshToken);
}

