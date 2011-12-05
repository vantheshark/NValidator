using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;
// ReSharper disable InconsistentNaming
namespace NValidator.Test
{
    [TestFixture]
    public class RulesForTests
    {
        class OrderValidator : TypeValidator<Order>
        {
            public OrderValidator()
            {
                RulesFor(x => x.FirstOrderDetail.ProductCode, 
                         x => x.OrderDetails[0].ProductCode)
                    .Not().Null()
                    .WithMessage("ProductCode must not be null.");
            }
        }

        [Test]
        public void GetValidationResult_should_return_errors_for_all_properties_set_in_rules()
        {
            // Arrange
            var order = new Order
                            {
                                FirstOrderDetail = new OrderDetail(),
                                OrderDetails = new [] {new OrderDetail()}
                            };
                           
            var validator = new OrderValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Order.FirstOrderDetail.ProductCode", results[0].MemberName);
            Assert.AreEqual("ProductCode must not be null.", results[0].Message);

            Assert.AreEqual("Order.OrderDetails[0].ProductCode", results[1].MemberName);
            Assert.AreEqual("ProductCode must not be null.", results[1].Message);
        }

        class AddressValidator : TypeValidator<Address>
        {
            public AddressValidator()
            {
                RulesFor(x => x.Number, x => x.Street, x => x.Suburb, x => x.PostCode)
                    .StopOnFirstError()
                    .Not().Null()
                    .NotEmpty().When(x => !string.IsNullOrEmpty(x.Country))
                    .AllWithMessage(x => "The Address detail is not valid.");
            }
        }

        [Test]
        public void GetValidationResult_should_return_errors_for_all_properties_set_in_Address_rules()
        {
            // Arrange
            var order = new Address
            {
                Country = "Australia"
            };

            var validator = new AddressValidator();

            // Action
            var results = validator.GetValidationResult(order).ToList();

            // Assert
            Assert.AreEqual(4, results.Count());
            Assert.AreEqual("Address.Number", results[0].MemberName);
            Assert.AreEqual("The Address detail is not valid.", results[0].Message);

            Assert.AreEqual("Address.Street", results[1].MemberName);
            Assert.AreEqual("The Address detail is not valid.", results[1].Message);

            Assert.AreEqual("Address.Suburb", results[2].MemberName);
            Assert.AreEqual("The Address detail is not valid.", results[2].Message);

            Assert.AreEqual("Address.PostCode", results[3].MemberName);
            Assert.AreEqual("The Address detail is not valid.", results[3].Message);
        }
    }
}
// ReSharper restore InconsistentNaming