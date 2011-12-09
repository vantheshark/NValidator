using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class CompositeValidatorTests : ResetDefaultValidatorFactoryTests
    {
        class OrderValidator : CompositeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails).Not().Null();
            }
        }

        class OrderDetailValidator : TypeValidator<OrderDetail>
        {
            public OrderDetailValidator()
            {
                RuleFor(x => x.Price).Must(x => x > 0);
                RuleFor(x => x.Amount).Must(x => x > 0);
                RuleFor(x => x.ProductCode).Must(x => x != null && x.Length > 3);
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
            Assert.AreEqual("Amount does not match condition.", results[1].Message);
            Assert.AreEqual("Order.OrderDetails[2].ProductCode", results[2].MemberName);
            Assert.AreEqual("ProductCode does not match condition.", results[2].Message);
        }

        class OrderValidator2 : CompositeValidator<Order>
        {
            public OrderValidator2()
            {
                RuleFor(x => x.OrderDetails[0])
                    .SetValidator<OrderDetailValidator>()
                    .When(x => x.OrderDetails != null && x.OrderDetails[0] != null);

                RuleFor(x => x.OrderDetails[1])
                    .SetValidator<CustomOrderDetailValidator>()
                    .When(x => x.OrderDetails != null && x.OrderDetails[1] != null);

            }
        }

        class CustomOrderDetailValidator : TypeValidator<OrderDetail>
        {
            public CustomOrderDetailValidator()
            {
                RuleFor(x => x.Price).GreaterThanOrEqual(100).WithMessage("Error from custom validator");
                RuleFor(x => x.Amount).GreaterThanOrEqual(2).WithMessage("Error from custom validator");
                RuleFor(x => x.ProductCode).Not().Null().Length(1, 3).WithMessage("Error from custom validator");
            }
        }

        [Test]
        public void Explicitly_set_validator_on_property_should_use_registered_validators_to_validate()
        {
            // Arrange
            ValidatorFactory.Current.Register<OrderDetailValidator>(true);
            ValidatorFactory.Current.Register<CustomOrderDetailValidator>();

            var order = new Order
            {
                OrderDetails = new[] {
                                    new OrderDetail{Amount = 1, ProductCode = "1234"},
                                    new OrderDetail{Price = 1000, ProductCode = "4567"},
                                    new OrderDetail{Price = 1000, Amount = 1000},
                                    new OrderDetail{Amount = 3, ProductCode = "VTN"},
                                }
            };
            var validator = new OrderValidator2();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(6, results.Count());
            Assert.AreEqual("Order.OrderDetails[0].Price", results[0].MemberName);
            Assert.AreEqual("Price does not match condition.", results[0].Message);

            Assert.AreEqual("Order.OrderDetails[1].Amount", results[1].MemberName);
            Assert.AreEqual("Error from custom validator", results[1].Message);

            Assert.AreEqual("Order.OrderDetails[1].ProductCode", results[2].MemberName);
            Assert.AreEqual("Error from custom validator", results[2].Message);

            Assert.AreEqual("Order.OrderDetails[2].ProductCode", results[3].MemberName);
            Assert.AreEqual("ProductCode does not match condition.", results[3].Message);

            Assert.AreEqual("Order.OrderDetails[3].Price", results[4].MemberName);
            Assert.AreEqual("Price does not match condition.", results[4].Message);

            Assert.AreEqual("Order.OrderDetails[3].ProductCode", results[5].MemberName);
            Assert.AreEqual("ProductCode does not match condition.", results[5].Message);
        }

        [Test]
        public void CompositeValidator_must_reset_the_builders_to_original_state_after_validation()
        {
            // Arrange
            ValidatorFactory.Current.Register<OrderDetailValidator>(true);
            ValidatorFactory.Current.Register<CustomOrderDetailValidator>();

            var order = new Order
            {
                OrderDetails = new[] {
                                    new OrderDetail{Amount = 1, ProductCode = "1234"},
                                    new OrderDetail{Price = 1000, ProductCode = "4567"},
                                    new OrderDetail{Price = 1000, Amount = 1000},
                                    new OrderDetail{Amount = 3, ProductCode = "VTN"},
                                }
            };
            var order2 = new Order
            {
                OrderDetails = new OrderDetail[0]
            };
            var validator = new OrderValidator2();

            // Action
            var results1 = validator.GetValidationResult(order).ToList();
            var results2 = validator.GetValidationResult(order2).ToList();

            // Assert
            Assert.AreEqual(6, results1.Count());
            Assert.AreEqual(0, results2.Count());
        }
    }
}
// ReSharper restore InconsistentNaming