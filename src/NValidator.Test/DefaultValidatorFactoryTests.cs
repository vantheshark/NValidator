using System;
using NUnit.Framework;
// ReSharper disable InconsistentNaming
namespace NValidator.Test
{
    [TestFixture]
    public class DefaultValidatorFactoryTests : ResetDefaultValidatorFactoryTests
    {
        [Test]
        public void Register_should_add_the_validator_type_to_cache()
        {
            // Action
            ValidatorFactory.Current.Register<OrderValidator>();

            // Assert
            Assert.IsNotNull(ValidatorFactory.Current.Resolve<IValidator<Order>>());
            Assert.IsNull(ValidatorFactory.Current.GetValidatorFor<Order>());
            Assert.IsNull(ValidatorFactory.Current.GetValidatorFor(typeof(Order)));
        }

        [Test]
        public void Register_instance_should_add_the_validator_type_to_cache()
        {
            // Arrange
            var v1 = new OrderValidator();

            // Action
            ValidatorFactory.Current.Register(v1);

            // Assert
            Assert.IsNotNull(ValidatorFactory.Current.Resolve<IValidator<Order>>());
            Assert.IsNull(ValidatorFactory.Current.GetValidatorFor<Order>());
            Assert.IsNull(ValidatorFactory.Current.GetValidatorFor(typeof(Order)));
        }

        [Test]
        public void Register_should_override_the_validator_with_same_type()
        {
            // Arrange
            var v1 = new OrderValidator();
            var v2 = new OrderValidator();

            // Action
            ValidatorFactory.Current.Register(v1);
            ValidatorFactory.Current.Register(v2);

            // Assert
            Assert.AreNotSame(v1, v2);
            Assert.AreSame(v2, ValidatorFactory.Current.Resolve<OrderValidator>());
            Assert.IsNull(ValidatorFactory.Current.GetValidatorFor<Order>());
            Assert.IsNull(ValidatorFactory.Current.GetValidatorFor(typeof(Order)));
        }


        class OrderValidator2 : OrderValidator {}
        [Test]
        public void Register_should_not_override_the_validators_for_same_type()
        {
            // Arrange
            var v1 = new OrderValidator();
            var v2 = new OrderValidator2();

            // Action
            ValidatorFactory.Current.Register(v1);
            ValidatorFactory.Current.Register(v2);

            // Assert
            Assert.AreNotSame(v1, v2);
            Assert.AreSame(v1, ValidatorFactory.Current.Resolve<OrderValidator>());
            Assert.AreSame(v2, ValidatorFactory.Current.Resolve<OrderValidator2>());
        }

        [Test]
        public void Resolve_should_return_the_demand_type_instead_of_default_type()
        {
            // Arrange
            var v1 = new OrderValidator();
            var v2 = new OrderValidator2();

            // Action
            ValidatorFactory.Current.Register(v1, true);
            ValidatorFactory.Current.Register(v2);

            // Assert
            Assert.AreNotSame(v1, v2);
            Assert.AreSame(v1, ValidatorFactory.Current.Resolve<OrderValidator>());
            Assert.AreSame(v1, ValidatorFactory.Current.GetValidatorFor<Order>());
            Assert.AreSame(v1, ValidatorFactory.Current.GetValidatorFor(typeof(Order)));

            Assert.AreSame(v2, ValidatorFactory.Current.Resolve<OrderValidator2>());
        }

        [Test]
        public void Resolve_should_return_the_demand_type_instead_of_default_type_2()
        {
            // Arrange
            var v1 = typeof(OrderValidator);
            var v2 = typeof(OrderValidator2);

            // Action
            ValidatorFactory.Current.Register<OrderValidator>(true);
            ValidatorFactory.Current.Register<OrderValidator2>();

            // Assert
            Assert.AreEqual(v1, ValidatorFactory.Current.Resolve<OrderValidator>().GetType());
            Assert.AreEqual(v1, ValidatorFactory.Current.GetValidatorFor<Order>().GetType());
            Assert.AreEqual(v1, ValidatorFactory.Current.GetValidatorFor(typeof(Order)).GetType());

            Assert.AreEqual(v2, ValidatorFactory.Current.Resolve<OrderValidator2>().GetType());
        }

