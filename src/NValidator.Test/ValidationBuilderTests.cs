using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NValidator.Builders;

namespace NValidator.Test
{
    [TestFixture]
    public class ValidationBuilderTests
    {
        [Test]
        public void Test_something()
        {
            var parameter = Expression.Parameter(typeof(Order), "x");
            var body = Expression.Constant("someValue", typeof(string));
            
            
            var lambda = Expression.Lambda(body, parameter);

            var value = lambda.Compile().DynamicInvoke(new Order());
            var myClassType = typeof(ValidationBuilder<,>).MakeGenericType(typeof(Order), typeof(string));

            var myClassInstance = Activator.CreateInstance(myClassType, lambda);

        }
    }
}
