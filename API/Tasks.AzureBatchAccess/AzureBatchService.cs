using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;
using Tasks.BO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tasks.AzureBatchAccess
{
    public class AzureBatchService : IAzureBatchService
    {
        private const string BatchUrl = "BATCHURL";
        private const string AccountName = "ACCOUNTNAME";

        private const string AccountKey = "AccountKey";

        public async Task CreateJobAsync(StorageTask task)
        {
            var credentials = new BatchSharedKeyCredentials(BatchUrl, AccountName, AccountKey);

            using (BatchClient batchClient = await BatchClient.OpenAsync(credentials))
            {
                // a job is uniquely identified by its ID so your account name along with a timestamp is added as suffix
                var jobId = $"{task.Id}";
                var poolService = new PoolService();

                var existingPoolJob = await poolService.CreatePoolIfNotExistAsync(batchClient, "pool-job");
                    
                await SubmitJobAsync(batchClient, jobId, existingPoolJob.Id);
                    
                await CreateTasksAsync(batchClient, jobId, task);
            }
        }

        private async Task SubmitJobAsync(BatchClient batchClient, string jobId, string poolId)
        {
            //create an empty unbound Job
            CloudJob unboundJob = batchClient.JobOperations.CreateJob();
            unboundJob.Id = jobId;
            unboundJob.PoolInformation = new PoolInformation { PoolId = poolId };

            //Commit Job to create it in the service
            await unboundJob.CommitAsync();
        }

        private async Task CreateTasksAsync(BatchClient batchClient, string jobId, StorageTask task)
        {
            TaskContainerSettings cmdContainerSettings = new TaskContainerSettings(
                imageName: "ImageName",
                containerRunOptions: ""
            );

            // create a simple task. Each task within a job must have a unique ID
            for (var i = 0; i < task.Services.Count; i++)
            {
                var commandLine = $"cmd /c c:\\TestAppService.exe {task.Id} 1";

                CloudTask cloudTask =
                    new CloudTask(
                        $"{task.Services[i]}{i}", commandLine);

                cloudTask.ContainerSettings = cmdContainerSettings;

                //La tâche doit être éxécutée en administrateur
                cloudTask.UserIdentity = new UserIdentity(new AutoUserSpecification(elevationLevel: ElevationLevel.Admin, scope: AutoUserScope.Task));
                cloudTask.Constraints = new TaskConstraints
                {
                    MaxTaskRetryCount = 3
                };

                await batchClient.JobOperations.AddTaskAsync(jobId, cloudTask);
            }
        }
    }
}
