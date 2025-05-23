﻿using GoTorz.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace GoTorz.Api.Services.Auth
{
    /// <summary>
    /// Handles user authentication and registration logic.
    /// </summary>
    public interface IAuthService 
    {
        Task<IdentityResult> RegisterUserAsync(RegisterDTO dto);
        Task<LoginResponseDTO> LoginUserAsync(LoginDTO dto);
        Task<bool> DeleteUserAsync(string userId);

    }

}
