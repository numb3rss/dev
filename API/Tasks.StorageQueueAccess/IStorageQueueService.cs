using Tasks.BO;
using System.Threading.Tasks;

namespace Tasks.StorageQueueAccess
{
    public interface IStorageQueueService
    {
        Task<StorageTask> GetMessageAsync();

        Task AddMessageAsync(StorageTask storageTask);
    }
}
