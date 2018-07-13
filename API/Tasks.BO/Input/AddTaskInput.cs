using System.Collections.Generic;

namespace Tasks.BO.Input
{
    public class AddTaskInput
    {
        public string Title { get; set; }
        public List<Service> Services { get; set; }
        public bool IsSuccess { get; set; }
    }
}
