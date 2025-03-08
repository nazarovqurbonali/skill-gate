namespace Application.Extensions.Smtp;

public sealed class SmtpClientWrapper(
    EmailConfig emailConfig,
    SmtpClient client,
    ILogger<SmtpClientWrapper> logger) : ISmtpClientWrapper
{
    public async Task<BaseResult> ConnectAsync()
    {
        for (int attempt = 1; attempt <= emailConfig.MaxRetryAttempts; attempt++)
        {
            try
            {
                logger.LogInformation(
                    "Attempting to connect to SMTP server {SmtpServer} on port {SmtpPort}. Attempt {Attempt}/{MaxRetryAttempts}",
                    emailConfig.SmtpServer, emailConfig.SmtpPort, attempt, emailConfig.MaxRetryAttempts);

                client.Timeout = emailConfig.Timeout;
                await client.ConnectAsync(emailConfig.SmtpServer, emailConfig.SmtpPort, emailConfig.EnableSsl);
                await client.AuthenticateAsync(emailConfig.SenderEmailAddress, emailConfig.AppPassword);

                logger.LogInformation("Successfully connected to SMTP server.");
                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                if (attempt == emailConfig.MaxRetryAttempts)
                {
                    logger.LogError(ex, "Failed to connect to SMTP server after {MaxRetryAttempts} attempts.",
                        emailConfig.MaxRetryAttempts);
                    return BaseResult.Failure(
                        ResultPatternError.InternalServerError($"SMTP Connection Failed: {ex.Message}"));
                }

                logger.LogWarning(ex, "Attempt {Attempt} to connect to SMTP server failed. Retrying...", attempt);
                await Task.Delay(emailConfig.RetryDelay);
            }
        }

        return BaseResult.Failure(
            ResultPatternError.InternalServerError("SMTP Connection failed after multiple attempts."));
    }

    public async Task<BaseResult> SendMessageAsync(MimeMessage emailMessage)
    {
        try
        {
            logger.LogInformation("Attempting to send email message to {ToEmail}.", emailMessage.To);
            await client.SendAsync(emailMessage);
            logger.LogInformation("Email message sent successfully to {ToEmail}.", emailMessage.To);
            return BaseResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email message.");
            return BaseResult.Failure(ResultPatternError.InternalServerError($"Email Sending Failed: {ex.Message}"));
        }
    }

    public async Task<BaseResult> DisconnectAsync()
    {
        try
        {
            logger.LogInformation("Disconnecting from SMTP server.");
            await client.DisconnectAsync(true);
            logger.LogInformation("Successfully disconnected from SMTP server.");
            return BaseResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to disconnect from SMTP server.");
            return BaseResult.Failure(ResultPatternError.InternalServerError($"SMTP Disconnect Failed: {ex.Message}"));
        }
    }
}