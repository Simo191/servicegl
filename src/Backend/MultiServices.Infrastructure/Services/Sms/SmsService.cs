using MultiServices.Domain.Interfaces.Services;

namespace MultiServices.Infrastructure.Services.Sms;

public class SmsService : ISmsService
{
    public Task SendAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        // TODO: brancher Twilio / Vonage / etc.
        return Task.CompletedTask;
    }

    public Task SendOtpAsync(string phoneNumber, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task SendSmsAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> VerifyOtpAsync(string phoneNumber, string otp, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
