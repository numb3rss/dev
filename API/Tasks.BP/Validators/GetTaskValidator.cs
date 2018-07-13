using Command.Core;
using Tasks.BO;

namespace Tasks.BP.Validators
{
    public interface IGetTaskValidator : IValidator<TaskModel>
    {

    }
    public class GetTaskValidator : IGetTaskValidator
    {
        public bool Validate(TaskModel value)
        {
            return true;
        }
    }
}
