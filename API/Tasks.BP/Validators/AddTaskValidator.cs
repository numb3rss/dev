using System.Linq;
using Command.Core;
using Tasks.BO;
using Tasks.BO.Input;

namespace Tasks.BP.Validators
{
    public interface IStorageTaskValidator : IValidator<StorageTask>
    {

    }

    public class StorageTaskValidator : IStorageTaskValidator
    {
        public bool Validate(StorageTask storageTask)
        {
            if (string.IsNullOrEmpty(storageTask.Title))
            {
                return false;
            }

            if (storageTask.Services == null || !storageTask.Services.Any())
            {
                return false;
            }

            return storageTask.Services.All(s => s == Service.Dematerialization ||
                                           s == Service.Adr ||
                                           s == Service.Signing);
        }
    }
}
