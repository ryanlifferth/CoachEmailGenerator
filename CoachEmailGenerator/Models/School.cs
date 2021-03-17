using Microsoft.Azure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Models
{
    public class School : TableEntity
    {

        public School(string userName, string Id)
        {
            this.PartitionKey = userName;
            this.RowKey = Id;
        }

        public School() { }

        public Guid Id { get; set; }
        public string SchoolName { get; set; }
        public string SchoolNameShort { get; set; }
        public string CoachName { get; set; }
        public string CoachEmail { get; set; }
        public string CoachPhoneNumber { get; set; }
        public bool IsEnabled { get; set; }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, Microsoft.WindowsAzure.Storage.OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            this.Id = Guid.Parse(this.RowKey);
        }

    }
}
