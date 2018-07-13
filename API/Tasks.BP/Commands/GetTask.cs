using System;
using System.Threading.Tasks;
using Command;
using Command.Core;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using Tasks.BO;
using Tasks.BP.Validators;
using Tasks.StorageQueueAccess;

namespace Tasks.BP.Commands
{
    public interface IGetTask : ICommandOutAsync<TaskModel>
    {
    }

    public class GetTask : CommandOutAsync<TaskModel>, IGetTask
    {
        private readonly IStorageQueueService _storageQueueService;

        public GetTask(IStorageQueueService storageQueueService, IGetTaskValidator getTaskValidator)
            : base(getTaskValidator)
        {
            _storageQueueService = storageQueueService;
        }

        protected override async Task<Result<TaskModel>> OnExecuteAsync()
        {
            try
            {
                var storageTask = await _storageQueueService.GetMessageAsync();

                return new Result<TaskModel>
                {
                    Status = Status.Success,
                    Value = new TaskModel
                    {
                        Title = storageTask.Title,
                        Id = storageTask.Id
                    }
                };
            }
            catch (Exception)
            {
                return new Result<TaskModel>
                {
                    Status = Status.Fail
                };
            }
        }
    }
}
