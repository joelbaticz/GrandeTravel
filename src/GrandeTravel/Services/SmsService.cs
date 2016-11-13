using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GrandeTravel.Models;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GrandeTravel.Services
{
    public class SmsService: ISmsService
    {

        private SmsSettings _smsSettings;

        public SmsService()
        {
            _smsSettings = new SmsSettings
            {
                Sid = "AC765c9bc589df510a2f7f72ada58a84b3",
                Token = "1dbcc94d00901efa07f646a5896ec0c9",
                BaseUri = "https://api.twilio.com",
                RequestUri = "/Messages",
                From = "+61437845577"
            };

        }


        private const string TwilioSmsEndpointFormat = "https://api.twilio.com/2010-04-01/Accounts/{0}/Messages.json";

        public async Task SendSmsAsync(string number, string message)
        {
            using (var client = new HttpClient { BaseAddress = new Uri("https://api.twilio.com/2010-04-01/Accounts/AC765c9bc589df510a2f7f72ada58a84b3/Messages.json") })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_smsSettings.Sid}:{_smsSettings.Token}")));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To",number),
                    new KeyValuePair<string, string>("From", _smsSettings.From),
                    new KeyValuePair<string, string>("Body", message)
                });

                var postUrl = string.Format(
                    TwilioSmsEndpointFormat,
                    _smsSettings.Sid);

                /*await client.PostAsync(_smsSettings.RequestUri, content).ConfigureAwait(false);*/

                await client.PostAsync("https://api.twilio.com/2010-04-01/Accounts/AC765c9bc589df510a2f7f72ada58a84b3/Messages.json", content).ConfigureAwait(false);

                /*
                var response = await client.PostAsync(
                "https://api.twilio.com/Accounts/AC765c9bc589df510a2f7f72ada58a84b3/Messages",
                content).ConfigureAwait(false);
                */

                /*
                if (response.IsSuccessStatusCode)
                {
                    if (log != null)
                    {
                        log.LogDebug("success sending sms message to " + toPhoneNumber);
                    }

                    return true;
                }
                */

            }
        }
    }
}
