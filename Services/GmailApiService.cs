using CoachEmailGenerator.Models;
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
using System.Text.RegularExpressions;
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

        public void CreateEmail(GoogleCredential cred, string userEmailAddress, EmailTemplate emailTemplate, List<School> schools)
        {
            // Create the Gmail API service
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,
                ApplicationName = "Coach Email Web Client"
            });

            foreach (var school in schools)
            {
                var scrubbedEmailSubject = ScrubSubjectLineTags(emailTemplate.EmailSubjectLine, school);
                var scrubbedEmailBodyText = ScrubEmailBodyTags(emailTemplate.EmailBody, school);

                CreateGmailDraft(service, userEmailAddress, scrubbedEmailBodyText, scrubbedEmailSubject, school.HeadCoach.Email);
            }

            //var tagValues = new Dictionary<string, string>()
            //{
            //    { "coach-name", "RYAN LIFFERTH" },
            //    { "school-name", "BYU" }
            //};
            //var scrubbedEmailText = ReplaceEmailTags(emailText, tagValues);

            //CreateGmailDraft(service, userEmailAddress, scrubbedEmailText);

        }

        private void CreateGmailDraft(GmailService service, string username, string emailBodyText, string emailSubject, string emailTo)
        {
            // Create the email body
            var email = CreateEmail(emailBodyText, emailSubject, emailTo);
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

        private MailMessage CreateEmail(string emailBody, string emailSubject, string emailTo)
        {
            // Create the message first
            var msg = new MailMessage()
            {
                Subject = emailSubject,
                //Body = "This is just a test",
                Body = emailBody,
                IsBodyHtml = true,
            };
            //msg.To.Add(new MailAddress("coach@school.edu", "Coach Coachy"));
            msg.To.Add(new MailAddress(emailTo));

            return msg;
        }

        private string ScrubEmailBodyTags(string emailText, School school)
        {
            string scrubbedText = emailText;
            string pattern = String.Empty;

            var headCoachLastName = (Regex.Match(school.HeadCoach.Name, "[^ ]* (.*)") != null &&
                                     Regex.Match(school.HeadCoach.Name, "[^ ]* (.*)").Length >= 1) ? 
                                     Regex.Match(school.HeadCoach.Name, "[^ ]* (.*)").Groups[1].Value :
                                     school.HeadCoach.Name;
            var coachName = "Coach " + headCoachLastName;

            var tags = new Dictionary<string, string>
                {
                    { "school-name", school.SchoolName },
                    { "school-name-short",  string.IsNullOrEmpty(school.SchoolNameShort) ? string.Empty : school.SchoolNameShort },
                    { "coach-name", coachName },
                    { "coach-email", string.IsNullOrEmpty(school.HeadCoach.Email) ? string.Empty : school.HeadCoach.Email },
                    { "coach-phone", string.IsNullOrEmpty(school.HeadCoach.PhoneNumber) ? string.Empty : school.HeadCoach.PhoneNumber }
                };

            var tags2 = new Dictionary<string, KeyValuePair<string, string>>
                {
                    { "school-name", new KeyValuePair<string, string>("[SCHOOL]", school.SchoolName) },
                    { "school-name-short", new KeyValuePair<string, string>("[SCHOOL NAME SHORT]",string.IsNullOrEmpty(school.SchoolNameShort) ? string.Empty : school.SchoolNameShort) },
                    { "coach-name", new KeyValuePair<string, string>("[COACH NAME]",coachName) },
                    { "coach-email", new KeyValuePair<string, string>("[COACH EMAIL]",string.IsNullOrEmpty(school.HeadCoach.Email) ? string.Empty : school.HeadCoach.Email) },
                    { "coach-phone", new KeyValuePair<string, string>("[COACH PHONE]",string.IsNullOrEmpty(school.HeadCoach.PhoneNumber) ? string.Empty : school.HeadCoach.PhoneNumber) }
                };

            foreach (var item in tags2)
            {
                // Example of match
                //<span class="coach-button mceNonEditable" data-school-info="coach-email">[COACH EMAIL]</span>
                
                // TODO:  I can't get the REGEX to work properly.  Matches some and not others (actually matches long strings of <span data-school-info....).
                //pattern = $"<span .*? data-school-info=\"{item.Key}\">(.|\n)*?<\\/span>";
                //scrubbedText = Regex.Replace(scrubbedText, pattern, item.Value);
                
                var replacePattern = $@"<span class=""coach-button mceNonEditable"" data-school-info=""{item.Key}"">{item.Value.Key}</span>";
                scrubbedText = scrubbedText.Replace(replacePattern, item.Value.Value);
            }

            return scrubbedText;
        }

        private string ScrubSubjectLineTags(string subjectText, School school)
        {
            var scrubbedSubjectLine = subjectText;
            var results = scrubbedSubjectLine.Split('[', ']').Where((item, index) => index % 2 != 0).Select(x => '[' + x + ']').ToList();

            foreach (var item in results)
            {
                // get the right tag
                var schoolValue = item.ToUpper() switch
                {
                    var x when
                    x == "[SCHOOL]" => school.SchoolName,
                    "[SCHOOL NAME SHORT]" => string.IsNullOrEmpty(school.SchoolNameShort) ? string.Empty : school.SchoolNameShort,
                    "[COACH NAME]" => string.IsNullOrEmpty(school.HeadCoach.Name) ? string.Empty : school.HeadCoach.Name,
                    "[COACH EMAIL]" => string.IsNullOrEmpty(school.HeadCoach.Email) ? string.Empty : school.HeadCoach.Email,
                    "[COACH PHONE]" => string.IsNullOrEmpty(school.HeadCoach.PhoneNumber) ? string.Empty : school.HeadCoach.PhoneNumber,
                    _ => ""
                };

                scrubbedSubjectLine = scrubbedSubjectLine.Replace(item, schoolValue);
            }

            return scrubbedSubjectLine;
        }


       
    }
}


