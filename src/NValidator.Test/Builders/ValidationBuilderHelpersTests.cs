using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using NValidator.Builders;
using NValidator.Test.Models;

// ReSharper disable InconsistentNaming
namespace NValidator.Test.Builders
{
    [TestFixture]
    public class ValidationBuilderHelpersTests
    {
        private readonly Expression<Func<Order, OrderDetail>> expression = x => x.OrderDetails[0];

        [Test]
        public void CreateGenericBuilder_create_ValidationBuilder_if_default_type_is_null()
        {
            // Action
            var builder = ValidationBuilderHelpers.CreateGenericBuilder(expression, null);

            // Assert
            Assert.That(builder, Is.TypeOf(typeof(ValidationBuilder<Order, OrderDetail>)));
        }

        [Test]
        public void CreateGenericBuilder_create_ValidationBuilder_if_default_generic_type_is_ValidationBuilder()
        {
            // Action
            var builder = ValidationBuilderHelpers.CreateGenericBuilder(expression, typeof(ValidationBuilder<, >));

            // Assert
            Assert.That(builder, Is.TypeOf(typeof(ValidationBuilder<Order, OrderDetail>)));
        }

        class TestValidationBuilder<T, TProperty> : ValidationBuilder<T, TProperty>
        {
            public TestValidationBuilder(Expression<Func<T, TProperty>> expression) : base(expression)
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

        [TestCase(typeof(ValidationBuilder<,>), typeof(ValidationBuilder<Order, OrderDetail>))]
        [TestCase(typeof(TestValidationBuilder<,>), typeof(TestValidationBuilder<Order, OrderDetail>))]
        [TestCase(typeof(ChildTestValidationBuilder<,>), typeof(ChildTestValidationBuilder<Order, OrderDetail>))]
        public void CreateGenericBuilder_create_builder_if_provide_an_inherit_type_of_generic_ValidationBuilder(Type defaultType, Type expectedType)
        {
            // Action
            var builder = ValidationBuilderHelpers.CreateGenericBuilder(expression, defaultType);

            // Assert
            Assert.That(builder, Is.TypeOf(expectedType));
        }


        [TestCase(typeof(Dictionary<,>), typeof(ValidationBuilder<Order, OrderDetail>))]
        public void CreateGenericBuilder_throw_exception_if_cannot_create_validation_builder(Type defaultType, Type expectedType)
        {
            // Action
            Assert.Throws<Exception>(() => ValidationBuilderHelpers.CreateGenericBuilder(expression, defaultType));
        }
    }
}
// ReSharper restore InconsistentNaming