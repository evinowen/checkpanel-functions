using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Communication;
using Azure.Communication.Sms;

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

            string sms_sender_telephone_number = Environment.GetEnvironmentVariable("SMS_SENDER_TELEPHONE_NUMBER");
            string communication_services_connection_string = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");

            SmsClient smsClient = new SmsClient(communication_services_connection_string);
            log.LogInformation("SendDeadlineNotice Initialized SMS Client");

            log.LogInformation($"SendDeadlineNotice Send deadline notice from {sms_sender_telephone_number}");
            log.LogInformation($"SendDeadlineNotice Send deadline notice to {model.Telephone}");

            string message;

            if (model.Deadline > 0)
            {
                message = $"checkpanel: Item \"{model.Summary}\" is due in {model.Deadline} minutes.";
            }
            else
            {
                message = $"checkpanel: Item \"{model.Summary}\" has expired";
            }

            SmsSendResult result = smsClient.Send(
                from: sms_sender_telephone_number,
                to: model.Telephone,
                message: message
            );

            if (result.Successful)
            {
                log.LogInformation("SendDeadlineNotice Complete");
            }
            else
            {
                log.LogInformation($"SendDeadlineNotice Failed, {result.ErrorMessage}");
            }
        }
    }
}
