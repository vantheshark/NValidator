using System;
using System.Linq.Expressions;

namespace NValidator.Builders
{
    public class ValidationBuilderHelpers
    {
        internal static IValidationBuilder<T, TProperty> CreateGenericBuilder<T, TProperty>(Expression<Func<T, TProperty>> expression, Type defaultBuilderType)
        {
            IValidationBuilder<T, TProperty> newBuilder = null;
            if (defaultBuilderType == null || defaultBuilderType.GetGenericTypeDefinition() == typeof(ValidationBuilder<,>))
            {
                newBuilder = new ValidationBuilder<T, TProperty>(expression);
            }
            else if (defaultBuilderType.Is(typeof(ValidationBuilder<,>)) && defaultBuilderType.IsGenericTypeDefinition)
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
        /// <param name="currentBuilder">The last builder.</param>
        /// <param name="action">The action.</param>
        /// <param name="leftToRight">if set to <c>true</c> [left to right].</param>
        public static void UpdateBuilderChain<T>(IValidationBuilder<T> currentBuilder, Action<IValidationBuilder<T>> action, bool leftToRight = false)
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

            var pointer = currentBuilder;
            while (pointer != null)
            {
                updateBuilder(pointer);
                pointer = GetNextBuilder(pointer, leftToRight);
            }
        }

        public static void UpdateBuilderChain<T, TProperty>(IValidationBuilder<T, TProperty> currentBuilder, Action<IValidationBuilder<T, TProperty>> action, bool leftToRight = false)
        {
            Action<IValidationBuilder<T, TProperty>> updateBuilder = x =>
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

            var pointer = currentBuilder;
            while (pointer != null)
            {
                updateBuilder(pointer);
                pointer = GetNextBuilder(pointer, leftToRight) as IValidationBuilder<T, TProperty>;
            }
        }

        private static IValidationBuilder<T> GetNextBuilder<T>(IChainOfValidationBuilder<T> builder, bool leftToRight)
        {
            return leftToRight ? builder.Next : builder.Previous;
        }

        /// <summary>
        /// Clones the chain begin from the provided builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="firstBuilder">The first builder.</param>
        /// <param name="action">The action to execute on each cloned builder on the chain.</param>
        /// <returns></returns>
        public static IValidationBuilder<T, TProperty> CloneChain<T, TProperty>(IValidationBuilder<T, TProperty> firstBuilder, Action<IValidationBuilder<T, TProperty>> action = null)
        {
            var newBeginingBuilder = (IValidationBuilder<T, TProperty>)firstBuilder.Clone();
            var pointer = firstBuilder.Next as IValidationBuilder<T, TProperty>;
            var newBuilderPointer = newBeginingBuilder;
            if (action != null)
            {
                action(newBeginingBuilder);
            }
            while (pointer != null)
            {
                //var nextBuilder = CloneBuilderWithoutConnection(pointer);
                var nextBuilder = (IValidationBuilder<T, TProperty>) pointer.Clone();
                newBuilderPointer.Next = nextBuilder;
                nextBuilder.Previous = newBuilderPointer;
                if (action != null)
                {
                    action(nextBuilder);
                }

                pointer = pointer.Next as IValidationBuilder<T, TProperty>;
                newBuilderPointer = newBuilderPointer.Next as IValidationBuilder<T, TProperty>;
            }

            return newBeginingBuilder;
        }
    }
}
