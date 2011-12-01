using System.Linq;
using NUnit.Framework;
// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class PredicateValidatorTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails[0].ProductCode).Must(x => x != null && x.Length > 10);
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_must_condition_not_match()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new[] {
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
            Assert.AreEqual("ProductCode does not match condition.", firstResult.Message);
        }

        [Test]
        public void GetValidationResult_return_empty_result_if_the_must_condition_matches()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new[] {
                                    new OrderDetail{ProductCode = "Product12345"}
                                }
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