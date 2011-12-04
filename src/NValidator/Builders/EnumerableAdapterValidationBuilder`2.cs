using System;
using System.Collections.Generic;
using System.Linq;

namespace NValidator.Builders
{
    internal class EnumerableAdapterValidationBuilder<T, TProperty, TItem> : ValidationBuilder<T, TProperty> where TProperty : IEnumerable<TItem>
    {
        public IValidationBuilder<T, TItem> InternalBuilder { get; private set; }
        public EnumerableAdapterValidationBuilder(IValidationBuilder<T, TProperty> previousBuilder) :
            base(previousBuilder.Expression)
        {
            BeforeValidation = previousBuilder.BeforeValidation;
            AfterValidation = previousBuilder.AfterValidation;
            previousBuilder.Next = this;
            Previous = previousBuilder;
            Validator = new InternalTypeValidator<TItem>();
            ContainerName = previousBuilder.ChainName ?? previousBuilder.ContainerName;

            InternalBuilder = (IValidationBuilder<T, TItem>)ValidationBuilderHelpers.CreateGenericBuilder<T>(typeof(TItem), default(TItem), ValidatorFactory.DefaultValidationBuilderType);
        }

        protected internal override IEnumerable<ValidationResult> GetResults(ValidationContext validationContext, T containerObject, out string propertyChain)
        {
            propertyChain = ContainerName;
            if (InternalBuilder == null || validationContext.ShouldIgnore(propertyChain))
            {
                return Enumerable.Empty<ValidationResult>();
            }

            var enumerable = (TProperty)GetObjectToValidate(containerObject);
            var results = new List<ValidationResult>();
            var index = 0;
            
            foreach (TItem item in enumerable)
            {
                var newChainName = string.Format("{0}[{1}]", propertyChain, index++);
                if (validationContext.ShouldIgnore(newChainName))
                {
                    continue;// while, go to next builder
                }

                Action<IValidationBuilder<T, TItem>> action = b =>
                {
                    b.Expression = x => item;
                    b.UpdateContainerName(newChainName);
                    b.ChainName = newChainName;
                };

                var clonedBuilder = ValidationBuilderHelpers.CloneChain(InternalBuilder, action);
                var builder = clonedBuilder;
                while (builder != null)
                {
                    var tempResults = builder.Validate(containerObject, validationContext).ToList();
                    tempResults = tempResults.Where(validationResult => !validationContext.ShouldIgnore(validationResult.MemberName)).ToList();
                    results.AddRange(tempResults);
                    if (tempResults.Count > 0 && builder.StopChainOnError)
                    {
                        break;
                    }
                    builder = builder.Next as IValidationBuilder<T, TItem>;
                }
            }
            return results;
        }
    }
}