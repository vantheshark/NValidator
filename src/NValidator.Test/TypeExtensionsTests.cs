using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using NValidator.Builders;
// ReSharper disable InconsistentNaming
namespace NValidator.Test
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        class TestValidationBuilder<T, TProperty> : ValidationBuilder<T, TProperty>
        {
            public TestValidationBuilder(Expression<Func<T, TProperty>> expression)
                : base(expression)
            {
            }
        }

        class ChildTestValidationBuilder<T, TProperty> : TestValidationBuilder<T, TProperty>
        {
            public ChildTestValidationBuilder(Expression<Func<T, TProperty>> expression)
                : base(expression)
            {
            }
        }

        [TestCase(typeof(TestValidationBuilder<,>), true)]
        [TestCase(typeof(ChildTestValidationBuilder<,>), true)]
        [TestCase(typeof(ValidationBuilder<,>), true)]
        [TestCase(typeof(EnumerableAdapterValidationBuilder<,,>), true)]
        [TestCase(typeof(Dictionary<,>), false)]
        public void Is_should_return_true_if_a_type_is_derivative_of_ValidationBuilder(Type type, bool result)
        {
            Assert.AreEqual(result, type.Is(typeof(ValidationBuilder<, >)));
        }

        class A
        { }

        class AValidator : TypeValidator<A>
        { }

        class B : A
        { }

        class BValidator : TypeValidator<B>
        { }

        [Test]
        public void Covariance_validator_type_tests()
        {
            // Arrange
            var aValidator = new AValidator();
            var bValidator = new BValidator();
            
            // Assert
            Assert.IsFalse(bValidator is IValidator<A>);
            Assert.IsFalse(bValidator.GetType().Is<IValidator<A>>());

            Assert.IsTrue(aValidator is IValidator<B>); // Covariance. 
            Assert.IsTrue(aValidator.GetType().Is<IValidator<B>>());
        }
        

        [Test]
        public void IsMoreConcreteGenericParam_return_false_if_one_of_the_param_is_not_generic_type()
        {
            // Arrange
            var aValidator = typeof(AValidator);
            var bValidator = typeof(BValidator);

            // Assert
            Assert.IsFalse(aValidator.IsMoreConcreteGenericParam(bValidator));
            Assert.IsTrue(bValidator.IsMoreConcreteGenericParam(aValidator));
        }
    }
}
// ReSharper restore InconsistentNaming