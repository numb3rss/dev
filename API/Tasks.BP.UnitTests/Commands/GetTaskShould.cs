using System;
using System.Threading.Tasks;
using Command;
using Moq;
using Tasks.BO;
using Tasks.BP.Commands;
using Tasks.BP.Validators;
using Tasks.StorageQueueAccess;
using Xunit;

namespace Tasks.BP.UnitTests.Commands
{
    public class GetTaskShould
    {
        public GetTaskShould()
        {
            _storageQueueService = new Mock<IStorageQueueService>();
            var getTaskValidator = new Mock<IGetTaskValidator>();

            getTaskValidator.Setup(val => val.Validate(It.IsAny<TaskModel>())).Returns(true);
            _getTask = new GetTask(_storageQueueService.Object, getTaskValidator.Object);
        }

        private readonly Mock<IStorageQueueService> _storageQueueService;
        private readonly GetTask _getTask;

        [Fact]
        public async Task ReturnFail_WhenGetMessageAsyncIsNotOk()
        {
            //Arrange
            _storageQueueService.Setup(sto => sto.GetMessageAsync()).ThrowsAsync(new ArgumentException());

            //Act
            var result = await _getTask.ExecuteAsync();

            //Assert
            _storageQueueService.Verify(sto => sto.GetMessageAsync(), Times.Once);
            Assert.Equal(Status.Fail, result.Status);
        }

        [Fact]
        public async Task ReturnSuccess_WhenGetMessageAsyncIsOk()
        {
            //Arrange
            _storageQueueService.Setup(sto => sto.GetMessageAsync()).ReturnsAsync(new StorageTask());

            //Act
            var result = await _getTask.ExecuteAsync();

            //Assert
            _storageQueueService.Verify(sto => sto.GetMessageAsync(), Times.Once);
            Assert.Equal(Status.Success, result.Status);
        }
    }
}