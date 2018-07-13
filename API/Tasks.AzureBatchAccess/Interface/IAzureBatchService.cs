using Microsoft.Azure.Batch;
using Tasks.BO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.AzureBatchAccess
{
    public interface IAzureBatchService
    {
        Task CreateJobAsync(StorageTask task);
    }
}
