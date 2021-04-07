using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using PostmarkDotNet;

namespace checkpanel_functions
{
    public class SendDailyReportModel
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public int PointsEarned { get; set; }
        public int PointsAvailable { get; set; }
        public List<SendDailyReportModelRecord> Records { get; set; }
    }
    public class SendDailyReportModelRecord
    {
        public string Name { get; set; }
        public List<string> Punches { get; set; }
    }

    public static class SendDailyReport
    {
        [FunctionName("SendDailyReport")]
        public static async Task Run([ServiceBusTrigger("%SEND_DAILY_REPORT_QUEUE%")] SendDailyReportModel model, ILogger log)
        {
            log.LogInformation("SendDailyReport Triggered");

            var postmark_server_token = Environment.GetEnvironmentVariable("POSTMARK_SERVER_TOKEN");
            var client = new PostmarkClient(postmark_server_token);

            log.LogInformation("SendDailyReport Initialized E-Mail Client");

            var email_from_address = Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS");

            log.LogInformation($"SendDailyReport Send daily report from {email_from_address}");
            log.LogInformation($"SendDailyReport Send daily report to {model.EmailAddress}");

            double point_percentage = ((double) model.PointsEarned / (double) model.PointsAvailable) * 100.0;

            var subject = "Daily CheckPanel Report";

            var text = $"Daily CheckPanel Report for {model.Name}\r\n" +
                $"{point_percentage:0.##}%\r\n" +
                 "Great job, {model.Name}!\r\n" +
                $"You earned {model.PointsEarned}/{model.PointsAvailable} points today.\r\n";

            var punch_table = "<table style='margin: auto;'>";
            foreach (var record in model.Records)
            {
                text += $" {record.Name}\r\n";
                punch_table += "<tr>";
                for (int i = 0; i < 7; i++)
                {
                    var status = "empty";

                    if (record.Punches != null)
                    {
                        if (record.Punches.Count > i)
                        {
                            status = record.Punches[i];
                        }
                    }

                    switch (status)
                    {
                        case "empty":
                            punch_table += $"<td style='color: white'>";
                            break;
                        case "hit":
                            punch_table += $"<td style='color: green'>";
                            break;
                        case "free":
                            punch_table += $"<td style='color: grey'>";
                            break;
                        case "miss":
                            punch_table += $"<td style='color: red'>";
                            break;
                    }
                    punch_table += $"■ </td>";
                }
                punch_table += $"<td>&nbsp;&nbsp;&nbsp;</td><td>{record.Name}</td>";
                punch_table += "</tr>";
            }
            punch_table += "</table>";

            var html = $"<h1>Daily CheckPanel Report for {model.Name}</h1>" +
                "<div style='text-align: center;'>" +
                  $"<p style='font-size: 3em; font-weight: bold;'>{point_percentage:0.##}%</p>" +
                  $"<p style='font-size: 2em; font-weight: bold'>Great job, {model.Name}!</p>" +
                  $"<p>You earned {model.PointsEarned}/{model.PointsAvailable} points today.</p>" +
                  punch_table +
                "</div>";

            var message = new PostmarkMessage()
            {
                To = model.EmailAddress,
                From = email_from_address,
                TrackOpens = true,
                Subject = subject,
                TextBody = text,
                HtmlBody = html
            };

            var result = await client.SendMessageAsync(message);

            log.LogInformation("SendDailyReport Complete");
        }
    }
}
