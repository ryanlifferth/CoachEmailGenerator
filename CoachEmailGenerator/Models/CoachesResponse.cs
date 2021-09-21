using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Models
{
    public class CoachesResponse : TableEntity
    {
        public CoachesResponse(string userName, string Id)
        {
            this.PartitionKey = userName;
            this.RowKey = Id;
        }

        public CoachesResponse() { }

        public string CoachName { get; set; }
        public string CoachTitle { get; set; }
        public string SchoolName { get; set; }
        public string SchoolNameShort { get; set; }
        public Guid SchoolId { get; set; }
        public string Sport { get; set; }
        public string Division { get; set; }
        public string Conference { get; set; }
        public bool IsEnabled { get; set; }
    }
}
