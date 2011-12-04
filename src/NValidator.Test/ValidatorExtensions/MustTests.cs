using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.ValidatorExtensions
{
    [TestFixture]
    public class MustTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails[0].ProductCode).Must(x => x != "");
            }
        }

        class OrderValidator2 : TypeValidator<Order>
        {
            public OrderValidator2()
            {
                RuleFor(x => x.OrderDetails[0].ProductCode)
                    .Must((x, y) => x.Name != y)
                    .WithMessage("@PropertyName must be different to Name");
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_product_code_is_empty()
        {
            // Arrange
            var order = new Order
                            {
                                OrderDetails = new [] {
                                    new OrderDetail{ProductCode = ""}
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
        public void GetValidationResult_return_empty_result_if_the_property_is_not_empty()
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
            Assert.AreEqual("ProductCode must be different to Name", firstResult.Message);
        }
    }
}
// ReSharper restore InconsistentNaming