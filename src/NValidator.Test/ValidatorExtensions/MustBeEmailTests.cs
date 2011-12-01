using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.ValidatorExtensions
{
    [TestFixture]
    public class MustBeEmailTests
    {
        class Customer
        {
            public string Email { get; set; }
        }

        class CustomerValidator : TypeValidator<Customer>
        {
            public CustomerValidator()
            {
                RuleFor(x => x.Email).Email();
            }
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_email_is_not_valid()
        {
            // Arrange
            var customer = new Customer{Email = "abc@."};
            var validator = new CustomerValidator();

            // Action
            var results = validator.GetValidationResult(customer).ToList();

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Customer.Email", results[0].MemberName);
            Assert.AreEqual("Email is not a valid email address.", results[0].Message);
        }

        [Test]
        public void GetValidationResult_return_one_error_result_if_the_email_is_valid()
        {
            // Arrange
            var customer = new Customer { Email = "th1s.Is.an-3mail@gmail.com" };
            var validator = new CustomerValidator();

            // Action
            var results = validator.GetValidationResult(customer).ToList();

            // Assert
            Assert.AreEqual(0, results.Count());
        }
    }
}
// ReSharper restore InconsistentNaming