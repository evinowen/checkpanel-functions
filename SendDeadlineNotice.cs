using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace checkpanel_functions
{
    public class SendDeadlineNoticeModel
    {
        public string Summary { get; set; }
        public int Deadline { get; set; }
        public string Telephone { get; set; }
    }

    public static class SendDeadlineNotice
    {
        [FunctionName("SendDeadlineNotice")]
        public static void Run([ServiceBusTrigger("%SEND_DEADLINE_NOTICE_QUEUE%")] SendDeadlineNoticeModel model, ILogger log)
        {
            log.LogInformation("SendDeadlineNotice Triggered");

            var twilio_account_sid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var twilio_auth_token = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            var twilio_sender = Environment.GetEnvironmentVariable("TWILIO_SENDER");

            log.LogInformation("SendDeadlineNotice Initialized Twilio Client");
            TwilioClient.Init(twilio_account_sid, twilio_auth_token);

            log.LogInformation($"SendDeadlineNotice Send deadline notice to ${model.Telephone}");

            string body;

            if (model.Deadline > 0)
            {
                body = $"checkpanel: Item \"{model.Summary}\" is due in {model.Deadline} minutes.";
            }
            else
            {
                body = $"checkpanel: Item \"{model.Summary}\" has expired";
            }

            MessageResource.Create(
                body: body,
                from: new Twilio.Types.PhoneNumber(twilio_sender),
                to: new Twilio.Types.PhoneNumber(model.Telephone)
            );

            log.LogInformation("SendDeadlineNotice Complete");
        }
    }
}
