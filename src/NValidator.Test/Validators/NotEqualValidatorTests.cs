using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class NotEqualValidatorTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails[0].ProductCode)
                    .NotEqual("123")
                    .WithMessage("@PropertyName can not equal to 123.");
            }
        }

        class OrderValidator2 : TypeValidator<Order>
        {
            public OrderValidator2()
            {
                RuleFor(x => x.OrderDetails[0].ProductCode)
                    .NotEqual(x => x.Name)
                    .WithMessage("@PropertyName can not equal to order Name");
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_product_code_equals_to_123()
        {
            // Arrange
            var order = new Order
                            {
                                OrderDetails = new [] {
                                    new OrderDetail{ProductCode = "123"}
                                }
                            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order);

            // Assert
            Assert.AreEqual(1, results.Count());
            var firstResult = results.FirstOrDefault();
            Assert.AreEqual("Order.OrderDetails[0].ProductCode", firstResult.MemberName);
            Assert.AreEqual("ProductCode can not equal to 123.", firstResult.Message);
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
        public void GetValidationResult_return_one_error_result_if_the_product_code_equals_to_order_name()
        {
            // Arrange
            var order = new Order
            {
                Name = "P1",
                OrderDetails = new[] {
                                    new OrderDetail{ProductCode = "P1"}
                                }
            };
            var validator = new OrderValidator2();

            // Action
            var results = validator.GetValidationResult(order);

            // Assert
            Assert.AreEqual(1, results.Count());
            var firstResult = results.FirstOrDefault();
            Assert.AreEqual("Order.OrderDetails[0].ProductCode", firstResult.MemberName);
            Assert.AreEqual("ProductCode can not equal to order Name", firstResult.Message);
        }
    }
}
// ReSharper restore InconsistentNaming