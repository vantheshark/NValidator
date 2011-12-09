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

        class OrderValidator2 : TypeValidator<Order>
        {
            public OrderValidator2()
            {
                RuleFor(x => x.ID).Not().In("1", "2", "3").WithMessage("@PropertyName must not be one of the following values: (@ProhibitValues) !!!");
            }
        };

        [Test]
        public void Can_override_the_default_message_with_parameters()
        {
            // Arrange
            var order = new Order
            {
                ID = "2",
            };
            var validator = new OrderValidator2();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.ID", results[0].MemberName);
            Assert.AreEqual("ID must not be one of the following values: (1, 2, 3) !!!", results[0].Message);
        }
    }
}
// ReSharper restore InconsistentNaming