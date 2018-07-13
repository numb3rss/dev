using Command;
using Moq;
using Tasks.AzureBatchAccess;
using Tasks.BO;
using Tasks.BO.Input;
using Tasks.BP.Commands;
using Tasks.BP.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tasks.BP.UnitTests.Commands
{
    public class HandleAzureBatchShould
    {
        private readonly Mock<IAzureBatchService> _azureBatchService;
        private readonly HandleAzureBatch _handleAzureBatch;

        public HandleAzureBatchShould()
        {
            _azureBatchService = new Mock<IAzureBatchService>();
            var storageTaskValidator = new Mock<IStorageTaskValidator>();

            storageTaskValidator.Setup(val => val.Validate(It.IsAny<StorageTask>())).Returns(true);
            _handleAzureBatch = new HandleAzureBatch(_azureBatchService.Object, storageTaskValidator.Object);
        }

        [Fact]
        public async Task ReturnSuccess_WhenHandleAzureBatchIsOk()
        {
            //Arrange
            var storageTask = new StorageTask
            {
                Title = "Mon titre"
            };

            _azureBatchService.Setup(sto => sto.CreateJobAsync(It.IsAny<StorageTask>())).Returns(Task.CompletedTask);

            //Act
            var result = await _handleAzureBatch.ExecuteAsync(storageTask);

            //Assert
            _azureBatchService.Verify(sto => sto.CreateJobAsync(It.IsAny<StorageTask>()), Times.Once);
            Assert.Equal(Status.Success, result.Status);
        }

        [Fact]
        public async Task ReturnFail_WhenHandleAzureBatchIsNotOk()
        {
            //Arrange
            var storageTask = new StorageTask
            {
                Title = "Mon titre"
            };
            _azureBatchService.Setup(sto => sto.CreateJobAsync(It.IsAny<StorageTask>())).ThrowsAsync(new ArgumentException());

            //Act
            var result = await _handleAzureBatch.ExecuteAsync(storageTask);

            //Assert
            _azureBatchService.Verify(sto => sto.CreateJobAsync(It.IsAny<StorageTask>()), Times.Once);
            Assert.Equal(Status.Fail, result.Status);
        }
    }
}
