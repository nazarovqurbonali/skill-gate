namespace Application.Extensions.Smtp;

public sealed class EmailService(
    ISmtpClientWrapper smtpClientWrapper,
    EmailConfig emailConfig,
    ILogger<EmailService> logger) : IEmailService
{
    public async Task<BaseResult> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            logger.LogInformation("Starting email send process to {ToEmail} with subject {Subject}.", toEmail,
                subject);

            MimeMessage emailMessage = CreateEmailMessage(toEmail, subject, body);

            logger.LogInformation("Attempting to connect to the SMTP server.");
            BaseResult connectResult = await smtpClientWrapper.ConnectAsync();
            if (!connectResult.IsSuccess)
            {
                logger.LogError("Failed to connect to SMTP server: {ErrorMessage}", connectResult.Error.Message);
                return connectResult;
            }

            logger.LogInformation("Sending email message to {ToEmail}.", toEmail);
            BaseResult sendResult = await smtpClientWrapper.SendMessageAsync(emailMessage);
            if (!sendResult.IsSuccess)
            {
                logger.LogError($"Failed to send email message to {toEmail}: {sendResult.Error.Message}");
                return sendResult;
            }

            logger.LogInformation("Email sent successfully to {ToEmail}.", toEmail);

            logger.LogInformation("Disconnecting from SMTP server.");
            BaseResult disconnectResult = await smtpClientWrapper.DisconnectAsync();
            return disconnectResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while sending email.");
            return Result<BaseResult>.Failure(
                ResultPatternError.InternalServerError($"Unexpected error: {ex.Message}"));
        }
    }

    private MimeMessage CreateEmailMessage(string toEmail, string subject, string body)
    {
        logger.LogInformation("Creating email message for {ToEmail} with subject {Subject}.", toEmail, subject);
        return new MimeMessage
        {
            From = { new MailboxAddress(emailConfig.SenderName, emailConfig.SenderEmailAddress) },
            To = { new MailboxAddress(toEmail, toEmail) },
            Subject = subject,
            Body = new TextPart(TextFormat.Plain) { Text = body }
        };
    }
}