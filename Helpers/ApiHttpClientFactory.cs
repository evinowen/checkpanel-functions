using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace checkpanel_functions.Helpers
{
    public static class ApiHttpClientFactory
    {
        public static HttpClient MakeClient()
        {
            var api_key = Environment.GetEnvironmentVariable("CHECKPANEL_API_KEY");

            byte[] hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(api_key));

            StringBuilder builder = new StringBuilder();
            foreach (byte b in hash)
            {
                builder.Append(b.ToString("X2"));
            }

            string token = builder.ToString();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
    }
}
