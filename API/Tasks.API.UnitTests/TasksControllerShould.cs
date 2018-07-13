using Tasks.BO.Input;

namespace Tasks.API.UnitTests
{
    using System.Threading.Tasks;
    using Command;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Controllers;
    using BO;
    using BP.Commands;
    using Xunit;

    public class TasksControllerShould
    {
        private readonly Mock<ICreateContainerInstance> _createContainerInstance;
        private readonly Mock<IGetTask> _getTaskMock;
        private readonly Mock<IAddTask> _addTaskMock;
        private readonly Mock<IHandleAzureBatch> _handleAzureBatch;
        private readonly TasksController _tasksController;

        public TasksControllerShould()
        {
            _getTaskMock = new Mock<IGetTask>();
            _addTaskMock = new Mock<IAddTask>();
            _handleAzureBatch = new Mock<IHandleAzureBatch>();
            _createContainerInstance = new Mock<ICreateContainerInstance>();

            _tasksController = new TasksController(_getTaskMock.Object, _addTaskMock.Object, _handleAzureBatch.Object, _createContainerInstance.Object);
        }

        [Fact]
        public async Task GetAll_ReturnOk_WhenTaskCanBeRetrieved()
        {
            //Arrange
            _getTaskMock.Setup(mes => mes.ExecuteAsync()).ReturnsAsync(new Result<TaskModel>
            {
                Status = Status.Success,
                Value = new TaskModel()
            });

            //Act
            var actionResult = await _tasksController.Get() as OkObjectResult;

            //Assert
            _getTaskMock.Verify(mes => mes.ExecuteAsync(), Times.Once);
            Assert.NotNull(actionResult);
            Assert.IsType<Result<TaskModel>>(actionResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnBadRequest_WhenTaskCannotBeRetrieved()
        {
            //Arrange
            _getTaskMock.Setup(mes => mes.ExecuteAsync()).ReturnsAsync(new Result<TaskModel>
            {
                Status = Status.Fail
            });

            //Act
            var actionResult = await _tasksController.Get() as BadRequestObjectResult;

            //Assert
            _getTaskMock.Verify(mes => mes.ExecuteAsync(), Times.Once);
            Assert.NotNull(actionResult);
            Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Tasks can't be retrieved", actionResult.Value);
        }

        [Fact]
        public async Task Add_ReturnOk_WhenTaskHasBeenSuccessfullyAdded()
        {
            //Arrange
            var addTaskInput = new AddTaskInput {Title = "Ma tâche"};
            _addTaskMock.Setup(mes => mes.ExecuteAsync(It.IsAny<StorageTask>())).ReturnsAsync(new Result
            {
                Status = Status.Success
            });

            _handleAzureBatch.Setup(mes => mes.ExecuteAsync(It.IsAny<StorageTask>())).ReturnsAsync(new Result
            {
                Status = Status.Success
            });

            //Act
            var actionResult = await _tasksController.Post(addTaskInput) as OkObjectResult;

            //Assert
            _addTaskMock.Verify(mes => mes.ExecuteAsync(It.IsAny<StorageTask>()), Times.Once);
            _handleAzureBatch.Verify(mes => mes.ExecuteAsync(It.IsAny<StorageTask>()), Times.Once);
            Assert.NotNull(actionResult);
            Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Task has been successfully added", actionResult.Value);
        }

        [Fact]
        public async Task Add_ReturnBadRequest_WhenTaskHasNotBeenSuccessfully()
        {
            //Arrange
            var addTaskInput = new AddTaskInput {Title = "Ma tâche"};

            _handleAzureBatch.Setup(mes => mes.ExecuteAsync(It.IsAny<StorageTask>())).ReturnsAsync(new Result
            {
                Status = Status.Success
            });

            _addTaskMock.Setup(mes => mes.ExecuteAsync(It.IsAny<StorageTask>())).ReturnsAsync(new Result
            {
                Status = Status.Fail
            });

            //Act
            var actionResult = await _tasksController.Post(addTaskInput) as BadRequestObjectResult;

            //Assert
            _handleAzureBatch.Verify(mes => mes.ExecuteAsync(It.IsAny<StorageTask>()), Times.Once);
            _addTaskMock.Verify(mes => mes.ExecuteAsync(It.IsAny<StorageTask>()), Times.Once);
            Assert.NotNull(actionResult);
            Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Task cannot be added", actionResult.Value);
        }

        [Fact]
        public async Task Add_ReturnBadRequest_WhenTaskHasNotBeenSuccessfullyBeacauseHandleAzureBatchFailed()
        {
            //Arrange
            var addTaskInput = new AddTaskInput { Title = "Ma tâche" };

            _handleAzureBatch.Setup(mes => mes.ExecuteAsync(It.IsAny<StorageTask>())).ReturnsAsync(new Result
            {
                Status = Status.Fail
            });

            _addTaskMock.Setup(mes => mes.ExecuteAsync(It.IsAny<StorageTask>())).ReturnsAsync(new Result
            {
                Status = Status.Success
            });

            //Act
            var actionResult = await _tasksController.Post(addTaskInput) as BadRequestObjectResult;

            //Assert
            _handleAzureBatch.Verify(mes => mes.ExecuteAsync(It.IsAny<StorageTask>()), Times.Once);
            _addTaskMock.Verify(mes => mes.ExecuteAsync(It.IsAny<StorageTask>()), Times.Once);
            Assert.NotNull(actionResult);
            Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Task cannot be added", actionResult.Value);
        }
    }
}