using Core.Application.Interfaces;
using Dapper;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailWorkerProject;

public class EmailWorker(
    ILogger<EmailWorker> logger,
    DapperDbContext dapper,
    IConfiguration configuration,
    IMailSender mailSender) : BackgroundService
{
    private readonly string _baseUrl = configuration["BaseURL"] ?? "http://localhost:3000";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var hoursDiff = int.Parse(configuration["EmailWorker:HOURS"] ?? "24");
                using var connection = dapper.CreateConnection();
                var sql = $"SELECT u.email, us.AssignmentId FROM UserAssignments as us" +
                          $" LEFT JOIN Users as u on(u.Id = us.UserId)" +
                          $" WHERE IsEvaluated = 1 And FeedbackSeen = 0" +
                          $" AND DATEDIFF(HOUR, EvaluateDate, GETDATE()) >= {hoursDiff}";
                var email = await connection.QueryAsync<(string, string)>(sql);
                foreach (var ml in email)
                {
                    var result = await mailSender.SendEmailAsync(ml.Item1, "Feedback Reminder",
                        $"You have pending feedback to review. Please log in to your account to complete the evaluation. {_baseUrl + "/feedback/" + ml.Item2}");
                    if (result)
                        logger.LogInformation("Feedback reminder email sent to {Ml}", ml);
                    else
                        logger.LogError("Failed to send feedback reminder email to {Ml}", ml);
                }
            } catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while sending feedback reminder emails.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}