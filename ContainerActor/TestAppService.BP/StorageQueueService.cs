using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppService.BP
{
    public interface IStorageQueueService
    {
        Task RemoveMessageAsync(string guid);
    }
    public class StorageQueueService : IStorageQueueService
    {
        public async Task RemoveMessageAsync(string guid)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
               new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                   "AccountName",
                   "AccountKey"), true);

            // Create the CloudQueueClient object for the storage account.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Get a reference to the CloudQueue named "messagequeue"
            CloudQueue messageQueue = queueClient.GetQueueReference("tasks-to-queue");
            messageQueue.FetchAttributes();

            int? storageQueueLength = messageQueue.ApproximateMessageCount;
            CloudQueueMessage messageToDelete = null;

            if (storageQueueLength.HasValue)
            {
                var messages = await messageQueue.GetMessagesAsync(storageQueueLength.Value);
                int i = 0;

                while (messageToDelete == null && i < storageQueueLength.Value)
                {
                    var task = JsonConvert.DeserializeObject<StorageTask>(messages.ElementAt(i).AsString);

                    if (task.Id.ToString() == guid)
                    {
                        messageToDelete = messages.ElementAt(i);
                    }

                    i++;
                }
            }

            await messageQueue.DeleteMessageAsync(messageToDelete);
        }

        public class StorageTask
        {
            public Guid Id { get; set; }

            public string Title { get; set; }

            public List<Service> Services { get; set; }

            public bool IsSuccess { get; set; }
        }

        public enum Service
        {
            Dematerialization = 0,

            Adr = 1,

            Signing = 2
        }
    }
}
