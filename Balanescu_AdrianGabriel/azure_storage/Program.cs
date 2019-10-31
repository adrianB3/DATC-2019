using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace azure_storage
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
            new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
            "datcjoi800", "Lv1zRf3Qr8gGMLu5E513ZaRymsOJusnvYWCgPPU40tGvZ5Yn7TN75e4O1sqnUTLpWnBeWy+3Iacv+Z2tfSYP8A=="), true);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable mytable = tableClient.GetTableReference("adrianb");
            mytable.CreateIfNotExistsAsync().Wait();

            var transaction1 = new Transaction("contemitent12345");
            transaction1.banca = "ING";
            transaction1.contBeneficiar = "contbeneficiar12345";
            transaction1.numeBeneficiar = "Ion";
            transaction1.numeEmitent = "Adrian";
            transaction1.suma = 100f;
            
            TableOperation insertOperation = TableOperation.Insert(transaction1);

            // Execute the insert operation.
            mytable.ExecuteAsync(insertOperation).Wait();

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<Transaction> query = new TableQuery<Transaction>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "contemitent12345"));

            // Print the fields for each customer.
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<Transaction> resultSegment = mytable.ExecuteQuerySegmentedAsync(query, token).Result;
                token = resultSegment.ContinuationToken;

                foreach (Transaction entity in resultSegment.Results)
                {
                    Console.WriteLine("{0}, {1}\t{2}\t{3}\t{4}\t{5}", entity.PartitionKey, entity.RowKey,
                    entity.numeBeneficiar, entity.numeEmitent, entity.suma, entity.Timestamp);
                }
            } while (token != null);
        }
    }
}
