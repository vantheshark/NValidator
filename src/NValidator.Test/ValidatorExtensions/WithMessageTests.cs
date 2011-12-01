using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.ValidatorExtensions
{
    [TestFixture]
    public class WithMessageTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails[0].ProductCode).NotNull().WithMessage("The '@PropertyName' must not be null.");
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
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.OrderDetails[0].ProductCode", results[0].MemberName);
            Assert.AreEqual("The 'ProductCode' must not be null.", results[0].Message);
        }
    }
}
// ReSharper restore InconsistentNaming