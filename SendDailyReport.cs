using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace checkpanel_functions
{
    public class SendDailyReportModel
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
    }

    public static class SendDailyReport
    {
        [FunctionName("SendDailyReport")]
        public static void Run([ServiceBusTrigger("%SEND_DAILY_REPORT_QUEUE%")] SendDailyReportModel model, ILogger log, [SendGrid] out SendGridMessage message)
        {
            log.LogInformation("SendDailyReport Triggered");
            var sendgrid_sender = Environment.GetEnvironmentVariable("SENDGRID_SENDER");

            log.LogInformation($"SendDailyReport Send daily report from {sendgrid_sender}");
            log.LogInformation($"SendDailyReport Send daily report to {model.EmailAddress}");

            message = new SendGridMessage();
            message.AddTo(model.EmailAddress);
            message.AddContent("text/html", $"<h1>Daily CheckPanel Report for {model.Name}</h1><br /><br />Great job, {model.Name}!");
            message.SetFrom(new EmailAddress(sendgrid_sender));
            message.SetSubject("Daily CheckPanel Report");

            log.LogInformation("SendDailyReport Complete");
        }
    }
}
