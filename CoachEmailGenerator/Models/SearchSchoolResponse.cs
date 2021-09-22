using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Models
{
    public class SearchSchoolResponse : TableEntity
    {

        public SearchSchoolResponse() { }

        public SearchSchoolResponse(string compoundKey, string coachEmail)
        {
            this.PartitionKey = compoundKey;
            this.RowKey = coachEmail;
        }

        public string SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string SchoolNameShort { get; set; }

        public List<Coach> Coaches { get; set; }

        public string Division { get; set; }
        public string Conference { get; set; }
        public bool IsEnabled { get; set; }

    }
}
