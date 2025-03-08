namespace Application.Extensions.Smtp;

public interface IEmailService
{
    Task<BaseResult> SendEmailAsync(string toEmail, string subject, string body);
}