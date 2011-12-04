using System.Collections.Generic;
using System.Linq;

namespace NValidator.Builders
{
    internal class InternalTypeValidator<TItem> : TypeValidator<TItem>
    {
    }

    internal class CompositeValidationBuilder<T, TProperty, TItem> : ValidationBuilder<T, TProperty> where TProperty : IEnumerable<TItem>
    {
        public CompositeValidationBuilder(IValidationBuilder<T, TProperty> previousBuilder) :
            base(previousBuilder.Expression)
        {
            BeforeValidation = previousBuilder.BeforeValidation;
            AfterValidation = previousBuilder.AfterValidation;
            previousBuilder.Next = this;
            Previous = previousBuilder;
            Validator = new InternalTypeValidator<TItem>();
            ContainerName = previousBuilder.ChainName ?? previousBuilder.ContainerName;
        }

        protected internal override IEnumerable<ValidationResult> GetResults(ValidationContext validationContext, T containerObject, out string propertyChain)
        {
            propertyChain = ContainerName;
            if (Validator == null || validationContext.ShouldIgnore(propertyChain))
            {
                return Enumerable.Empty<ValidationResult>();
            }
            var enumerable = (TProperty)GetObjectToValidate(containerObject);
            var results = new List<ValidationResult>();
            var index = 0;
            foreach (TItem item in enumerable)
            {
                var newChainName = string.Format("{0}[{1}]", propertyChain, index++);
                Validator.TryUpdateContainerName(newChainName);
                results.AddRange(Validator.GetValidationResult(item, validationContext));
            }
            return results;
        }
    }
}