using System;
using System.Linq;
using Moq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.ValidatorExtensions
{
    [TestFixture]
    public class NotTests : ResetDefaultValidatorFactoryTests
    {
        class OrderValidator : CompositeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails).Not().Null().WithMessage("Details cannot be null!");
            }
        }

        class OrderValidator2 : CompositeValidator<Order>
        {
            public OrderValidator2()
            {
                RuleFor(x => x.ID).Not().Null().WithMessage("ID cannot be null!")
                                  .Not().In("11", "22", "33");
            }
        }

        class OrderDetailValidator : TypeValidator<OrderDetail>
        {
            public OrderDetailValidator()
            {
                RuleFor(x => x.Price).Must(x => x > 0);
                RuleFor(x => x.Amount).Not().LessThanOrEqual(0);
                RuleFor(x => x.ProductCode).Must(x => x != null && x.Length > 3).WithMessage("@PropertyName must be longer than 3.");
            }
        }

        [Test]
        public void GetValidationResult_returns_errors_for_items_in_IEnumerable_property()
        {
            // Arrange
            var factory = new Mock<IValidatorFactory>();
            factory.Setup(x => x.GetValidatorFor(It.Is<Type>(t => t == typeof(OrderDetail))))
                   .Returns(new OrderDetailValidator());
            factory.Setup(x => x.IsDefaultValidator(It.Is<Type>(t => t == typeof(OrderDetailValidator)))).Returns(true);
            ValidatorFactory.Current = factory.Object;

            var order = new Order
                            {
                                ID = "ORDER ID",
                                OrderDetails = new [] {
                                    new OrderDetail{Amount = 1000, ProductCode = "1234"},
                                    new OrderDetail{Price = 1000, ProductCode = "4567"},
                                    new OrderDetail{Price = 1000, Amount = 1000},
                                }
                            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(3, results.Count());
            Assert.AreEqual("Order.OrderDetails[0].Price", results[0].MemberName);
            Assert.AreEqual("Price does not match condition.", results[0].Message);
            Assert.AreEqual("Order.OrderDetails[1].Amount", results[1].MemberName);
            Assert.AreEqual("Amount must be greater than 0.", results[1].Message);
            Assert.AreEqual("Order.OrderDetails[2].ProductCode", results[2].MemberName);
            Assert.AreEqual("ProductCode must be longer than 3.", results[2].Message);
        }

        [Test]
        public void GetValidationResult_returns_errors_with_custom_message()
        {
            // Arrange
            var order = new Order();
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.OrderDetails", results[0].MemberName);
            Assert.AreEqual("Details cannot be null!", results[0].Message);
        }

        [Test]
        public void GetValidationResult_returns_errors_for_multiple_not_statements()
        {
            // Arrange
            var order = new Order();
            var validator = new OrderValidator2();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.ID", results[0].MemberName);
            Assert.AreEqual("ID cannot be null!", results[0].Message);
        }

        [TestCase("11", 1)]
        [TestCase("22", 1)]
        [TestCase("33", 1)]
        [TestCase("44", 0)]
        public void GetValidationResult_returns_errors_for_multiple_not_statements2(string orderID, int errorCount)
        {
            // Arrange
            var order = new Order { ID = orderID };
            var validator = new OrderValidator2();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(errorCount, results.Count());
            if (errorCount == 1)
            {
                Assert.AreEqual("Order.ID", results[0].MemberName);
                Assert.AreEqual("ID must not be one of the following values: 11, 22, 33.", results[0].Message);
            }
        }
    }
}
// ReSharper restore InconsistentNaming