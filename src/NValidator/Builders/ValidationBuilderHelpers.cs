using System;
using System.Linq.Expressions;

namespace NValidator.Builders
{
    public class ValidationBuilderHelpers
    {
        internal static IValidationBuilder<T, TProperty> CreateGenericBuilder<T, TProperty>(Expression<Func<T, TProperty>> expression, Type defaultBuilderType)
        {
            IValidationBuilder<T, TProperty> newBuilder = null;
            if (defaultBuilderType == typeof(ValidationBuilder<,>) || defaultBuilderType == null)
            {
                newBuilder = new ValidationBuilder<T, TProperty>(expression);
            }
            else if (defaultBuilderType.Is(typeof(ValidationBuilder<,>)))
            {
                Type generic = defaultBuilderType.MakeGenericType(typeof(T), typeof(TProperty));
                newBuilder = Activator.CreateInstance(generic, expression) as IValidationBuilder<T, TProperty>;
            }

            if (newBuilder != null)
            {
                newBuilder.Validator = ValidatorFactory.NullValidator;
                return newBuilder;
            }

            throw new Exception("Invalid default validation builder type.");
        }

        internal static IValidationBuilder<T> CreateGenericBuilder<T>(Type propertyType, object propertyValue, Type defaultBuilderType)
        {
            IValidationBuilder<T> newBuilder = null;
            if (defaultBuilderType != null && defaultBuilderType.Is(typeof(ValidationBuilder<,>)))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var body = Expression.Constant(propertyValue, propertyType);
                var lambda = Expression.Lambda(body, parameter);

                Type generic = defaultBuilderType.MakeGenericType(typeof(T), propertyType);
                newBuilder = Activator.CreateInstance(generic, lambda) as IValidationBuilder<T>;
            }

            if (newBuilder != null)
            {
                newBuilder.Validator = ValidatorFactory.NullValidator;
                return newBuilder;
            }

            throw new Exception("Invalid default validation builder type.");
        }

        /// <summary>
        /// Updates BeforeValidation for all the builders in current chain by executing provided action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lastBuilder">The last builder.</param>
        /// <param name="action">The action.</param>
        public static void UpdateBuilderChain<T>(IValidationBuilder<T> lastBuilder, Action<IValidationBuilder<T>> action)
        {
            Action<IValidationBuilder<T>> updateBuilder = x =>
            {
                var originValue = x.BeforeValidation;
                x.BeforeValidation = (builder, context) =>
                {
                    if (originValue != null)
                    {
                        originValue(builder, context);
                    }
                    action(x);
                };
            };

            var pointer = lastBuilder;
            while (pointer != null)
            {
                updateBuilder(pointer);
                pointer = pointer.Previous;
            }
        }
    }
}
