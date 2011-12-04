using NUnit.Framework;
using NValidator.Builders;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Builders
{
    [TestFixture]
    public class ValidationBuilderTests
    {
        [Test]
        public void ChainName_should_return_private_field_if_it_has_value()
        {
            // Arrange
            var builder = new ValidationBuilder<Order, string>(x => x.ID) {ChainName = "My.Chain.Name"};

            // Action
            var name = builder.ChainName;

            // Assert
            Assert.That(name, Is.EqualTo("My.Chain.Name"));
        }

        [Test]
        public void ChainName_should_return_value_resolved_from_expression__if_private_field_does_not_have_value()
        {
            // Arrange
            var builder = new ValidationBuilder<Order, string>(x => x.ID);

            // Action
            var name = builder.ChainName;

            // Assert
            Assert.That(name, Is.EqualTo("Order.ID"));
        }

        [Test]
        public void ChainName_should_return_container_name_if_cannot_resolve_value_from_expression()
        {
            // Arrange
            var builder = new ValidationBuilder<Order, string>(null);
            builder.UpdateContainerName("The.Order");

            // Action
            var name = builder.ChainName;

            // Assert
            Assert.That(name, Is.EqualTo("The.Order"));
        }

        [Test]
        public void ContainerName_when_set_will_update_chain_name()
        {
            // Arrange
            var builder = new ValidationBuilder<Order, string>(null);
            builder.ChainName = "Chain.Name";
            Assert.That(builder.ChainName, Is.EqualTo("Chain.Name"));
            builder.UpdateContainerName("The.Order");

            // Action
            var name = builder.ChainName;

            // Assert
            Assert.That(name, Is.EqualTo("The.Order"));
        }

        [Test]
        public void GetChainFromExpression_should_return_a_chain()
        {
            // Arrange
            var builder = new ValidationBuilder<Order, string>(x => x.OrderDetails[0].ProductCode);
            builder.UpdateContainerName("The.Order");

            // Action
            var name = builder.ChainName;

            // Assert
            Assert.That(name, Is.EqualTo("The.Order.OrderDetails[0].ProductCode"));
        }

        [Test]
        public void GetChainFromExpression_should_return_a_container_if_expression_type_is_not_member_access()
        {
            // Arrange
            var builder = new ValidationBuilder<Order, string>(x => "Not A Member Access Expression");
            builder.UpdateContainerName("The.Order.OrderDetails[0].Id");

            // Action
            var name = builder.ChainName;

            // Assert
            Assert.That(name, Is.EqualTo("The.Order.OrderDetails[0].Id"));
        }

        [Test]
        public void GetChainFromExpression_should_return_a_chain_with_a_container_as_prefix()
        {
            // Arrange
            var builder = new ValidationBuilder<Order, string>(x => x.OrderDetails[0].ProductCode);
            builder.UpdateContainerName("Container");

            // Action
            var name = builder.ChainName;

            // Assert
            Assert.That(name, Is.EqualTo("Container.OrderDetails[0].ProductCode"));
        }

        
    }
}
// ReSharper restore InconsistentNaming