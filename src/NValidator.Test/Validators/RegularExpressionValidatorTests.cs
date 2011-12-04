using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class RegularExpressionValidatorTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.ID).Match(@"^[a-zA-Z0-9]*$");
                RuleFor(x => x.Name).Match(@"^[0-9a-zA-Z\-]*$", false);
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_name_is_null()
        {
            // Arrange
            var order = new Order
                            {
                                ID = null,
                                Name = null
                            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order);

            // Assert
            Assert.AreEqual(1, results.Count());
            var firstResult = results.FirstOrDefault();
            Assert.AreEqual("Order.Name", firstResult.MemberName);
            Assert.AreEqual("Name is not in the correct format.", firstResult.Message);
        }

        [Test]
        public void GetValidationResult_return_empty_result_if_the_id_and_name_are_valid()
        {
            // Arrange
            var order = new Order
            {
                ID = "A12",
                Name = "A-123"
            };
            var validator = new OrderValidator();

            // Action
            var result = validator.GetValidationResult(order);

            // Assert

            Assert.AreEqual(0, result.Count());
        }
    }
}
// ReSharper restore InconsistentNaming