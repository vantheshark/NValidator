using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.ValidatorExtensions
{
    [TestFixture]
    public class IgnoreValidationTests
    {
        class OrderDetailValidator : TypeValidator<OrderDetail>
        {
            public OrderDetailValidator()
            {
                RuleFor(x => x.Price).Must(x => x > 0);
                RuleFor(x => x.Amount).Must(x => x > 0);
            }
        }

        class OrderValidator : CompositeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails[0].Price).IgnoreValidation();
            }
        }

        class OrderValidator2 : CompositeValidator<Order>
        {
            public OrderValidator2()
            {
                RuleFor(x => x.OrderDetails[0]).IgnoreValidation();
            }
        }

        class OrderValidator3 : CompositeValidator<Order>
        {
            public OrderValidator3()
            {
                RuleFor(x => x.OrderDetails).IgnoreValidation();
            }
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var factory = new Mock<IValidatorFactory>();
            factory.Setup(x => x.GetValidatorFor(It.Is<Type>(t => t == typeof(OrderDetail))))
                   .Returns(() => new OrderDetailValidator());
            factory.Setup(x => x.IsDefaultValidator(It.Is<Type>(t => t == typeof (OrderDetailValidator)))).Returns(true);
            ValidatorFactory.Current = factory.Object;
        }

        [Test]
        public void GetValidationResult_should_ignore_all_ignored_validations()
        {
            // Arrange
            var order = new Order
                            {
                                OrderDetails = new [] {
                                    new OrderDetail(),
                                    new OrderDetail{Amount = 1000}
                                }
                            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Order.OrderDetails[0].Amount", results[0].MemberName);
            Assert.AreEqual("Amount does not match condition.", results[0].Message);
            Assert.AreEqual("Order.OrderDetails[1].Price", results[1].MemberName);
            Assert.AreEqual("Price does not match condition.", results[1].Message);
        }

        [Test]
        public void GetValidationResult_should_ignore_all_validations_on_ignored_property()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new[] {
                                    new OrderDetail(),
                                    new OrderDetail{Amount = 1000}
                                }
            };
            var validator = new OrderValidator2();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.OrderDetails[1].Price", results[0].MemberName);
            Assert.AreEqual("Price does not match condition.", results[0].Message);
        }

        [Test]
        public void GetValidationResult_should_ignore_all_validations_on_all_children_of_an_ignored_collection_property()
        {
            // Arrange
            var order = new Order
            {
                OrderDetails = new[] {
                                    new OrderDetail(),
                                    new OrderDetail{Amount = 1000}
                                }
            };
            var validator = new OrderValidator3();

            // Action
            var results = validator.GetValidationResult(order).ToList();

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