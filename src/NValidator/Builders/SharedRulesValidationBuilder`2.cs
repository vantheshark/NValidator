using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NValidator.Builders
{
    internal class SharedRulesValidationBuilder<T, TProperty> : ValidationBuilder<T, TProperty>
    {
        private readonly Expression<Func<T, TProperty>>[] _expressions;
        public IValidationBuilder<T, TProperty> InternalBuilder { get; private set; }
        public SharedRulesValidationBuilder(IValidationBuilder<T, TProperty> previousBuilder, Expression<Func<T, TProperty>>[] expressions) :
            base(null)
        {
            BeforeValidation = previousBuilder.BeforeValidation;
            AfterValidation = previousBuilder.AfterValidation;
            previousBuilder.Next = this;
            Previous = previousBuilder;
            Validator = ValidatorFactory.NullValidator;
            ContainerName = previousBuilder.ChainName ?? previousBuilder.ContainerName;

            _expressions = expressions;
            Expression<Func<T, TProperty>> nullExpression = null;
            InternalBuilder = ValidationBuilderHelpers.CreateGenericBuilder(nullExpression, ValidatorFactory.DefaultValidationBuilderType);
            InternalBuilder.BeforeValidation = previousBuilder.BeforeValidation;
            InternalBuilder.AfterValidation = previousBuilder.AfterValidation;
            InternalBuilder.Validator = ValidatorFactory.NullValidator;
            InternalBuilder.UpdateContainerName(ContainerName);

        }

        protected internal override IEnumerable<ValidationResult> GetResults(ValidationContext validationContext, T containerObject, out string propertyChain)
        {
            propertyChain = ChainName ?? ContainerName;
            if (InternalBuilder.Validator == null || validationContext.ShouldIgnore(propertyChain))
            {
                return Enumerable.Empty<ValidationResult>();
            }
            var results = new List<ValidationResult>();
            foreach(var expression in _expressions)
            {
                var builder = InternalBuilder;
                while (builder != null)
                {
                    builder.Expression = expression;
                    var tempResults = builder.Validate(containerObject, validationContext).ToList();
                    tempResults = tempResults.Where(validationResult => !validationContext.ShouldIgnore(validationResult.MemberName)).ToList();
                    results.AddRange(tempResults);
                    if (tempResults.Count > 0 && builder.StopChainOnError)
                    {
                        break;
                    }
                    builder = builder.Next as IValidationBuilder<T, TProperty>;
                }
            }
            return results;
        }

        public override object Clone()
        {
            throw new NotSupportedException();
        }
    }
}