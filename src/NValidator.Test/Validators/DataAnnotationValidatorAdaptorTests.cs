using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class DataAnnotationValidatorAdaptorTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(x => x.Name).ValidateUsing(typeof(RequiredAttribute)).WithMessage("'@PropertyName' is required!!!!");
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_order_name_is_null()
        {
            // Arrange
            var order = new Order();
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.Name", results[0].MemberName);
            Assert.AreEqual("'Name' is required!!!!", results[0].Message);
        }
    }
}
// ReSharper restore InconsistentNaming