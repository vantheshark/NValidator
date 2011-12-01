using System;
using System.Collections.Generic;

namespace NValidator.Builders
{
    public sealed class NegativeValidationBuilder<T, TProperty> : ValidationBuilder<T, TProperty>
    {
        public NegativeValidationBuilder(IValidationBuilder<T, TProperty> previousBuilder) :
            base(previousBuilder.Expression)
        {
            BeforeValidation = previousBuilder.BeforeValidation;
            AfterValidation = previousBuilder.AfterValidation;
            ContainerName = previousBuilder.ContainerName;
            StopChainOnError = previousBuilder.StopChainOnError;
            previousBuilder.Next = this;
            Previous = previousBuilder;

            Validator = new NegativeValidator<INegatableValidator<TProperty>, TProperty>();
        }

        public override IValidationBuilder<T> SetValidator(IValidator validator)
        {
            InternalSetValidator(validator);
            return (IValidationBuilder<T>) Clone();
        }

        public override IValidationBuilder<T, TProperty> SetValidator(IValidator<TProperty> validator)
        {
            InternalSetValidator(validator);
            return (IValidationBuilder<T, TProperty>)Clone();
        }

        private void InternalSetValidator(IValidator validator)
        {
            if (Validator is NegativeValidator<INegatableValidator<TProperty>, TProperty>)
            {
                ((NegativeValidator<INegatableValidator<TProperty>, TProperty>)Validator).OriginalValidator = (INegatableValidator<TProperty>)validator;
            }
            else
            {
                throw new Exception("Validator must be a NegativeValidator");
            }
        }

        private class NegativeValidator<TValidator, TPropertyType> : BaseNegatableValidator<TPropertyType>, IHaveContainer where TValidator : class, INegatableValidator<TPropertyType>
        {
            public TValidator OriginalValidator { get; set; }

            public override IEnumerable<ValidationResult> GetErrors(TPropertyType value, ValidationContext validationContext)
            {
                CheckOriginalValidator();
                return (OriginalValidator).GetNegatableErrors(value, validationContext);
            }

            public override IEnumerable<ValidationResult> GetNegatableErrors(TPropertyType value, ValidationContext validationContext)
            {
                CheckOriginalValidator();
                return (OriginalValidator).GetErrors(value, validationContext);
            }

            public override bool IsValid(TPropertyType value, ValidationContext validationContext)
            {
                CheckOriginalValidator();
                return !(OriginalValidator).IsValid(value, validationContext);
            }

            public string ContainerName
            {
                get { return OriginalValidator is IHaveContainer ? (OriginalValidator as IHaveContainer).ContainerName : null; }
            }

            public void UpdateContainerName(string containerName)
            {
                if (OriginalValidator is IHaveContainer)
                {
                    (OriginalValidator as IHaveContainer).UpdateContainerName(containerName);
                }
            }
            private void CheckOriginalValidator()
            {
                if (OriginalValidator == null)
                {
                    throw new Exception("A Negatable validator was not set");
                }
            }
        }
    }
}