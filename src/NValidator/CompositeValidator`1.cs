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
                AddValidatorForProperty(property.PropertyType, string.Format("{0}.{1}", ContainerName ?? typeof(T).Name, property.Name), property.GetValue(value));
            }
            return base.InternalGetValidationResult(value, validationContext);
        }

        private void AddValidatorForProperty(Type propertyType, string containerName, object propertyValue)
        {
            if (propertyValue == null)
            {
                return;
            }
            var builder = CreateGenericBuilder(propertyType, propertyValue);

            if (ValidationBuilders.Any(x => x.ChainName == containerName))
            {
                // NOTE: Stop here because there is an existing validator manually set on that property
                return;
            }

            var validator = ValidatorFactory.Current.GetValidatorFor(propertyType);

            if (builder != null && validator != null && 
                ValidatorFactory.Current.IsDefaultValidator(validator.GetType())) /* Make sure current validator factory works as expected */
            {
                builder.UpdateContainerName(containerName);
                builder.Validator = validator;
                ValidationBuilders.Add(builder);
            }
        }

        protected virtual IValidationBuilder<T> CreateGenericBuilder(Type propertyType, object propertyValue)
        {
            return ValidationBuilderHelpers.CreateGenericBuilder<T>(propertyType, propertyValue, DefaultBuilderType);
        }
    }
}
