using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

using GrandeTravel.Models;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

namespace GrandeTravel.Services
{
    public class EmailService : IEmailService
    {

        //private readonly EmailSettings _emailSettings;

        private EmailSettings myEmailSettings;
        /*
        public AuthMessageSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = new EmailSettings();
                
            emailSettings.Value;
        }
        */

        /*
            {
  "EmailSettings": {
    "ApiKey": "api:key-*",
    "BaseUri": "https://api.mailgun.net/v3/",
    "RequestUri": "sandbox*.mailgun.org/messages",
    "From": "postmaster@sandbox*.mailgun.org"
  }
}
*/
        public EmailService()
        {
            myEmailSettings = new EmailSettings()
            {
                ApiKey = "api:key-62ead3e0fd2a1f3723d34bff09f136fe",
                BaseUri = "https://api.mailgun.net/v3/sandbox51c62c88b46e413690f7763c68ea1339.mailgun.org",
                RequestUri = "sandbox51c62c88b46e413690f7763c68ea1339.mailgun.org/messages",
                From = "postmaster@sandbox51c62c88b46e413690f7763c68ea1339.mailgun.org"
            };
        }


        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(myEmailSettings.BaseUri) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(myEmailSettings.ApiKey)));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("from", myEmailSettings.From),
                    new KeyValuePair<string, string>("to", email),
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("text", message)
                });

                await client.PostAsync(myEmailSettings.RequestUri, content).ConfigureAwait(false);
            }
        }

        /*
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(_emailSettings.BaseUri) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(_emailSettings.ApiKey)));

                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("from", _emailSettings.From),
            new KeyValuePair<string, string>("to", email),
            new KeyValuePair<string, string>("subject", subject),
            new KeyValuePair<string, string>("text", message)
        });

                await client.PostAsync(_emailSettings.RequestUri, content).ConfigureAwait(false);
            }
        }
        */


        /*
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("", "postmaster@sandbox51c62c88b46e413690f7763c68ea1339.mailgun.org"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            using (var client = new SmtpClient())
            {
                client.LocalDomain = "sandbox51c62c88b46e413690f7763c68ea1339.mailgun.org";
                
                await client.ConnectAsync("https://api.mailgun.net/v3", 25, SecureSocketOptions.None).ConfigureAwait(false);
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }

            return; // "Email sent!";


        }
        */


        /*
        public static RestResponse SendSimpleMessage()
        {
            RestClient client = new RestClient();
            client.BaseUrl = "https://api.mailgun.net/v3";
            client.Authenticator =
                   new HttpBasicAuthenticator("api",
                                              "key-62ead3e0fd2a1f3723d34bff09f136fe");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                "sandbox51c62c88b46e413690f7763c68ea1339.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <postmaster@sandbox51c62c88b46e413690f7763c68ea1339.mailgun.org>");
            request.AddParameter("to", "Joel Baticz <joel.baticz@gmail.com>");
            request.AddParameter("subject", "Hello Joel Baticz");
            request.AddParameter("text", "Congratulations Joel Baticz, you just sent an email with Mailgun!  You are truly awesome!  You can see a record of this email in your logs: https://mailgun.com/cp/log .  You can send up to 300 emails/day from this sandbox server.  Next, you should add your own domain so you can send 10,000 emails/month for free.");
            request.Method = Method.POST;
            return client.Execute(request);
        }
        */


    }
}
