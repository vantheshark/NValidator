using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class NotEmptyValidatorTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails).NotEmpty();
            }
        }

        [Test]
        public void GetValidationResult_return_no_error_result_if_the_OrderDetails_has_at_least_one_item()
        {
            // Arrange
            var order = new Order
                            {
                                OrderDetails = new [] {
                                    new OrderDetail()
                                }
                            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order);

            // Assert
            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public void GetValidationResult_return_empty_result_if_the_OrderDetails_is_empty()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new OrderDetail[] {}
            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.OrderDetails", results[0].MemberName);
            Assert.AreEqual("OrderDetails cannot be empty.", results[0].Message);
        }
    }
}
// ReSharper restore InconsistentNaming