﻿using CoachEmailGenerator.Interfaces;
using CoachEmailGenerator.Models;
using CoachEmailGenerator.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using CoachEmailGenerator.Common;

namespace CoachEmailGenerator.Services
{
    public class AzureDataService : IDataService
    {

        private CloudTableClient _cloudTableClient;
        private CloudTable _cloudTable;
        private readonly IConfiguration _config;

        public AzureDataService(IConfiguration config)
        {
            _config = config;

            var storageAccount = CloudStorageAccount.Parse(_config["AzureTableConnectionString"]);
            _cloudTableClient = storageAccount.CreateCloudTableClient();
        }


        public EmailTemplate GetEmailTemplateByEmailAddress(string emailAddress)
        {
            _cloudTable = _cloudTableClient.GetTableReference("EmailTemplate");

            var queryCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Helper.GetUserNameFromEmail(emailAddress));

            var queryTask = Task.Run(async () => await AzureHelper.QueryTable<EmailTemplate>(_cloudTable, queryCondition));
            var result = queryTask.GetAwaiter().GetResult().ToList().FirstOrDefault();

            return result;
        }

        public List<School> GetSchoolsByEmailAddress(string userEmail)
        {
            _cloudTable = _cloudTableClient.GetTableReference("Schools");

            var userName = Helper.GetUserNameFromEmail(userEmail);
            var queryCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userName);

            var queryTask = Task.Run(async () => await AzureHelper.QueryTable<School>(_cloudTable, queryCondition));
            var result = queryTask.GetAwaiter().GetResult().ToList();

            return result;
        }

        public Guid SaveEmailTemplate(EmailTemplate emailTemplate)
        {
            _cloudTable = _cloudTableClient.GetTableReference("EmailTemplate");

            // See if this is new or edit
            if (emailTemplate.Id == Guid.Empty)
            {
                emailTemplate.Id = Guid.NewGuid();
            }

            if (string.IsNullOrEmpty(emailTemplate.PartitionKey))
            {
                emailTemplate.PartitionKey = Helper.GetUserNameFromEmail(emailTemplate.EmailAddress);
            }

            if (string.IsNullOrEmpty(emailTemplate.RowKey))
            {
                emailTemplate.RowKey = emailTemplate.Id.ToString();
            }

            var saveTask = Task.Run(async () => await _cloudTable.ExecuteAsync(TableOperation.InsertOrReplace(emailTemplate)));
            var result = saveTask.GetAwaiter().GetResult();

            return emailTemplate.Id;
        }

        public void SaveSchools(string userEmail, List<School> schools)
        {
            _cloudTable = _cloudTableClient.GetTableReference("Schools");
            var batchOperation = new TableBatchOperation();

            var batches = schools.Batch(100);
            foreach (var schoolBatch in batches)
            {
                batchOperation = new TableBatchOperation();

                foreach (var school in schoolBatch)
                {
                    // See if this is new or edit
                    if (school.Id == Guid.Empty)
                    {
                        school.Id = Guid.NewGuid();
                    }

                    if (string.IsNullOrEmpty(school.PartitionKey))
                    {
                        school.PartitionKey = Helper.GetUserNameFromEmail(userEmail);
                    }

                    if (string.IsNullOrEmpty(school.RowKey))
                    {
                        school.RowKey = school.Id.ToString();
                    }

                    batchOperation.InsertOrReplace(school);
                    //var saveTask = Task.Run(async () => await _cloudTable.ExecuteAsync(TableOperation.InsertOrReplace(school)));
                }
                var saveTask = Task.Run(async () => await _cloudTable.ExecuteBatchAsync(batchOperation));
                var result = saveTask.GetAwaiter().GetResult();
            }


        }

        public void SaveSchool(string userEmail, School school)
        {
            _cloudTable = _cloudTableClient.GetTableReference("Schools");
            if (string.IsNullOrEmpty(school.PartitionKey))
            {
                school.PartitionKey = Helper.GetUserNameFromEmail(userEmail);
            }

            if (string.IsNullOrEmpty(school.RowKey))
            {
                school.RowKey = school.Id.ToString();
            }

            var saveTask = Task.Run(async () => await _cloudTable.ExecuteAsync(TableOperation.InsertOrReplace(school)));
            var result = saveTask.GetAwaiter().GetResult();
        }

        public void DeleteSchool(string userEmail, School school)
        {
            _cloudTable = _cloudTableClient.GetTableReference("Schools");
            if (string.IsNullOrEmpty(school.PartitionKey))
            {
                school.PartitionKey = Helper.GetUserNameFromEmail(userEmail);
            }

            if (string.IsNullOrEmpty(school.RowKey))
            {
                school.RowKey = school.Id.ToString();
            }

            var deleteTask = Task.Run(async () => await _cloudTable.ExecuteAsync(TableOperation.Delete(school)));
            var result = deleteTask.GetAwaiter().GetResult();
        }

    }
}
