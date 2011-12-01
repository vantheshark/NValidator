using System;
using System.Collections.Generic;
using System.Linq;

namespace NValidator.Validators
{
    public class ConditionalValidator<T, TProperty> : BaseValidator<TProperty>, IHaveContainer
    {
        private readonly IValidator _originalValidator;
        private readonly Func<T, bool> _condition;

        public ConditionalValidator(IValidator originalValidator, Func<T, bool> condition)
        {
            _originalValidator = originalValidator;
            _condition = condition;
        }

        public override IEnumerable<ValidationResult> GetValidationResult(TProperty value, ValidationContext validationContext)
        {
            if (_condition((T)validationContext.ContainerInstance))
            {
                return _originalValidator.GetValidationResult(value, validationContext);
            }
            return Enumerable.Empty<ValidationResult>();
        }

        public string ContainerName
        {
            get { return _originalValidator.TryGetContainerName(); }
        }

        public void UpdateContainerName(string containerName)
        {
            _originalValidator.TryUpdateContainerName(containerName);
        }
    }
}
