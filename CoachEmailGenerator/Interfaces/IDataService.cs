using CoachEmailGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Interfaces
{
    public interface IDataService
    {

        public EmailTemplate GetEmailTemplateByEmailAddress(string emailAddress);
        public Guid SaveEmailTemplate(EmailTemplate emailTemplate);
        public List<School> GetSchoolsByEmailAddress(string userEmail);
        public void SaveSchools(string userEmail, List<School> schools);
        public void DeleteSchool(string userEmail, School schools);



    }
}
