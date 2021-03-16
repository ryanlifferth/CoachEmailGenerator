using CoachEmailGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Services
{
    public class DataService
    {

        private string _filePath = Directory.GetCurrentDirectory() + "\\Data\\";

        public EmailTemplate LoadTemplateFromJsonSource(string emailAddress)
        {
            var fileName = GetUserNameFromEmail(emailAddress) + "-template.json";
            var fullFilePath = _filePath + fileName;

            var jsonString = File.Exists(fullFilePath) ? File.ReadAllText(fullFilePath) : string.Empty;
            return !string.IsNullOrEmpty(jsonString) ? JsonSerializer.Deserialize<EmailTemplate>(jsonString) : null;
        }

        public Guid SaveTemplate(EmailTemplate emailTemplate)
        {
            var now = DateTime.Now;
            var fileName = GetUserNameFromEmail(emailTemplate.EmailAddress) + "-template.json";
            var fullFilePath = _filePath + fileName;

            // See if this is new or edit
            if (emailTemplate.Id == Guid.Empty)
            {
                emailTemplate.Id = Guid.NewGuid();
                emailTemplate.CreatedDate = now;
            }

            emailTemplate.LastEditedDate = now;

            var jsonString = JsonSerializer.Serialize(emailTemplate);
            File.WriteAllText(fullFilePath, jsonString);

            return emailTemplate.Id;
        }

        public string GetUserNameFromEmail(string emailAddress)
        {
            var addr = new System.Net.Mail.MailAddress(emailAddress);
            //string name = addr.User;
            //string domain = addr.Host;
            return addr.User;
        }

        public List<School> LoadSchoolListFromJsonSource(string userEmail)
        {
            // Load JSON from file
            var fileName = GetUserNameFromEmail(userEmail) + "-schools.json";
            var fullFilePath = _filePath + fileName;

            var jsonString = System.IO.File.Exists(fullFilePath) ? System.IO.File.ReadAllText(fullFilePath) : string.Empty;

            return !string.IsNullOrEmpty(jsonString) ? JsonSerializer.Deserialize<List<School>>(jsonString) : null;
        }

        public void SaveSchoolListBackToJsonSource(string userEmail, List<School> schools)
        {
            var fileName = GetUserNameFromEmail(userEmail) + "-schools.json";
            var fullFilePath = _filePath + fileName;

            System.IO.File.WriteAllText(fullFilePath, JsonSerializer.Serialize(schools));
        }

    }
}
