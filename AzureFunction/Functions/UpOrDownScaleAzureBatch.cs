using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Functions
{
    public static class UpOrDownScaleAzureBatch
    {
        [FunctionName("UpOrDownScaleAzureBatch")]

        public static void Run(
            [TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            TraceWriter log)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
               new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                   "AccountName",
                   "AccountKey"), true);

            // Create the CloudQueueClient object for the storage account.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Get a reference to the CloudQueue named "messagequeue"
            CloudQueue messageQueue = queueClient.GetQueueReference("QueueName");
            messageQueue.FetchAttributes();

            var messageCount = messageQueue.ApproximateMessageCount;

            AzureBatchService azureBatchService = new AzureBatchService(messageCount);
            azureBatchService.HandleScalingAsync().GetAwaiter().GetResult();
        }
    }
}
