using Command;
using Command.Core;
using Tasks.AzureBatchAccess;
using Tasks.BO;
using Tasks.BO.Input;
using Tasks.BP.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.BP.Commands
{
    public interface IHandleAzureBatch : ICommandInAsync<StorageTask>
    {

    }

    public class HandleAzureBatch : CommandInAsync<StorageTask>, IHandleAzureBatch
    {
        private IAzureBatchService _azureBatchService;

        public HandleAzureBatch(IAzureBatchService azureBatchService, IStorageTaskValidator storageTaskValidator) : base(storageTaskValidator)
        {
            _azureBatchService = azureBatchService;
        }

        protected override async Task<Result> OnExecuteAsync(StorageTask input)
        {
            try
            {
                await _azureBatchService.CreateJobAsync(input);
                return new Result { Status = Status.Success };
            }
            catch (Exception ex)
            {
                return new Result { Status = Status.Fail };
            }
        }
    }
}
