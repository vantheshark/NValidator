using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.ValidatorExtensions
{
    [TestFixture]
    public class WhenTests
    {
        class UserValidator : CompositeValidator<User>
        {
        }

        class AddressValidator : TypeValidator<Address>
        {
            public AddressValidator()
            {
                RuleFor(x => x.Number).StopOnFirstError().NotNull().NotEmpty().When(x => x != null);
                RuleFor(x => x.Street).Length(1, 100).When(x => x != null).WithMessage("Not a valid street line.");
                RuleFor(x => x.Suburb).NotEmpty().When(x => x != null);
                RuleFor(x => x.PostCode).Length(4, 5).When(x => x != null).WithMessage("Not a valid postcode.");
            }
        }

        [Test]
        public void ConditionalValidator_should_return_validation_result_when_the_nested_validator_has_when_condition()
        {
            // Arrange
            var factory = new Mock<IValidatorFactory>();
            factory.Setup(x => x.GetValidatorFor(It.Is<Type>(t => t == typeof(Address))))
                   .Returns(new AddressValidator());
            factory.Setup(x => x.IsDefaultValidator(It.Is<Type>(t => t == typeof(AddressValidator)))).Returns(true);
            ValidatorFactory.Current = factory.Object;

            var user = new User
                            {
                                Address = new Address()
                            };
            var validator = new UserValidator();

            // Action
            var results = validator.GetValidationResult(user).ToList();

            // Assert
            Assert.AreEqual(4, results.Count());
            Assert.AreEqual("User.Address.Number", results[0].MemberName);
            Assert.AreEqual("Number must not be null.", results[0].Message);

            Assert.AreEqual("User.Address.Street", results[1].MemberName);
            Assert.AreEqual("Not a valid street line.", results[1].Message);

            Assert.AreEqual("User.Address.Suburb", results[2].MemberName);
            Assert.AreEqual("Suburb cannot be empty.", results[2].Message);

            Assert.AreEqual("User.Address.PostCode", results[3].MemberName);
            Assert.AreEqual("Not a valid postcode.", results[3].Message);
        }
    }
}
// ReSharper restore InconsistentNaming