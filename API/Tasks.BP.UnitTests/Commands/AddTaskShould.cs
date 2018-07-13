using System;
using System.Threading.Tasks;
using Command;
using Command.Core;
using Moq;
using Tasks.BO;
using Tasks.BO.Input;
using Tasks.BP.Commands;
using Tasks.BP.Validators;
using Tasks.StorageQueueAccess;
using Xunit;

namespace Tasks.BP.UnitTests.Commands
{
    public class AddTaskShould
    {
        private readonly Mock<IStorageQueueService> _storageQueueService;
        private readonly AddTask _addTask;

        public AddTaskShould()
        {
            _storageQueueService = new Mock<IStorageQueueService>();
            var addTaskValidator = new Mock<IStorageTaskValidator>();

            addTaskValidator.Setup(val => val.Validate(It.IsAny<StorageTask>())).Returns(true);
            _addTask = new AddTask(_storageQueueService.Object, addTaskValidator.Object);
        }

        [Fact]
        public async Task ReturnSuccess_WhenAddTaskIsOk()
        {
            //Arrange
            var storageTask = new StorageTask
            {
                Title = "Mon titre"
            };

            _storageQueueService.Setup(sto => sto.AddMessageAsync(storageTask)).Returns(Task.CompletedTask);

            //Act
            var result = await _addTask.ExecuteAsync(storageTask);

            //Assert
            _storageQueueService.Verify(sto => sto.AddMessageAsync(storageTask), Times.Once);
            Assert.Equal(Status.Success, result.Status);
        }

        [Fact]
        public async Task ReturnFail_WhenAddTaskIsNotOk()
        {
            //Arrange
            var storageTask = new StorageTask
            {
                Title = "Mon titre"
            };
            _storageQueueService.Setup(sto => sto.AddMessageAsync(storageTask)).ThrowsAsync(new ArgumentException());

            //Act
            var result = await _addTask.ExecuteAsync(storageTask);

            //Assert
            _storageQueueService.Verify(sto => sto.AddMessageAsync(storageTask), Times.Once);
            Assert.Equal(Status.Fail, result.Status);
        }
    }
}
