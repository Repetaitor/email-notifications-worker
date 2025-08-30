using System.Net;
using System.Net.Mail;
using Core.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MailSender;

public class MailServiceService(IConfiguration configuration, ILogger<IMailSender> _logger)
    : IMailSender
{
    private readonly string _email = configuration["GmailOptions:Email"] ??
                                     throw new ArgumentNullException("GmailOptions:Email is not configured");

    private readonly string _host = configuration["GmailOptions:Host"] ??
                                    throw new ArgumentNullException("GmailOptions:Host is not configured");

    private readonly int _port = int.Parse(configuration["GmailOptions:Port"] ?? "587");

    private readonly string _password = configuration["GmailOptions:Password"] ??
                                        throw new ArgumentNullException("GmailOptions:Password is not configured");

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(_email);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;

            var smtpClient = new SmtpClient(_host)
            {
                Port = _port,
                Credentials = new NetworkCredential(_email, _password),
                EnableSsl = true,
            };
            await smtpClient.SendMailAsync(mail);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}