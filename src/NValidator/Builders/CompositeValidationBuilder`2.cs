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

        protected internal override IEnumerable<ValidationResult> InternalValidate(T containerObject, ValidationContext validationContext)
        {
            if (BeforeValidation != null)
            {
                BeforeValidation(this, validationContext);
            }

            var propertyChain = ContainerName;
            if (Validator == null || validationContext.ShouldIgnore(propertyChain))
            {
                yield break;
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

            if (AfterValidation != null)
            {
                results = AfterValidation(this, results).ToList();
            }

            foreach (var modelValidationResult in results)
            {
                if (validationContext.ShouldIgnore(modelValidationResult.MemberName))
                {
                    continue; // foreach
                }
                yield return FormatValidationResult(modelValidationResult, propertyChain);
            }
        }
    }
}