using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

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
                RuleFor(x => x.ID).Equal("123").WithMessage(x => string.Format("The '@PropertyName' must equal to 123, you entered {0}.", x.ID));
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_property_is_null()
        {
            // Arrange
            var order = new Order
                            {
                                ID = "234",
                                OrderDetails = new [] {
                                    new OrderDetail()
                                }
                            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Order.OrderDetails[0].ProductCode", results[0].MemberName);
            Assert.AreEqual("The 'ProductCode' must not be null.", results[0].Message);

            Assert.AreEqual("Order.ID", results[1].MemberName);
            Assert.AreEqual("The 'ID' must equal to 123, you entered 234.", results[1].Message);
        }
    }
}
// ReSharper restore InconsistentNaming