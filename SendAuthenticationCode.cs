using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace checkpanel_functions
{
    public class SendAuthenticationCodeModel
    {
        public string Authentication { get; set; }
        public string Telephone { get; set; }
    }

    public static class SendAuthenticationCode
    {
        [FunctionName("SendAuthenticationCode")]
        public static void Run([QueueTrigger("send-authentication-code", Connection = "")] SendAuthenticationCodeModel item, ILogger log)
        {
            log.LogInformation("SendAuthenticationCode Triggered");

            var twilio_account_sid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var twilio_auth_token = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            var twilio_sender = Environment.GetEnvironmentVariable("TWILIO_SENDER");

            log.LogInformation("SendAuthenticationCode Initialized Twilio Client");
            TwilioClient.Init(twilio_account_sid, twilio_auth_token);

            log.LogInformation($"SendAuthenticationCode Send authentication code to ${item.Telephone}");

            MessageResource.Create(
                body: $"checkpanel: {item.Authentication}",
                from: new Twilio.Types.PhoneNumber(twilio_sender),
                to: new Twilio.Types.PhoneNumber(item.Telephone)
            );

            log.LogInformation("SendAuthenticationCode Complete");
        }
    }
}
