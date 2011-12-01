using System;
using System.Collections.Generic;

namespace NValidator.Builders
{
    public interface IValidationBuilder<T> : IChainOfValidationBuilder<T>, IHaveContainer
    {
        IValidator Validator { get; set; }
        IValidationBuilder<T> SetValidator(IValidator validator);
        IEnumerable<ValidationResult> Validate(T container, ValidationContext validationContext);

        Action<IValidationBuilder<T>, ValidationContext> BeforeValidation { get; set; }
        Func<IValidationBuilder<T>, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>> AfterValidation { get; set; }
    }
}
