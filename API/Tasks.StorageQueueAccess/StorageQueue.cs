using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Tasks.StorageQueueAccess
{
    public class StorageQueue<T> where T : BaseMessage, new()
    {
        protected CloudQueue Queue;

        public StorageQueue(CloudQueue queue)
        {
            this.Queue = queue;
        }

        public async Task AddMessageAsync(T message)
        {
            var msg = new CloudQueueMessage(string.Empty);
            msg.SetMessageContent(message.ToBinary());
            await Queue.AddMessageAsync(msg);
        }

        public void DeleteMessageAsync(CloudQueueMessage msg)
        {
            this.Queue.DeleteMessageAsync(msg);
        }

        public async Task<CloudQueueMessage> GetMessage()
        {
            return await Queue.GetMessageAsync(
                TimeSpan.FromSeconds(120), 
                new QueueRequestOptions(), 
                new OperationContext());
        }
    }
}
