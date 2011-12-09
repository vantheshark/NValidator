using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class TypeValidatorTests
    {
        class OrderDetailValidator : TypeValidator<OrderDetail>
        {
            public OrderDetailValidator()
            {
                RuleFor(x => x.Price).Must(x => x > 0);
                RuleFor(x => x.Amount).Must(x => x > 0);
                RuleFor(x => x.ProductCode).Must(x => x != null && x.Length > 3);
            }
        }
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.FirstOrderDetail).Not()
                                                .Null()
                                                .When(x => x.OrderDetails != null &&
                                                           x.OrderDetails.Count() > 0);
            }
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var factory = new Mock<IValidatorFactory>();
            factory.Setup(x => x.GetValidatorFor(It.Is<Type>(t => t == typeof(OrderDetail))))
                   .Returns(() => new OrderDetailValidator());
            ValidatorFactory.Current = factory.Object;
        }

        [Test]
        public void GetValidationResult_does_not_validate_items_in_collection_property()
        {
            // Arrange
            var order = new Order
                            {
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
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.FirstOrderDetail", results[0].MemberName);
            Assert.AreEqual("FirstOrderDetail must not be null.", results[0].Message);
        }

        class WhenOrderValidator : TypeValidator<Order>
        {
            public WhenOrderValidator()
            {
                When(x => x.FirstOrderDetail != null, () =>
                {
                    RuleFor(x => x.FirstOrderDetail.Price).GreaterThan(0);

                    RuleFor(x => x.FirstOrderDetail.Amount).GreaterThan(0);

                    RuleFor(x => x.FirstOrderDetail.ProductCode).StopOnFirstError().NotNull().NotEmpty();
                });
            }
        }
        [Test]
        public void When_should_return_validation_results_for_the_builder_under_condition()
        {
            // Arrange
            var builder = new WhenOrderValidator();
            var order = new Order
                            {
                                FirstOrderDetail = new OrderDetail()
                            };

            // Action
            var results = builder.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(3, results.Count());
            Assert.AreEqual("Order.FirstOrderDetail.Price", results[0].MemberName);
            Assert.AreEqual("Price must be greater than 0.", results[0].Message);

            Assert.AreEqual("Order.FirstOrderDetail.Amount", results[1].MemberName);
            Assert.AreEqual("Amount must be greater than 0.", results[1].Message);

            Assert.AreEqual("Order.FirstOrderDetail.ProductCode", results[2].MemberName);
            Assert.AreEqual("ProductCode must not be null.", results[2].Message);
        }

        [Test]
        public void When_should_return_empty_results_if_the_condition_not_match()
        {
            // Arrange
            var builder = new WhenOrderValidator();
            var order = new Order();

            // Action
            var results = builder.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(0, results.Count());
        }

        [TestFixtureTearDown]
        public void TestFixtureTeardown()
        {
            ValidatorFactory.Current = new NullValidatorFactory();
        }
    }
}
// ReSharper restore InconsistentNaming