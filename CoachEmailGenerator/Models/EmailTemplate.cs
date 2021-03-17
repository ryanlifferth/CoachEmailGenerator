using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace CoachEmailGenerator.Models
{
    public class EmailTemplate : TableEntity
    {

        public EmailTemplate(string userName, string Id)
        {
            this.PartitionKey = userName;
            this.RowKey = Id;
        }

        public EmailTemplate() { }

        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
        public string EmailSubjectLine { get; set; }
        public string EmailBody { get; set; }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, Microsoft.WindowsAzure.Storage.OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            this.Id = Guid.Parse(this.RowKey);
        }

    }
}
