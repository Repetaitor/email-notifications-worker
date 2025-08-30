using Core.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MailSender;

public static class ServiceExtensions
{
    public static void AddMailSender(this IServiceCollection services)
    {
        services.AddScoped<IMailSender, MailServiceService>();
    }
}