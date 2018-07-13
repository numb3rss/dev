using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tasks.BO;
using Tasks.BO.Input;

namespace Tasks.StorageQueueAccess
{
    [ExcludeFromCodeCoverage]
    public class StorageQueueService : IStorageQueueService
    {
        private readonly AppSettingsModel _appSettings;

        public StorageQueueService(IOptions<AppSettingsModel> settings)
        {
            _appSettings = settings.Value;
        }

        public async Task AddMessageAsync(StorageTask storageTask)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    _appSettings.AccountName,
                    _appSettings.AccountKey), true);

            // Create the CloudQueueClient object for the storage account.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Get a reference to the CloudQueue named "messagequeue"
            CloudQueue messageQueue = queueClient.GetQueueReference(_appSettings.StorageQueueName);

            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(storageTask, Formatting.Indented));
            await messageQueue.AddMessageAsync(message);
        }

        public async Task<StorageTask> GetMessageAsync()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    _appSettings.AccountName,
                    _appSettings.AccountKey), true);

            // Create the CloudQueueClient object for the storage account.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Get a reference to the CloudQueue named "messagequeue"
            CloudQueue messageQueue = queueClient.GetQueueReference(_appSettings.StorageQueueName);

            // Async dequeue the message.
            CloudQueueMessage retrievedMessage = await messageQueue.PeekMessageAsync();
            return JsonConvert.DeserializeObject<StorageTask>(retrievedMessage.AsString);
        }
    }
}
