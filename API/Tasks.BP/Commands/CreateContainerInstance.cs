using Command;
using Command.Core;
using Tasks.ContainerInstanceAccess;
using Tasks.BO;
using Tasks.BP.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.BP.Commands
{
    public interface ICreateContainerInstance : ICommandIn<StorageTask>
    {

    }
    public class CreateContainerInstance : CommandIn<StorageTask>, ICreateContainerInstance
    {
        private readonly IContainerInstanceProvider _containerInstanceProvider;

        public CreateContainerInstance(
            IContainerInstanceProvider containerInstanceProvider, 
            IStorageTaskValidator storageTaskValidator)
            : base(storageTaskValidator)
        {
            _containerInstanceProvider = containerInstanceProvider;
        }

        protected override IResult OnExecute(StorageTask input)
        {
            try
            {
                _containerInstanceProvider.CreateContainerImage(input.Id.ToString());
                return new Result { Status = Status.Success };
            }
            catch (Exception ex)
            {
                return new Result { Status = Status.Fail };
            }
        }
    }
}
