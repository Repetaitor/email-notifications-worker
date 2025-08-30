using Core.Application.Interfaces;
using EmailWorkerProject;
using Infrastructure.MailSender;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmailNotificationWorker;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<DapperDbContext>();
                services.AddHostedService<EmailWorker>();
                services.AddMailSender();
            })
            .Build();

        await host.RunAsync();
    }
}