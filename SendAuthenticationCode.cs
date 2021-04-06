using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Communication;
using Azure.Communication.Sms;

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
        public static void Run([ServiceBusTrigger("%SEND_AUTHENTICATION_CODE_QUEUE%")] SendAuthenticationCodeModel model, ILogger log)
        {
            log.LogInformation("SendAuthenticationCode Triggered");

            string sms_sender_telephone_number = Environment.GetEnvironmentVariable("SMS_SENDER_TELEPHONE_NUMBER");
            string communication_services_connection_string = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");

            SmsClient smsClient = new SmsClient(communication_services_connection_string);
            log.LogInformation("SendAuthenticationCode Initialized SMS Client");

            log.LogInformation($"SendAuthenticationCode Send authentication code from {sms_sender_telephone_number}");
            log.LogInformation($"SendAuthenticationCode Send authentication code to {model.Telephone}");

            SmsSendResult result = smsClient.Send(
                from: sms_sender_telephone_number,
                to: model.Telephone,
                message: $"Here is your checkpanel authentication code: {model.Authentication}"
            );

            if (result.Successful)
            {
                log.LogInformation("SendAuthenticationCode Complete");
            }
            else
            {
                log.LogInformation($"SendAuthenticationCode Failed, {result.ErrorMessage}");
            }
        }
    }
}