        [Test]
        public void Resolve_should_return_the_default_validator_as_highest_priority()
        {
            // Arrange
            var v1 = new OrderValidator();
            var v2 = new OrderValidator2();

            // Action
            ValidatorFactory.Current.Register(v2);
            ValidatorFactory.Current.Register(v1, true);

            // Assert
            Assert.AreSame(v1, ValidatorFactory.Current.Resolve<IValidator<Order>>());
            Assert.AreSame(v1, ValidatorFactory.Current.GetValidatorFor<Order>());
            Assert.AreSame(v1, ValidatorFactory.Current.GetValidatorFor(typeof(Order)));
        }

        [Test]
        public void Resolve_should_return_the_default_validator_as_highest_priority_2()
        {
            // Arrange
            var v1 = typeof(OrderValidator);

            // Action
            ValidatorFactory.Current.Register<OrderValidator2>();
            ValidatorFactory.Current.Register<OrderValidator>(true);

            // Assert
            Assert.AreEqual(v1, ValidatorFactory.Current.Resolve<IValidator<Order>>().GetType());
            Assert.AreEqual(v1, ValidatorFactory.Current.GetValidatorFor<Order>().GetType());
            Assert.AreEqual(v1, ValidatorFactory.Current.GetValidatorFor(typeof(Order)).GetType());
        }

        [Test]
        public void Register_default_type_should_add_the_validator_type_to_default_cache()
        {
            // Action
            ValidatorFactory.Current.Register<OrderValidator>(true);

            // Assert
            Assert.IsNotNull(ValidatorFactory.Current.Resolve<IValidator<Order>>());
            Assert.IsNotNull(ValidatorFactory.Current.GetValidatorFor<Order>());
            Assert.IsNotNull(ValidatorFactory.Current.GetValidatorFor(typeof(Order)));
        }

        [Test]
        public void Resolve_should_return_a_pre_registerred_validator()
        {
            // Action
            ValidatorFactory.Current.Register(new OrderValidator(), true);

            // Assert
            Assert.IsNotNull(ValidatorFactory.Current.Resolve<IValidator<Order>>());
            Assert.IsNotNull(ValidatorFactory.Current.GetValidatorFor<Order>());
            Assert.IsNotNull(ValidatorFactory.Current.GetValidatorFor(typeof(Order)));
        }

        [Test]
        public void Resolve_using_base_type_should_return_a_pre_registerred_validator()
        {
            // Action
            ValidatorFactory.Current.Register<OrderValidator>();

            // Assert
            Assert.IsNotNull(ValidatorFactory.Current.Resolve<BaseValidator<Order>>());
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Can not resolve validator for Order.")]
        public void Resolve_using_base_type_should_throw_exception_if_there_is_no_registerred_validator()
        {
            // Action
            ValidatorFactory.Current.Resolve<BaseValidator<Order>>();
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Can not resolve validator OrderValidator.")]
        public void Resolve_using_base_type_should_throw_exception_if_there_is_no_registerred_validator2()
        {
            // Action
            ValidatorFactory.Current.Resolve<OrderValidator>();
        }

        [Test]
        public void GetValidatorFor_should_return_a_pre_registerred_validator()
        {
            // Action
            ValidatorFactory.Current.Register<OrderValidator>(true);

            // Assert
            Assert.IsNotNull(ValidatorFactory.Current.GetValidatorFor<Order>());
            Assert.IsNotNull(ValidatorFactory.Current.GetValidatorFor(typeof(Order)));
        }


        [Test]
        public void IsDefaultValidator_should_return_a_true_if_type_is_registered_as_default()
        {
            // Action
            ValidatorFactory.Current.Register<OrderValidator>(true);
            ValidatorFactory.Current.Register<OrderValidator2>();

            // Assert
            Assert.IsTrue(ValidatorFactory.Current.IsDefaultValidator(typeof(OrderValidator)));
            Assert.IsTrue(ValidatorFactory.Current.IsDefaultValidator<OrderValidator>());

            Assert.IsFalse(ValidatorFactory.Current.IsDefaultValidator(typeof(OrderValidator2)));
            Assert.IsFalse(ValidatorFactory.Current.IsDefaultValidator<OrderValidator2>());
        }

        [Test]
        public void IsDefaultValidator_should_throw_exception_if_type_is_abstract_or_interface()
        {
            // Assert
            Assert.Throws<ArgumentException>(() => ValidatorFactory.Current.IsDefaultValidator(typeof(IValidator)));
            Assert.Throws<ArgumentException>(() => ValidatorFactory.Current.IsDefaultValidator(typeof(BaseValidator)));

        }
    }
}
// ReSharper restore InconsistentNaming