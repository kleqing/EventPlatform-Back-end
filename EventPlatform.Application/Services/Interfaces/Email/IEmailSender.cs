namespace EventPlatform.Application.Services.Interfaces.Email;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}