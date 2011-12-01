using System.Collections.Generic;

namespace NValidator.Validators
{
    internal class LazyValidator<TValidator, TProperty> : BaseValidator<TProperty>, IHaveContainer where TValidator : class, IValidator<TProperty>
    {
        public override IEnumerable<ValidationResult> GetValidationResult(TProperty value, ValidationContext validationContext)
        {
            var validator = ValidatorFactory.Current.Resolve<TValidator>();
            validator.TryUpdateContainerName(ContainerName);
            return validator.GetValidationResult(value, validationContext);
        }

        public string ContainerName { get; private set; }

        public void UpdateContainerName(string containerName)
        {
            ContainerName = containerName;
        }
    }
}
