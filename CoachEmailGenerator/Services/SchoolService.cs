using CoachEmailGenerator.Helpers;
using CoachEmailGenerator.Interfaces;
using CoachEmailGenerator.Models;
using CoachEmailGenerator.Models.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Services
{
    public class SchoolService : ISchoolService
    {

        private CloudTableClient _cloudTableClient;
        private CloudTable _cloudTable;
        private readonly IConfiguration _config;

        public SchoolService(IConfiguration config)
        {
            _config = config;

            var storageAccount = CloudStorageAccount.Parse(_config["AzureTableConnectionString"]);
            _cloudTableClient = storageAccount.CreateCloudTableClient();
        }

        public List<CoachesResponse> SearchSchools(CoachesSearchRequest request)
        {
            if (request == null)
            {
                throw new Exception("Bad request - missing search criteria");
            }

            if (string.IsNullOrEmpty(request.PartitionKey))
            {
                request.PartitionKey = "M_SOCCER_NCAA_D1";
            }

            _cloudTable = _cloudTableClient.GetTableReference("COACHES");
            var queryCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, request.PartitionKey);
            
            var schoolNameFilter1 = TableQuery.GenerateFilterCondition("SchoolName", QueryComparisons.GreaterThanOrEqual, request.SchoolName);
            var schoolNameFilter2 = TableQuery.GenerateFilterCondition("SchoolName", QueryComparisons.LessThan, request.SchoolName + "~");
            var schoolNameFilter = TableQuery.CombineFilters(schoolNameFilter1, TableOperators.And, schoolNameFilter2);

            var schoolNameShortFilter1 = TableQuery.GenerateFilterCondition("SchoolNameShort", QueryComparisons.GreaterThanOrEqual, request.SchoolName);
            var schoolNameShortFilter2 = TableQuery.GenerateFilterCondition("SchoolNameShort", QueryComparisons.LessThan, request.SchoolName + "~");
            var schoolNameShortFilter = TableQuery.CombineFilters(schoolNameShortFilter1, TableOperators.And, schoolNameShortFilter2);

            var schoolFilter = TableQuery.CombineFilters(schoolNameFilter, TableOperators.Or, schoolNameShortFilter);
            
            var filters = TableQuery.CombineFilters(queryCondition, TableOperators.And, schoolFilter);

            var queryTask = Task.Run(async () => await AzureHelper.QueryTable<CoachesResponse>(_cloudTable, filters));
            var result = queryTask.GetAwaiter().GetResult().ToList();

            return result;
        }

        public List<CoachesResponse> FindSchool(CoachesSearchRequest request)
        {
            if (request == null)
            {
                throw new Exception("Bad request - missing search criteria");
            }

            if (string.IsNullOrEmpty(request.PartitionKey))
            {
                request.PartitionKey = "M_SOCCER_NCAA_D1";
            }

            _cloudTable = _cloudTableClient.GetTableReference("COACHES");
            var partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, request.PartitionKey);
            var schoolFilter = TableQuery.GenerateFilterCondition("SchoolName", QueryComparisons.Equal, request.SchoolName);
            var filters = TableQuery.CombineFilters(partitionFilter, TableOperators.And, schoolFilter);

            var queryTask = Task.Run(async () => await AzureHelper.QueryTable<CoachesResponse>(_cloudTable, filters));
            var result = queryTask.GetAwaiter().GetResult().ToList();

            return result;
        }

    }
}
