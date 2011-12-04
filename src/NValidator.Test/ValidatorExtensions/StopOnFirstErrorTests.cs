using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.ValidatorExtensions
{
    [TestFixture]
    public class StopOnFirstErrorTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.ID).NotEmpty();

                RuleFor(x => x.Name)
                    .StopOnFirstError()
                    .NotNull().WithMessage("@PropertyName cannot be null!!")
                    .NotEmpty().WithMessage("@PropertyName cannot be empty!!")
                    .Length(5, 10);

                RuleFor(x => x.OrderDetails).NotNull();
            }
        }

        [Test]
        public void GetValidationResult_returns_only_one_error_if_Name_is_null()
        {
            // Arrange
            var order = new Order{ID = "ID", OrderDetails = new OrderDetail[]{}};
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.Name", results[0].MemberName);
            Assert.AreEqual("Name cannot be null!!", results[0].Message);
        }

        [Test]
        public void GetValidationResult_returns_only_one_error_if_Name_is_empty()
        {
            // Arrange
            var order = new Order { ID = "ID", Name = "", OrderDetails = new OrderDetail[] { } };

            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.Name", results[0].MemberName);
            Assert.AreEqual("Name cannot be empty!!", results[0].Message);
        }

        [Test]
        public void GetValidationResult_returns_only_one_error_if_Name_length_is_not_valid()
        {
            // Arrange
            var order = new Order { ID = "ID", Name = "1234", OrderDetails = new OrderDetail[] { } };

            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.Name", results[0].MemberName);
            Assert.AreEqual("Name must be between 5 and 10 in length. You had 4 in length.", results[0].Message);
        }

        [Test]
        public void GetValidationResult_returns_other_error_from_different_rules()
        {
            // Arrange
            var order = new Order { ID = "ID", Name = "1234" };

            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Order.Name", results[0].MemberName);
            Assert.AreEqual("Name must be between 5 and 10 in length. You had 4 in length.", results[0].Message);

            Assert.AreEqual("Order.OrderDetails", results[1].MemberName);
            Assert.AreEqual("OrderDetails must not be null.", results[1].Message);
        }
        
        [Test]
        public void GetValidationResult_returns_other_error_from_different_rules2()
        {
            // Arrange
            var order = new Order { ID = "ID", Name = "12345" };

            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.OrderDetails", results[0].MemberName);
            Assert.AreEqual("OrderDetails must not be null.", results[0].Message);
        }
    }
}
// ReSharper restore InconsistentNaming