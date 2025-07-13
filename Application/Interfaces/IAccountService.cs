using Application.ApiResponse;
using Application.DTOs.Account;

namespace Application.Interfaces;

public interface IAccountService
{
    Task<Response<string>> RegisterAsync(RegisterDTO register);
    Task<Response<string>> LoginAsync(LoginDTO login);
    Task<Response<string>> ResetPasswordAsync(ResetPasswordDTO dto);
    Task<Response<string>> ChangePasswordAsync(ChangePasswordDTO dto);
    Task<Response<string>> RequestPasswordResetAsync(ForgotPasswordRequestDTO dto);
}
