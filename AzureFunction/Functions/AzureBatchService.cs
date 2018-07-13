using System;
using System.Threading.Tasks;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;
using Microsoft.Azure.WebJobs.Host;

namespace Functions
{
    class AzureBatchService
    {
        private const string BatchUrl = "BatchUrl";
        private const string AccountName = "AccountName";

        private const string AccountKey = "AccountKey";
        private int? _messageCount;

        public AzureBatchService(int? messageCount)
        {
            this._messageCount = messageCount;
        }

        public async Task HandleScalingAsync()
        {
            var credentials = new BatchSharedKeyCredentials(BatchUrl, AccountName, AccountKey);

            using (BatchClient batchClient = await BatchClient.OpenAsync(credentials))
            {
                var poolService = new PoolService();

                try
                {
                    var pool = await poolService.CreatePoolIfNotExistAsync(batchClient, "pool-job");

                    if (pool.AllocationState == AllocationState.Resizing)
                    {
                        return;
                    }

                    if (_messageCount.GetValueOrDefault() >= 10 
                        && pool.TargetDedicatedComputeNodes != 2)
                    {
                        await UpScaleAsync(pool);
                        return;
                    }

                    if (_messageCount.GetValueOrDefault() < 10 
                        && pool.TargetDedicatedComputeNodes > 1)
                    {
                        await DownScale(pool);
                        return;
                    }

                }
                catch (Exception ex)
                {
                    var batchException = ex as BatchException;

                    if (batchException?.RequestInformation?.BatchError != null)
                    {
                        Console.WriteLine(batchException.RequestInformation.BatchError.Code);
                        Console.WriteLine(batchException.RequestInformation.BatchError.Message.Value);
                    }
                }
            }
        }

        private static async Task DownScale(CloudPool pool)
        {
            await pool.ResizeAsync(1, 0, TimeSpan.FromMinutes(200), ComputeNodeDeallocationOption.TaskCompletion);
        }

        private static async Task UpScaleAsync(CloudPool pool)
        {
            await pool.ResizeAsync(2, 0, TimeSpan.FromMinutes(200), ComputeNodeDeallocationOption.TaskCompletion);
        }
    }
}
