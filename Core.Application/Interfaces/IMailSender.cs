namespace Core.Application.Interfaces;

public interface IMailSender
{
    public Task<bool> SendEmailAsync(string to, string subject, string body);
}