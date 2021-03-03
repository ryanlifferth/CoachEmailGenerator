using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Services
{
    public class GmailApiService
    {
        private readonly IConfiguration _config;

        public GoogleCredential GoogleAuthCredential { get; set; }


        public GmailApiService(IConfiguration config)
        {
            _config = config;
        }

        public void CreateEmail(GoogleCredential cred, string emailText, string userEmailAddress)
        {
            // Create the Gmail API service
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,
                ApplicationName = "Coach Email Web Client"
            });

            
            CreateGmailDraft(service, userEmailAddress, emailText);

        }

        private void CreateGmailDraft(GmailService service, string username, string emailBodyText)
        {
            // Create the email body
            var email = CreateEmail(emailBodyText);
            var message = new Message();
            message.Raw = EncodeMessage(email);

            // Create a draft of an e-mail
            var draft = new Draft();
            draft.Message = message;
            draft = service.Users.Drafts.Create(draft, username).Execute();

            Console.WriteLine($"Test draft created");

        }

        private string EncodeMessage(MailMessage message)
        {
            var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(message);

            //m.Raw = Convert.ToBase64String(Encoding.UTF8.GetBytes(msg.ToString()));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(mimeMessage.ToString())).Replace('+', '-').Replace('/', '_').Replace("=", "");
        }

        private MailMessage CreateEmail(string emailBody)
        {
            // Create the message first
            var msg = new MailMessage()
            {
                Subject = $"Zach Lifferth - TEST SCHOOL U Soccer",
                //Body = "This is just a test",
                Body = emailBody,
                IsBodyHtml = true,
            };
            msg.To.Add(new MailAddress("coach@school.edu", "Coach Coachy"));

            return msg;
        }

    }
}


