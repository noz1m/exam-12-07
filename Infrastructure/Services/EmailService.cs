using Application.DTOs.Account;
using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services;

public class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
{
    private readonly EmailSettings emailSettings = emailSettings.Value;

    public async Task SendResetPasswordEmailAsync(string toEmail, string code)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(emailSettings.From));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Password Reset Code";

        var builder = new BodyBuilder
        {
            HtmlBody = $@"
                <html>
                <body>
                    <h2>Password Reset</h2>
                    <p>Your password reset code is: <strong>{code}</strong></p>
                    <p>This code is valid for 10 minutes.</p>
                    <p>If you did not request a password reset, please ignore this email.</p>
                </body>
                </html>"
        };

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(emailSettings.SmtpServer, emailSettings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(emailSettings.Username, emailSettings.Password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}