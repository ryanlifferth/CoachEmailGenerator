using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Models
{
    public class EmailTemplate
    {
        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
        public string EmailSubjectLine { get; set; }
        public string EmailBody { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastEditedDate { get; set; }

    }
}
