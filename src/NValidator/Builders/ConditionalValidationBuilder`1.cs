using System;
using System.Collections.Generic;

namespace NValidator.Builders
{
    /// <summary>
    /// This is an implementation of IValidationBuilder<T> that contains a set of validation builders which will be used
    /// to validate only if the provided condition in the constructor is matched.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ConditionalValidationBuilder<T> : IValidationBuilder<T>
    {
        public List<IValidationBuilder<T>> InternalBuilders { get; set; }

        public IValidationBuilder<T> Next { get; set; }

        public IValidationBuilder<T> Previous { get; set; }

        public bool StopChainOnError { get; set; }

        public string ChainName { get; set; }

        public string ContainerName { get; private set; }

        public IValidator Validator { 
            get { return ValidatorFactory.NullValidator; } 
            set { }
        }

        public Action<IValidationBuilder<T>, ValidationContext> BeforeValidation { get; set; }

        public Func<IValidationBuilder<T>, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>> AfterValidation { get; set; }

        private readonly Func<T, bool> _condition;
        public ConditionalValidationBuilder(Func<T, bool> condition)
        {
            _condition = condition;
            InternalBuilders = new List<IValidationBuilder<T>>();
        }

        public void UpdateContainerName(string containerName)
        {
            ContainerName = containerName;
        }

        public IValidationBuilder<T> SetValidator(IValidator validator)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<ValidationResult> Validate(T container, ValidationContext validationContext)
        {
            if (BeforeValidation != null)
            {
                BeforeValidation(this, validationContext);
            }

            var propertyChain = ChainName ?? ContainerName;
            var results = new List<ValidationResult>();
            if (_condition != null && _condition(container))
            {
                foreach (var builder in InternalBuilders)
                {
                    builder.UpdateContainerName(propertyChain);
                    results.AddRange(builder.Validate(container, validationContext));
                }
            }

            if (AfterValidation != null)
            {
                results = new List<ValidationResult>(AfterValidation(this, results));
            }

            return results;
        }
    }
}