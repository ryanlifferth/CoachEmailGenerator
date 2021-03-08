using CoachEmailGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Services
{
    public class SaveTemplateService
    {

        public Guid SaveTemplate(EmailTemplate emailTemplate)
        {
            var now = DateTime.Now;
            var path = Directory.GetCurrentDirectory() + "\\Data";
            var userName = GetUserNameFromEmail(emailTemplate.EmailAddress);

            // See if this is new or edit
            if (emailTemplate.Id == Guid.Empty)
            {
                emailTemplate.Id = Guid.NewGuid();
                emailTemplate.CreatedDate = now;
            }

            emailTemplate.LastEditedDate = now;

            var jsonString = JsonSerializer.Serialize(emailTemplate);
            File.WriteAllText(path + "\\" + userName + "-template.json", jsonString);

            return emailTemplate.Id;
        }

        public string GetUserNameFromEmail(string emailAddress)
        {
            var addr = new System.Net.Mail.MailAddress(emailAddress);
            //string name = addr.User;
            //string domain = addr.Host;
            return addr.User;
        }

    }
}
