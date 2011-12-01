using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NValidator.Builders;

namespace NValidator
{
    /// <summary>
    /// A base class for validate a type and perform deep validation on its properties if the properties have the validator registered
    /// <para>For example, Order has a list of OrderDetails. There are OrderValidator and OrderDetailValidator registered.</para>
    /// <para>If we perform validation on Order using OrderValidator, it will perform validation on those OrderDetails using OrderDetailValidator</para>
    /// </summary>
    public class CompositeValidator<T> : TypeValidator<T>
    {
        internal override protected IEnumerable<ValidationResult> InternalGetValidationResult(T value, ValidationContext validationContext)
        {
            var properties = TypeDescriptor.GetProperties(typeof (T));
            var i = 0;
            foreach (PropertyDescriptor property in properties)
            {
                var enumerable = property.GetValue(value) as IEnumerable;
                if (enumerable != null && typeof(string) != property.PropertyType)
                {
                    foreach (var item in enumerable)
                    {
                        AddValidatorForProperty(item.GetType(), string.Format("{0}.{1}[{2}]", ContainerName ?? typeof(T).Name, property.Name, i++), item);
                    }
                }
                AddValidatorForProperty(property.PropertyType, property.Name, property.GetValue(value));
            }
            return base.InternalGetValidationResult(value, validationContext);
        }

        private void AddValidatorForProperty(Type propertyType, string containerName, object propertyValue)
        {
            if (propertyValue == null)
            {
                return;
            }
            var builder = CreateGenericInternalValidationBuilder(new[] { typeof(T), propertyType }, containerName, propertyValue) as IValidationBuilder<T>;

            if (ValidationBuilders.Any(x => x.GetChainName() == containerName))
            {
                // NOTE: Stop here because there is an existing validator manually set on that property
                return;
            }

            var validator = ValidatorFactory.Current.GetValidatorFor(propertyType);

            if (builder != null && 
                validator != null && 
                ValidatorFactory.Current.IsDefaultValidator(validator.GetType())) /* Make sure current validator factory works as expected */
            {
                // NOTE: Should not update container name here because a validator can be shared across builders
                // Indeed, we should update container name right before perform validation on validator
                //if (valiator is IHaveContainer)
                //{
                //    (valiator as IHaveContainer).UpdateContainerName(containerName);
                //}
                builder.Validator = validator;
                ValidationBuilders.Add(builder);
            }
        }

        private static object CreateGenericInternalValidationBuilder(Type[] types, params object[] constructorParams)
        {
            var internalValidationBuilderType = typeof(InternalValidationBuilder<,>);
            Type generic = internalValidationBuilderType.MakeGenericType(types);
            return Activator.CreateInstance(generic, constructorParams);
        }
    }

    internal class InternalValidationBuilder<TType, TProperty> : ValidationBuilder<TType, TProperty>
    {
        public InternalValidationBuilder(string prefix, object propertyValue)
            : base(x => (TProperty)propertyValue)
        {
            ContainerName = prefix;
        }
    }
}
