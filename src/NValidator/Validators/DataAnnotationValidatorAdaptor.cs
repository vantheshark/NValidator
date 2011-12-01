using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NValidator.Validators
{
    public class DataAnnotationValidatorAdaptor : IValidator
    {
        private readonly Type _validationAttributeType;
        private ValidationAttribute _validator;
        public virtual ValidationAttribute Validator
        {
            get
            {
                if (_validator == null)
                {
                    _validator = Activator.CreateInstance(_validationAttributeType) as ValidationAttribute;
                }
                return _validator;
            }
        }

        protected DataAnnotationValidatorAdaptor()
        {
        }

        public DataAnnotationValidatorAdaptor(Type validationAttributeType)
        {
            _validationAttributeType = validationAttributeType;
        }

        public bool IsValid(object value)
        {
            return Validator.IsValid(value);
        }

        public bool IsValid(object value, ValidationContext validationContext)
        {
            return Validator.IsValid(value);
        }

        public IEnumerable<ValidationResult> GetValidationResult(object value)
        {
            var result = Validator.GetValidationResult(value, null);
            if (result != null)
            {
                yield return new ValidationResult
                {
                    PropertyName = result.MemberNames != null ? result.MemberNames.FirstOrDefault() : null,
                    Message = result.ErrorMessage
                };
            }
        }

        public IEnumerable<ValidationResult> GetValidationResult(object value, ValidationContext validationContext)
        {
            var result = Validator.GetValidationResult(value, validationContext != null ? new System.ComponentModel.DataAnnotations.ValidationContext(validationContext.ContainerInstance, null, null) : null);
            if (result != null)
            {
                yield return new ValidationResult
                {
                    PropertyName = result.MemberNames != null ? result.MemberNames.FirstOrDefault() : null,
                    Message = result.ErrorMessage
                };
            }
        }
    }
}
