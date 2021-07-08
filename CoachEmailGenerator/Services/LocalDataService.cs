using CoachEmailGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CoachEmailGenerator.Helpers;
using CoachEmailGenerator.Interfaces;

namespace CoachEmailGenerator.Services
{
    public class LocalDataService : IDataService
    {

        private string _filePath = Directory.GetCurrentDirectory() + "\\Data\\";

        public EmailTemplate GetEmailTemplateByEmailAddress(string emailAddress)
        {
            var fileName = Helper.GetUserNameFromEmail(emailAddress) + "-template.json";
            var fullFilePath = _filePath + fileName;

            var jsonString = File.Exists(fullFilePath) ? File.ReadAllText(fullFilePath) : string.Empty;
            return !string.IsNullOrEmpty(jsonString) ? JsonSerializer.Deserialize<EmailTemplate>(jsonString) : null;
        }

        public Guid SaveEmailTemplate(EmailTemplate emailTemplate)
        {
            var now = DateTime.Now;
            var fileName = Helper.GetUserNameFromEmail(emailTemplate.EmailAddress) + "-template.json";
            var fullFilePath = _filePath + fileName;

            // See if this is new or edit
            if (emailTemplate.Id == Guid.Empty)
            {
                emailTemplate.Id = Guid.NewGuid();
                //emailTemplate.CreatedDate = now;
            }

            //emailTemplate.LastEditedDate = now;

            var jsonString = JsonSerializer.Serialize(emailTemplate);
            File.WriteAllText(fullFilePath, jsonString);

            return emailTemplate.Id;
        }

        public List<School> GetSchoolsByEmailAddress(string userEmail)
        {
            // Load JSON from file
            var fileName = Helper.GetUserNameFromEmail(userEmail) + "-schools.json";
            var fullFilePath = _filePath + fileName;

            var jsonString = System.IO.File.Exists(fullFilePath) ? System.IO.File.ReadAllText(fullFilePath) : string.Empty;

            return !string.IsNullOrEmpty(jsonString) ? JsonSerializer.Deserialize<List<School>>(jsonString) : null;
        }

        public void SaveSchools(string userEmail, List<School> schools)
        {
            var fileName = Helper.GetUserNameFromEmail(userEmail) + "-schools.json";
            var fullFilePath = _filePath + fileName;

            System.IO.File.WriteAllText(fullFilePath, JsonSerializer.Serialize(schools));
        }

        public void SaveSchool(string userEmail, School school)
        {
            var fileName = Helper.GetUserNameFromEmail(userEmail) + "-schools.json";
            var fullFilePath = _filePath + fileName;

            var jsonString = System.IO.File.Exists(fullFilePath) ? System.IO.File.ReadAllText(fullFilePath) : string.Empty;
            var schools = !string.IsNullOrEmpty(jsonString) ? JsonSerializer.Deserialize<List<School>>(jsonString) : null;

            if (schools != null && school.Id != Guid.Empty)
            {
                foreach (var item in schools.Where(x => x.Id == school.Id))
                {
                    item.CoachEmail = school.CoachEmail;
                    item.CoachName = school.CoachName;
                    item.CoachPhoneNumber = school.CoachPhoneNumber;
                    item.IsEnabled = school.IsEnabled;
                    item.SchoolName = school.SchoolName;
                    item.SchoolNameShort = school.SchoolNameShort;
                }
            }

            System.IO.File.WriteAllText(fullFilePath, JsonSerializer.Serialize(schools));
        }

        public void DeleteSchool(string userEmail, School school)
        {
            var fileName = Helper.GetUserNameFromEmail(userEmail) + "-schools.json";
            var fullFilePath = _filePath + fileName;

            var jsonString = System.IO.File.Exists(fullFilePath) ? System.IO.File.ReadAllText(fullFilePath) : string.Empty;
            var schools = !string.IsNullOrEmpty(jsonString) ? JsonSerializer.Deserialize<List<School>>(jsonString) : null;

            if (schools != null && school.Id != Guid.Empty)
            {
                schools.Remove(schools.FirstOrDefault(x => x.Id == school.Id));
            }
            
            // save the file
            SaveSchools(userEmail, schools);
        }

    }
}
