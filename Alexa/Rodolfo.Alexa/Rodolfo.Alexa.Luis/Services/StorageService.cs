using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Rodolfo.Alexa.Luis.Contracts;
using Rodolfo.Alexa.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodolfo.Alexa.Luis.Services
{
    public class StorageService : IStorageService
    {
        private CloudStorageAccount storageAccount;

        public StorageService(IConfiguration configuration)
        {
            this.storageAccount = CloudStorageAccount.Parse(configuration["App:StorageConnectionString"]);
        }
        public async Task<MessageEntity> GetLastMessageAsync()
        {
            var message = await GetAllAsync();

            return message.LastOrDefault();
        }

        public async Task<List<MessageEntity>> GetAllAsync()
        {
            try
            {
                var tableClient = storageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference("Instagram");

                var query = new TableQuery<MessageEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Instagram"));

                var results = new List<MessageEntity>();
                TableContinuationToken continuationToken = null;
                do
                {
                    var queryResults = await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                    continuationToken = queryResults.ContinuationToken;

                    results.AddRange(queryResults.Results);

                } while (continuationToken != null);

                return results.ToList();
            }
            catch (Exception)
            {
                return new List<MessageEntity>();
            }
        }
    }
}
