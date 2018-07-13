using Tasks.BO.Input;

namespace Tasks.BP.Commands
{
    using System;
    using Validators;
    using System.Threading.Tasks;
    using Command;
    using Command.Core;
    using StorageQueueAccess;
    using Tasks.BO;

    public interface IAddTask : ICommandInAsync<StorageTask>
    {

    }

    public class AddTask : CommandInAsync<StorageTask>, IAddTask
    {
        private readonly IStorageQueueService _storageQueueService;

        public AddTask(IStorageQueueService storageQueueService, IStorageTaskValidator storageTaskValidator) 
            : base(storageTaskValidator)
        {
            _storageQueueService = storageQueueService;
        }

        protected override async Task<Result> OnExecuteAsync(StorageTask input)
        {
            try
            {
                await _storageQueueService.AddMessageAsync(input);
                return new Result { Status = Status.Success };
            }
            catch (Exception)
            {
                return new Result { Status = Status.Fail };
            }
        }
    }
}
