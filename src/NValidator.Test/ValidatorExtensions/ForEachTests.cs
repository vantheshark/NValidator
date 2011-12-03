using System.Linq;
using NUnit.Framework;
// ReSharper disable InconsistentNaming
namespace NValidator.Test.ValidatorExtensions
{
    [TestFixture]
    public class ForEachTests : ResetDefaultValidatorFactoryTests
    {
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
            var order = new Order
            {
                OrderDetails = new[] {
                                    new OrderDetail(),
                                    new OrderDetail{Amount = 1000}
                                }
            };
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

    }
}
// ReSharper restore InconsistentNaming