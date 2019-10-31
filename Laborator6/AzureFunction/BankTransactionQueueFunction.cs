using AzureFunction.Models;
using AzureFunction.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace AzureFunction
{
    public static class BankTransactionQueueFunction
    {
        public const string QUEUE_NAME = "myqueue-1";
        
        public const string TABLE_STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=datcjoi800;AccountKey=OR8OdO+x18rLrZllupXTyl0dkk38Ul9/BWti2SurGQ/l9PT6UfyiFB7kweOkTrvsMXRXZlwERtnIp4mDIy0yDQ==;EndpointSuffix=core.windows.net";

        public const string MY_NAME = "adrianb";

        [FunctionName("BankTransactionQueueFunction")]
        public static void Run([ServiceBusTrigger(QUEUE_NAME, Connection = "ConnectionStringKey")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            var bankTransactionRepository = new TableStorageService<BankTransaction>(log, TABLE_STORAGE_CONNECTION_STRING, $"{MY_NAME}BankTransaction");
            var bankTransactionStatisticsRepository = new TableStorageService<BankTransactionStatistics>(log, TABLE_STORAGE_CONNECTION_STRING, $"{MY_NAME}BankTransactionStatistics");  

            // deserialize myQueueItem
            var deserializedObject = JsonConvert.DeserializeObject<QueueMessage>(myQueueItem);


            // add transaction to table storage
            bankTransactionRepository.InsertOrUpdateEntry(
                new BankTransaction(Guid.NewGuid().ToString(), deserializedObject.SenderName) {
                    Ammount = deserializedObject.Ammount,
                    DestinationAccount = deserializedObject.DestinationAccount,
                    DestinationName = deserializedObject.DestinationName
                });


            // update statistic
            var entities = bankTransactionRepository.GetAllEntries();
            var senders = entities.Select(e => e.PartitionKey).Distinct();

            foreach(var sender in senders)
            {
                var transactions = bankTransactionRepository.GetByPartitionId(sender);
                var totalAmmount = transactions.Sum(t => t.Ammount);

                var existingBankTransactionStatistics = bankTransactionStatisticsRepository.GetByPartitionId(sender).FirstOrDefault();

                if(existingBankTransactionStatistics != null)
                {
                    existingBankTransactionStatistics.Ammount = totalAmmount;
                }
                else
                {
                    existingBankTransactionStatistics = new BankTransactionStatistics(Guid.NewGuid().ToString(), sender)
                    {
                        Ammount = totalAmmount
                    };
                }

                bankTransactionStatisticsRepository.InsertOrUpdateEntry(existingBankTransactionStatistics);
            }

        }
    }
}
