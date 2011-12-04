using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class LengthValidatorTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails)
                    .Length(1, 2)
                    .WithMessage("An order must have @MinimumLength or @MaximumLength order details. You have @TotalLength order details.");
            }
        }

        class OrderValidator2 : TypeValidator<Order>
        {
            public OrderValidator2()
            {
                RuleFor(x => x.ID)
                    .Length(1, 5)
                    .WithMessage("@PropertyName's length must be from @MinimumLength to @MaximumLength. You entered @TotalLength characters.");
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_there_are_not_any_order_details()
        {
            // Arrange
            var order = new Order
                            {
                                OrderDetails = new OrderDetail[] {}
                            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order);

            // Assert
            Assert.AreEqual(1, results.Count());
            var firstResult = results.FirstOrDefault();
            Assert.AreEqual("Order.OrderDetails", firstResult.MemberName);
            Assert.AreEqual("An order must have 1 or 2 order details. You have 0 order details.", firstResult.Message);
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_there_are_more_than_2_order_details()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new [] { new OrderDetail(), new OrderDetail(), new OrderDetail()}
            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order);

            // Assert
            Assert.AreEqual(1, results.Count());
            var firstResult = results.FirstOrDefault();
            Assert.AreEqual("Order.OrderDetails", firstResult.MemberName);
            Assert.AreEqual("An order must have 1 or 2 order details. You have 3 order details.", firstResult.Message);
        }

        [Test]
        public void GetValidationResult_return_1_result_if_the_Order_ID_is_too_long()
        {
            // Arrange
            var order = new Order
            {
                ID = "123456"
            };
            var validator = new OrderValidator2();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.ID", results[0].MemberName);
            Assert.AreEqual("ID's length must be from 1 to 5. You entered 6 characters.", results[0].Message);
        }

        [Test]
        public void GetValidationResult_return_empty_result_if_the_ID_is_valid()
        {
            // Arrange
            var order = new Order
            {
                ID = "P1",
            };
            var validator = new OrderValidator2();

            // Action
            var results = validator.GetValidationResult(order);

            // Assert
            Assert.AreEqual(0, results.Count());
        }
    }
}
// ReSharper restore InconsistentNaming