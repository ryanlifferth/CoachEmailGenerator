using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoachEmailGenerator.Helpers
{
    public static class AzureHelper
    {

        public static async Task<IEnumerable<T>> QueryTable<T>(CloudTable cloudTable, string filterCondition) where T : TableEntity, new()
        {

            var results = new List<T>();
            TableContinuationToken token = null;
            var cancellationToken = default(CancellationToken);

            var query = new TableQuery<T>().Where(filterCondition);

            do
            {
                //var query = new TableQuery<School>().Where(condition);
                var seg = await cloudTable.ExecuteQuerySegmentedAsync(query, token);
                token = seg.ContinuationToken;
                results.AddRange(seg);
            } while (token != null && !cancellationToken.IsCancellationRequested);

            return results;
        }

    }
}
