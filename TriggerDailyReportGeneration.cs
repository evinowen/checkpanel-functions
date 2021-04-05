using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using checkpanel_functions.Helpers;

namespace checkpanel_functions
{
    public static class TriggerDailyReportGeneration
    {
        public class Report
        {
            public string Name { get; set; }
        }

        [FunctionName("TriggerDailyReportGeneration")]
        public async static Task Run([TimerTrigger("0 0 5 * * *")] TimerInfo timer, ILogger log)
        {
            log.LogInformation("TriggerDailyReportGeneration Triggered");

            var daily_report_endpoint = Environment.GetEnvironmentVariable("CHECKPANEL_REPORT_ENDPOINT");
            log.LogInformation($"TriggerDailyReportGeneration Call endpoint {daily_report_endpoint} to trigger event");

            var report = new Report
            {
                Name = "daily"
            };

            HttpClient client = ApiHttpClientFactory.MakeClient();

            HttpResponseMessage response = await client.PostAsJsonAsync(daily_report_endpoint, report);

            response.EnsureSuccessStatusCode();

            log.LogInformation("TriggerDailyReportGeneration Complete");
        }
    }
}
