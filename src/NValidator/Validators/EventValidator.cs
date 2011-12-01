using System;
using System.Collections.Generic;

namespace NValidator.Validators
{
    public sealed class EventValidator<T> : BaseValidator<T>, INegatableValidator<T>, IHaveContainer
    {
        private readonly IValidator _originalValidator;
        public Action<object, ValidationContext> BeforeValidation { get; set; }
        public Func<IEnumerable<ValidationResult>, IEnumerable<ValidationResult>> AfterValidation { get; set; }

        public EventValidator(IValidator originalValidator)
        {
            _originalValidator = originalValidator;
        }

        public override IEnumerable<ValidationResult> GetValidationResult(T value, ValidationContext validationContext)
        {
            if (BeforeValidation != null)
            {
                BeforeValidation(value, validationContext);
            }
            var results = _originalValidator.GetValidationResult(value, validationContext);

            if (AfterValidation != null)
            {
                return AfterValidation(results);
            }
            return results;
        }

        public IEnumerable<ValidationResult> GetErrors(T value, ValidationContext validationContext)
        {
            if (_originalValidator is INegatableValidator<T>)
            {
                return ((INegatableValidator<T>)_originalValidator).GetErrors(value, validationContext);
            }
            throw new Exception(string.Format("{0} is not INegatableValidator", _originalValidator.GetType().Name));
        }

        public IEnumerable<ValidationResult> GetNegatableErrors(T value, ValidationContext validationContext)
        {
            if (_originalValidator is INegatableValidator<T>)
            {
                return ((INegatableValidator<T>)_originalValidator).GetNegatableErrors(value, validationContext);
            }
            throw new Exception(string.Format("{0} is not INegatableValidator", _originalValidator.GetType().Name));
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
