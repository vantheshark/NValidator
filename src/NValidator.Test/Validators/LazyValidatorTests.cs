using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using NValidator.Validators;
// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class LazyValidatorTests
    {
        private readonly Mock<IHaveContainer> haveContainer = new Mock<IHaveContainer>();

        [Test]
        public void GetValidationResult_must_resolve_validator_then_update_containerName_before_validation()
        {
            // Arrange
            haveContainer.Setup(x => x.UpdateContainerName(It.IsAny<string>())).Verifiable();
            var mockValidator = haveContainer.As<IValidator<string>>();
            mockValidator.Setup(x => x.GetValidationResult("ABC", It.IsAny<ValidationContext>()))
                         .Returns(new List<ValidationResult>())
                         .Verifiable();

            var factory = new Mock<IValidatorFactory>();
            factory.Setup(x => x.Resolve<IValidator<string>>())
                   .Returns(mockValidator.Object);
            ValidatorFactory.Current = factory.Object;

            // Action
            var lazy = new LazyValidator<IValidator<string>, string>();
            var results = lazy.GetValidationResult("ABC").ToList();

            // Assert
            haveContainer.Verify(x => x.UpdateContainerName(It.IsAny<string>()), Times.Once());
            mockValidator.Verify(x => x.GetValidationResult("ABC", It.IsAny<ValidationContext>()), Times.Once());
            Assert.AreEqual(0, results.Count());
        }
    }
}
// ReSharper restore InconsistentNaming