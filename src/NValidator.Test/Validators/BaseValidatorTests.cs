using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;
// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    public class BaseValidatorTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x).NotNull();

                RuleFor(x => x.Name).NotNull();
            }
        }

        [Test]
        public void Validator_should_work_if_try_to_validate_the_null_generic_type()
        {
            // Arrange
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(null).ToList();

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Order", results[0].MemberName);
            Assert.AreEqual("Order", results[0].PropertyName);
            Assert.AreEqual("Order must not be null.", results[0].Message);

            Assert.AreEqual("Order.Name", results[1].MemberName);
            Assert.AreEqual("Name", results[1].PropertyName);
            Assert.AreEqual("Name must not be null.", results[1].Message);
        }
    }
}
// ReSharper restore InconsistentNaming