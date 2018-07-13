using System.Collections.Generic;
using Tasks.BO;
using Tasks.BO.Input;
using Tasks.BP.Validators;
using Xunit;

namespace Tasks.BP.UnitTests.Validators
{
    public class AddTaskValidatorShould
    {
        private readonly StorageTaskValidator _addTaskValidator;

        public AddTaskValidatorShould()
        {
            _addTaskValidator = new StorageTaskValidator();
        }

        [Fact]
        public void IsTruthy_WhenTitleTaskIsFilled()
        {
            //Arrange
            var storageTask = new StorageTask
            {
                Title = "Mon titre",
                Services = new List<Service>
                {
                    Service.Adr 
                }
            };

            //Act
            var isValid = _addTaskValidator.Validate(storageTask);

            //Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsTruthy_WhenServiceIsDematerialization()
        {
            //Arrange
            var storageTask = new StorageTask
            {
                Title =  "Mon titre",
                Services = new List<Service>{ Service.Dematerialization }
            };

            //Act
            var isValid = _addTaskValidator.Validate(storageTask);

            //Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsTruthy_WhenServiceIsAdr()
        {
            //Arrange
            var storageTask = new StorageTask
            {
                Title = "Mon titre",
                Services = new List<Service> { Service.Adr }
            };

            //Act
            var isValid = _addTaskValidator.Validate(storageTask);

            //Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsTruthy_WhenServiceIsSigning()
        {
            //Arrange
            var storageTask = new StorageTask
            {
                Title = "Mon titre",
                Services = new List<Service> { Service.Signing }
            };

            //Act
            var isValid = _addTaskValidator.Validate(storageTask);

            //Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsFalsy_WhenTitleTaskIsNull()
        {
            //Arrange
            var storageTask = new StorageTask();

            //Act
            var isValid = _addTaskValidator.Validate(storageTask);

            //Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsFalsy_WhenServicesAreNull()
        {
            //Arrange
            var storageTask = new StorageTask
            {
                Title = "Mon titre",
                Services = null
            };

            //Act
            var isValid = _addTaskValidator.Validate(storageTask);

            //Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsFalsy_WhenServicesAreNotFilled()
        {
            //Arrange
            var storageTask = new StorageTask
            {
                Title = "Mon titre",
                Services = new List<Service>()
            };

            //Act
            var isValid = _addTaskValidator.Validate(storageTask);

            //Assert
            Assert.False(isValid);
        }
    }
}
