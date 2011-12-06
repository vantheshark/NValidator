using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.ValidatorExtensions
{
    [TestFixture]
    public class ForEachTests : ResetDefaultValidatorFactoryTests
    {
        private readonly Order order = new Order
        {
            OrderDetails = new[] {
                                    new OrderDetail(),
                                    new OrderDetail{Amount = 1000}
                                }
        };
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.OrderDetails)
                    .ForEach(t =>
                    {
                        t.RuleFor(p => p.Amount).GreaterThan(0);
                        t.RuleFor(p => p.Price).GreaterThan(0);
                    });
            }
        }

        [Test]
        public void GetValidationResult_should_return_errors_for_all_items_in_collection()
        {
            // Arrange
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(3, results.Count());
            Assert.AreEqual("Order.OrderDetails[0].Amount", results[0].MemberName);
            Assert.AreEqual("Amount must be greater than 0.", results[0].Message);
            Assert.AreEqual("Order.OrderDetails[0].Price", results[1].MemberName);
            Assert.AreEqual("Price must be greater than 0.", results[1].Message);
            Assert.AreEqual("Order.OrderDetails[1].Price", results[2].MemberName);
            Assert.AreEqual("Price must be greater than 0.", results[2].Message);
        }

        class OrderValidator3 : TypeValidator<Order>
        {
            public OrderValidator3()
            {
                RuleFor(x => x.OrderDetails)
                    .ForEach(t =>
                    {
                        t.RuleFor(p => p.Amount).Not().LessThanOrEqual(0);
                        t.RuleFor(p => p.Price).Not().LessThanOrEqual(0);
                    });
            }
        }

        [Test]
        public void ForEach_should_work_with_complex_rules()
        {
            // Arrange
            var validator = new OrderValidator3();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(3, results.Count());
            Assert.AreEqual("Order.OrderDetails[0].Amount", results[0].MemberName);
            Assert.AreEqual("Amount must be greater than 0.", results[0].Message);
            Assert.AreEqual("Order.OrderDetails[0].Price", results[1].MemberName);
            Assert.AreEqual("Price must be greater than 0.", results[1].Message);
            Assert.AreEqual("Order.OrderDetails[1].Price", results[2].MemberName);
            Assert.AreEqual("Price must be greater than 0.", results[2].Message);
        }


        class OrderValidator2 : TypeValidator<Order>
        {
            public OrderValidator2()
            {
                RuleForEach(x => x.OrderDetails).Must(i => i.Amount > 0).WithMessage("Amount must be greater than 0.")
                                                .Must(i => i.Price > 0).WithMessage("Price must be greater than 0.");
            }
        }

        [Test]
        public void GetValidationResult_should_return_errors_for_all_items_in_collection2()
        {
            // Arrange
            var validator = new OrderValidator2();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(3, results.Count());
            Assert.AreEqual("Order.OrderDetails[0]", results[0].MemberName);
            Assert.AreEqual("Amount must be greater than 0.", results[0].Message);
            Assert.AreEqual("Order.OrderDetails[0]", results[1].MemberName);
            Assert.AreEqual("Price must be greater than 0.", results[1].Message);
            Assert.AreEqual("Order.OrderDetails[1]", results[2].MemberName);
            Assert.AreEqual("Price must be greater than 0.", results[2].Message);
        }

        private class Customer
        {
            public List<string> Address { get; set; }
        }
        class CustomerValidator : TypeValidator<Customer>
        {
            public CustomerValidator()
            {
                RuleForEach(x => x.Address)
                    .StopOnFirstError()
                    .Not()
                    .Null()
                    .NotEmpty()
                    .Length(1, 10);
            }
        }

        [Test]
        public void RuleForEach_should_work_with_complex_rules()
        {
            // Arrange
            var customer = new Customer
            {
                Address = new List<string>
                {
                    null,
                    "",
                    "123456789j0",
                    "123"
                }
            };
            var validator = new CustomerValidator();

            // Action
            var results = validator.GetValidationResult(customer).ToList();

            // Assert
            Assert.AreEqual(3, results.Count());
            Assert.AreEqual("Customer.Address[0]", results[0].MemberName);
            Assert.AreEqual("Address[0] must not be null.", results[0].Message);
            Assert.AreEqual("Customer.Address[1]", results[1].MemberName);
            Assert.AreEqual("Address[1] cannot be empty.", results[1].Message);
            Assert.AreEqual("Customer.Address[2]", results[2].MemberName);
            Assert.AreEqual("Address[2] must be between 1 and 10 in length. You had 11 in length.", results[2].Message);
        }

        [Test]
        public void RuleForEach_should_ignore_validation_if_the_enumerble_is_null()
        {
            // Arrange
            var customer = new Customer();
            var validator = new CustomerValidator();

            // Action
            var results = validator.GetValidationResult(customer).ToList();

            // Assert
            Assert.AreEqual(0, results.Count());
        }
    }
}
// ReSharper restore InconsistentNaming