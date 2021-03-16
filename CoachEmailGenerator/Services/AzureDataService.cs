using CoachEmailGenerator.Interfaces;
using CoachEmailGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Services
{
    public class AzureDataService : IDataService
    {
        public EmailTemplate GetEmailTemplateByEmailAddress(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public List<School> GetSchoolsByEmailAddress(string userEmail)
        {
            throw new NotImplementedException();
        }

        public Guid SaveEmailTemplate(EmailTemplate emailTemplate)
        {
            throw new NotImplementedException();
        }

        public void SaveSchools(string userEmail, List<School> schools)
        {
            throw new NotImplementedException();
        }
    }
}
