using System;
using System.Threading.Tasks;
using Command;
using Microsoft.AspNetCore.Mvc;
using Tasks.ContainerInstanceAccess;
using Tasks.BO;
using Tasks.BO.Input;
using Tasks.BP.Commands;

namespace Tasks.API.Controllers
{
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly IGetTask _getTask;
        private readonly IAddTask _addTask;
        private readonly IHandleAzureBatch _handleAzureBatch;
        private readonly ICreateContainerInstance _createContainerInstance;

        public TasksController(IGetTask getTask, IAddTask addTask, IHandleAzureBatch handleAzureBatch, ICreateContainerInstance createContainerInstance)
        {
            _getTask = getTask;
            _addTask = addTask;
            _handleAzureBatch = handleAzureBatch;
            _createContainerInstance = createContainerInstance;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> Get()
        {
            var result = await _getTask.ExecuteAsync();

            if (result.Status == Status.Success)
            {
                return Ok(result);
            }

            return BadRequest("Tasks can't be retrieved");
        }

        // POST api/values
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Post([FromBody]AddTaskInput addTaskInput)
        {
            var task = new StorageTask
            {
                Id = Guid.NewGuid(),
                Title = addTaskInput.Title,
                IsSuccess = addTaskInput.IsSuccess,
                Services = addTaskInput.Services
            };

            //var resultHandle = await _handleAzureBatch.ExecuteAsync(task);

            var result = await _addTask.ExecuteAsync(task);

            var resultCreateInstance = _createContainerInstance.Execute(task);

            if (result.Status == Status.Success && resultCreateInstance.Status == Status.Success)
            {
                return Ok("Task has been successfully added");
            }

            return BadRequest("Task cannot be added");
        }
    }
}
