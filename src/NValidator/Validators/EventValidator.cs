using System;
using System.Collections.Generic;

namespace NValidator.Validators
{
    public class EventValidator<T> : BaseValidator<T>, INegatableValidator<T>, IHaveContainer
    {
        protected readonly IValidator OriginalValidator;
        public Action<T, ValidationContext> BeforeValidation { get; set; }
        public Func<T, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>> AfterValidation { get; set; }

        public EventValidator(IValidator originalValidator)
        {
            if (originalValidator == null)
            {
                throw new ArgumentNullException("originalValidator");
            }
            OriginalValidator = originalValidator;
        }

        public override IEnumerable<ValidationResult> GetValidationResult(T value, ValidationContext validationContext)
        {
            if (BeforeValidation != null)
            {
                BeforeValidation(value, validationContext);
            }
            var results = OriginalValidator.GetValidationResult(value, validationContext);

            if (AfterValidation != null)
            {
                return AfterValidation(value, results);
            }
            return results;
        }

        public IEnumerable<ValidationResult> GetErrors(T value, ValidationContext validationContext)
        {
            if (OriginalValidator is INegatableValidator<T>)
            {
                return ((INegatableValidator<T>)OriginalValidator).GetErrors(value, validationContext);
            }
            throw new Exception(string.Format("{0} is not INegatableValidator", OriginalValidator.GetType().Name));
        }

        public IEnumerable<ValidationResult> GetNegatableErrors(T value, ValidationContext validationContext)
        {
            if (OriginalValidator is INegatableValidator<T>)
            {
                return ((INegatableValidator<T>)OriginalValidator).GetNegatableErrors(value, validationContext);
            }
            throw new Exception(string.Format("{0} is not INegatableValidator", OriginalValidator.GetType().Name));
        }

        public string ContainerName
        {
            get { return OriginalValidator.TryGetContainerName(); }
        }

        public void UpdateContainerName(string containerName)
        {
            OriginalValidator.TryUpdateContainerName(containerName);
        }
    }

    public class EventValidator<T, TProperty> : EventValidator<TProperty> where T : class 
    {
        public new Action<T, TProperty, ValidationContext> BeforeValidation { get; set; }
        public new Func<T, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>> AfterValidation { get; set; }

        public EventValidator(IValidator originalValidator) : base(originalValidator)
        {
        }

        public override IEnumerable<ValidationResult> GetValidationResult(TProperty value, ValidationContext validationContext)
        {
            if (BeforeValidation != null)
            {
                BeforeValidation(validationContext.ContainerInstance as T, value, validationContext);
            }
            var results = OriginalValidator.GetValidationResult(value, validationContext);

            if (AfterValidation != null)
            {
                return AfterValidation(validationContext.ContainerInstance as T, results);
            }
            return results;
        }
    }
}
