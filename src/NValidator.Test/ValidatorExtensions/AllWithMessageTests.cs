using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.ValidatorExtensions
{
    [TestFixture]
    public class AllWithMessageTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.ID)
                    .NotNull()
                    .NotEmpty().When(x => x.ID != null)
                    .NotEqual("123").When(x => !string.IsNullOrEmpty(x.ID))
                    .AllWithMessage(x => "Invalid ID.");
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_ID_is_null()
        {
            // Arrange
            var order = new Order();
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.ID", results[0].MemberName);
            Assert.AreEqual("Invalid ID.", results[0].Message);
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_ID_is_empty()
        {
            // Arrange
            var order = new Order{ID = ""};
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.ID", results[0].MemberName);
            Assert.AreEqual("Invalid ID.", results[0].Message);
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_ID_equals_to_123()
        {
            // Arrange
            var order = new Order { ID = "123" };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.ID", results[0].MemberName);
            Assert.AreEqual("Invalid ID.", results[0].Message);
        }

        [Test]
        public void GetValidationResult_return_no_error_result_if_the_ID_not_equals_to_123()
        {
            // Arrange
            var order = new Order { ID = "1234" };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(0, results.Count());
        }
    }
}
// ReSharper restore InconsistentNaming