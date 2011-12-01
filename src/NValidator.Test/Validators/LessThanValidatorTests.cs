using System.Linq;
using NUnit.Framework;
// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class LessThanValidatorTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails[0].Price).LessThan(100);
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_product_price_is_greater_than_or_equal_100()
        {
            // Arrange
            var order = new Order
                            {
                                OrderDetails = new [] {
                                    new OrderDetail{Price = 100}
                                }
                            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.OrderDetails[0].Price", results[0].MemberName);
            Assert.AreEqual("Price must be less than 100.", results[0].Message);
        }

        [Test]
        public void GetValidationResult_return_empty_result_if_the_product_price_is_less_than_100()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new[] {
                                   new OrderDetail { Price = 99 }
                                }
            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(0, results.Count());
        }
    }
}
// ReSharper restore InconsistentNaming