using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test
{
    [TestFixture]
    public class SimpleTests : ResetDefaultValidatorFactoryTests
    {
        [Test]
        public void Should_register_default_validator_before_validation()
        {
            // Arrange
            ValidatorFactory.Current.Register<OrderDetailValidator>();

            var order = new Order
                            {
                                FirstOrderDetail = new OrderDetail {ProductCode = "P1"},
                            };
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Order.FirstOrderDetail.ProductCode", results[0].MemberName);
            Assert.AreEqual("ProductCode must be between 3 and 5 in length. You had 2 in length.", results[0].Message);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Can not resolve validator OrderDetailValidator.")]
        public void Throw_exception_if_use_a_validator_without_registration()
        {
            // Arrange
            var order = new Order
            {
                FirstOrderDetail = new OrderDetail(),
            };
            var validator = new OrderValidator();

            // Action
            validator.GetValidationResult(order).ToList();
        }

        [Test]
        public void Should_not_validate_on_null_property_even_though_the_default_validator_for_it_was_registered()
        {
            // Arrange
            ValidatorFactory.Current.Register<OrderDetailValidator>();
            var order = new Order();
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void Should_validate_on_not_null_property_if_the_default_validator_for_its_type_was_registered()
        {
            // Arrange
            ValidatorFactory.Current.Register<OrderDetailValidator>(true);
            var order = new Order
                            {
                                OrderDetails = new[] {new OrderDetail()}
                            };
            var validator = new CompositeOrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(3, results.Count);

            Assert.AreEqual("Order.OrderDetails[0].ProductCode", results[0].MemberName);
            Assert.AreEqual("ProductCode must not be null.", results[0].Message);

            Assert.AreEqual("Order.OrderDetails[0].ProductCode", results[1].MemberName);
            Assert.AreEqual("ProductCode cannot be empty.", results[1].Message);

            Assert.AreEqual("Order.OrderDetails[0].ProductCode", results[2].MemberName);
            Assert.AreEqual("ProductCode must be between 3 and 5 in length. You had 0 in length.", results[2].Message);
        }
    }
}
// ReSharper restore InconsistentNaming