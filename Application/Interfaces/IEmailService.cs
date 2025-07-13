namespace Application.Interfaces;

public interface IEmailService
{
    Task SendResetPasswordEmailAsync(string toEmail, string code);
}