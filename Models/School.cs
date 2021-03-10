using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Models
{
    public class School
    {
        public Guid Id { get; set; }
        public string SchoolName { get; set; }
        public string SchoolNameShort { get; set; }
        public Coach HeadCoach { get; set; }
        public Coach AssistantCoach { get; set; }
        public bool IsEnabled { get; set; }
    }
}
