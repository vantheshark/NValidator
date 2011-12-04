using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class ConditionalValidatorTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails[0].ProductCode).Not().Null().When(x => x.OrderDetails != null);
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_property_is_null()
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
            Assert.AreEqual(1, results.Count());
            var firstResult = results.FirstOrDefault();
            Assert.AreEqual("Order.OrderDetails[0].ProductCode", firstResult.MemberName);
            Assert.AreEqual("ProductCode must not be null.", firstResult.Message);
        }

        [Test]
        public void GetValidationResult_return_empty_result_if_the_property_is_not_null()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new[] {
                                    new OrderDetail{ProductCode = "Product1"}
                                }
            };
            var validator = new OrderValidator();

            // Action
            var result = validator.GetValidationResult(order);

            // Assert

            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetValidationResult_return_empty_result_if_the_condition_does_not_match()
        {
            // Arrange
            var order = new Order();
            var validator = new OrderValidator();

            // Action
            var result = validator.GetValidationResult(order);

            // Assert

            Assert.AreEqual(0, result.Count());
        }
    }
}
// ReSharper restore InconsistentNaming