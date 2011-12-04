using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test
{
    [TestFixture]
    public class ChainingValidatorsTests
    {
        class OrderValidator : CompositeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails)
                    .Not().Null()
                    .NotEmpty()

                    .Must(x => x != null && x.Count() > 1).WithMessage("Must have at least 2 order details.")

                    .Must(x => x[0].Price * x[0].Amount > 1000)
                    .When(x => x != null && x.OrderDetails != null && x.OrderDetails.Length > 0)
                    .WithMessage("First order detail must greater than $1000.")
                    
                    .Must(x => x[1].Price * x[1].Amount > 500)
                    .When(x => x != null && x.OrderDetails != null && x.OrderDetails.Length > 1)
                    .WithMessage("Second order detail must greater than $500.");
            }
        }

        [Test]
        public void GetValidationResult_returns_errors_if_item_is_null()
        {
            // Arrange
            var order = new Order();
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(3, results.Count());
            Assert.AreEqual("Order.OrderDetails", results[0].MemberName);
            Assert.AreEqual("OrderDetails must not be null.", results[0].Message);
            Assert.AreEqual("Order.OrderDetails", results[1].MemberName);
            Assert.AreEqual("OrderDetails cannot be empty.", results[1].Message);
            Assert.AreEqual("Order.OrderDetails", results[2].MemberName);
            Assert.AreEqual("Must have at least 2 order details.", results[2].Message);
        }

        [Test]
        public void GetValidationResult_returns_errors_if_item_is_empty()
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
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Order.OrderDetails", results[0].MemberName);
            Assert.AreEqual("OrderDetails cannot be empty.", results[0].Message);
            Assert.AreEqual("Order.OrderDetails", results[1].MemberName);
            Assert.AreEqual("Must have at least 2 order details.", results[1].Message);
        }

        [Test]
        public void GetValidationResult_returns_errors_if_has_less_than_2_order_details()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new [] { new OrderDetail() }
            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Order.OrderDetails", results[0].MemberName);
            Assert.AreEqual("Must have at least 2 order details.", results[0].Message);
            Assert.AreEqual("Order.OrderDetails", results[1].MemberName);
            Assert.AreEqual("First order detail must greater than $1000.", results[1].Message);
        }

        [Test]
        public void GetValidationResult_returns_errors_if_second_order_detail_less_than_500()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new[] { 
                    new OrderDetail{Amount = 100, Price = 10.01},
                    new OrderDetail()
                }
            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.OrderDetails", results[0].MemberName);
            Assert.AreEqual("Second order detail must greater than $500.", results[0].Message);
        }

        [Test]
        public void GetValidationResult_returns_no_errors_if_all_conditions_match()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new[] { 
                    new OrderDetail{Amount = 10, Price = 100.01},
                    new OrderDetail{Amount = 5, Price = 100.01},
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