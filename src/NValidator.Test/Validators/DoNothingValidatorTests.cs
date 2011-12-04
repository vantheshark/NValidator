using System.Linq;
using NUnit.Framework;
using NValidator.Test.Models;
using NValidator.Validators;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Validators
{
    [TestFixture]
    public class DoNothingValidatorTests
    {
        [Test]
        public void GetValidationResult_must_return_emtpy_result()
        {
            // Arrange
            var order = new Order
                            {
                                OrderDetails = new [] {
                                    new OrderDetail()
                                }
                            };
            var validator = new DoNothingValidator();

            // Action
            var results = validator.GetValidationResult(order);

            // Assert
            Assert.AreEqual(0, results.Count());
        }
    }
}
// ReSharper restore InconsistentNaming