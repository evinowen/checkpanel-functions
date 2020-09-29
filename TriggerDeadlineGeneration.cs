using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace checkpanel_functions
{

    public static class TriggerDeadlineGeneration
    {
        public class Deadline
        {
            public int DueHour { get; set; }
            public int DueMinute { get; set; }
        }

        [FunctionName("TriggerDeadlineGeneration")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")] TimerInfo timer, ILogger log)
        {
            log.LogInformation("TriggerDeadlineGeneration Triggered");

            var deadline_generation_endpoint = Environment.GetEnvironmentVariable("DEADLINE_GENERATION_ENDPOINT");
            log.LogInformation($"TriggerDeadlineGeneration Call endpoint {deadline_generation_endpoint} to trigger event");

            var deadline = new Deadline {
                DueHour = DateTime.Now.Hour,
                DueMinute = DateTime.Now.Minute
            };

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.PostAsJsonAsync(deadline_generation_endpoint, deadline);

            response.EnsureSuccessStatusCode();

            log.LogInformation("TriggerDeadlineGeneration Complete");
        }
    }
}
