using MultiServices.Domain.Interfaces.Services;

namespace MultiServices.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        // TODO: brancher SMTP / SendGrid / Mailjet / etc.
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task SendTemplatedEmailAsync(string to, string templateId, Dictionary<string, string> variables, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
