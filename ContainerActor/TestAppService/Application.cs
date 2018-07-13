using TestAppService.BP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestAppService
{
    public interface IApplication
    {
        void Run(string guid, int duration);
    }

    public class Application : IApplication
    {
        private IMessageService _messageService;
        private IStorageQueueService _storageQueueService;

        public Application(IMessageService messageService, IStorageQueueService storageQueueService)
        {
            _messageService = messageService;
            _storageQueueService = storageQueueService;
        }

        public void Run(string guid, int duration)
        {
            Console.WriteLine(_messageService.Get());
            Thread.Sleep(TimeSpan.FromMinutes(duration == 0 ? 5 : duration));
            _storageQueueService.RemoveMessageAsync(guid).GetAwaiter().GetResult();
        }
    }
}
