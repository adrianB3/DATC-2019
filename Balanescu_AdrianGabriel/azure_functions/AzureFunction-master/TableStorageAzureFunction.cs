using System;
using System.Collections.Generic;
using System.Linq;
using AzureFunction.Models;
using AzureFunction.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunction
{
    public static class TableStorageAzureFunction
    {
        public static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=datcjoi800;AccountKey=jqVCgNlrVBhv4W1HDVmzA3G+UVgtESCDayL9Zwmxn0fFAvDXK2WuUETxl2kf1fWTeDDC6kp5yeUVI+OHqbqfrQ==;EndpointSuffix=core.windows.net";
        public static string My_Name = "adrianb";

        [FunctionName("TimerTriggerCSharp")]
        public static void Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var bankTransactionRepository = new TableStorageService<BankTransaction>(log, ConnectionString, $"{My_Name}BankTransaction");
            var bankTransactionStatisticsRepository = new TableStorageService<BankTransactionStatistics>(log, ConnectionString, $"{My_Name}BankTransactionStatistics");

            SeedData(bankTransactionRepository);

            // Generate BankTransactionStatistics from all senders by aggregating the total sum of their transactions
            
            var entries = bankTransactionRepository.GetAllEntries();
            var listOfUsers = new Dictionary<string, double>();

            foreach(var user in entries){
              if(listOfUsers.ContainsKey(user.PartitionKey))
                listOfUsers[user.PartitionKey] += user.Ammount;
              else
                listOfUsers.Add(user.PartitionKey, user.Ammount);
            }

            foreach(var stat in listOfUsers)
            {
              bankTransactionStatisticsRepository.InsertOrUpdateEntry(new BankTransactionStatistics());
            }
        }

        private static void SeedData(TableStorageService<BankTransaction> tableStorageService)
        {
            var transactions = new BankTransaction[]
            {
                new BankTransaction("1", "John Smith")
                {
                    Ammount = new Random().Next(1, 100),
                    DestinationAccount = Guid.NewGuid().ToString(),
                    DestinationName = "Amanda"
                },
                new BankTransaction("2", "John Smith")
                {
                    Ammount = new Random().Next(1, 100),
                    DestinationAccount = Guid.NewGuid().ToString(),
                    DestinationName = "Elton"
                },
                new BankTransaction("3", "Elton James")
                {
                    Ammount = new Random().Next(1, 100),
                    DestinationAccount = Guid.NewGuid().ToString(),
                    DestinationName = "Amanda"
                },
                new BankTransaction("4", "John Smith")
                {
                    Ammount = new Random().Next(1, 100),
                    DestinationAccount = Guid.NewGuid().ToString(),
                    DestinationName = "Elton"
                },
                new BankTransaction("5", "Elton James")
                {
                    Ammount = new Random().Next(1, 100),
                    DestinationAccount = Guid.NewGuid().ToString(),
                    DestinationName = "Amanda"
                },
                new BankTransaction("6", "Amanda Jefferson")
                {
                    Ammount = new Random().Next(1, 100),
                    DestinationAccount = Guid.NewGuid().ToString(),
                    DestinationName = "John"
                },
                new BankTransaction("7", "Amanda Jefferson")
                {
                    Ammount = new Random().Next(1, 100),
                    DestinationAccount = Guid.NewGuid().ToString(),
                    DestinationName = "Elton"
                },
                new BankTransaction("8", "John Smith")
                {
                    Ammount = new Random().Next(1, 100),
                    DestinationAccount = Guid.NewGuid().ToString(),
                    DestinationName = "Elton"
                },
                new BankTransaction("9", "Amanda Jefferson")
                {
                    Ammount = new Random().Next(1, 100),
                    DestinationAccount = Guid.NewGuid().ToString(),
                    DestinationName = "Elton"
                },
            };

            foreach(var transaction in transactions)
            {
                tableStorageService.InsertOrUpdateEntry(transaction);
            }
        }
    }
}
