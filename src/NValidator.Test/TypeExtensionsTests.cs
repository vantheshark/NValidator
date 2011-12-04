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
    }
}
// ReSharper restore InconsistentNaming