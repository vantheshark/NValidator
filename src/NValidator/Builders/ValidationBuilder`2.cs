using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NValidator.Validators;

namespace NValidator.Builders
{
    public class ValidationBuilder<T, TProperty> : IValidationBuilder<T, TProperty>
    {
        public IValidationBuilder<T> Next { get; set; }
        public IValidationBuilder<T> Previous { get; protected set; }
        public bool StopChainOnError { get; set; }
        public Expression<Func<T, TProperty>> Expression { get; set; }
        public IValidator Validator { get; set; }
        public string ContainerName { get; protected set; }
        public Action<IValidationBuilder<T>, ValidationContext> BeforeValidation { get; set; }
        public Func<IValidationBuilder<T>, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>> AfterValidation { get; set; }

        public ValidationBuilder(Expression<Func<T, TProperty>> expression)
        {
            Expression = expression;
        }

        public void UpdateContainerName(string containerName)
        {
            ContainerName = containerName;
        }

        protected virtual object GetObjectToValidate(T value)
        {
            try
            {
                return Expression.Compile()(value);
            }
            catch
            {
                return null;
            }
        }

        public virtual string GetChainName()
        {
            var ex = Expression;
            if (ex.NodeType == ExpressionType.Lambda &&
               (ex.Body.NodeType == ExpressionType.MemberAccess || ex.Body.NodeType == ExpressionType.ArrayIndex))
            {
                var parameter = ex.Parameters[0].ToString();
                var result = ex.Body.ToString().Remove(0, parameter.Length + 1);
                return string.Format("{0}.{1}", ContainerName ?? typeof(T).Name, result);
            }
            return null;
        }
        
        public IValidationBuilder<T, TProperty> ToBuilder()
        {
            return this;
        }

        public IValidationBuilder<T, TProperty> SetValidator<TValidator>() where TValidator : class, IValidator<TProperty>
        {
            Validator = new LazyValidator<TValidator, TProperty>();
            return (IValidationBuilder<T, TProperty>)Clone();
        }

        public virtual IValidationBuilder<T, TProperty> SetValidator(IValidator<TProperty> validator)
        {
            Validator = validator;
            return (IValidationBuilder<T, TProperty>)Clone();
        }

        public IEnumerable<ValidationResult> Validate(T containerObject, ValidationContext validationContext)
        {
            if (BeforeValidation != null)
            {
                BeforeValidation(this, validationContext);
            }

            var propertyChain = GetChainName() ?? ContainerName;

            if (Validator == null || validationContext.ShouldIgnore(propertyChain))
            {
                yield break;
            }

            // NOTE: THe validators can be cached so we need to update their's container name before validation
            Validator.TryUpdateContainerName(propertyChain);

            var value = GetObjectToValidate(containerObject);
            var results = Validator.GetValidationResult(value, validationContext);

            if (AfterValidation != null)
            {
                results = AfterValidation(this, results);
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

        protected virtual ValidationResult FormatValidationResult(ValidationResult result, string propertyChain)
        {
            result.MemberName = result.MemberName ?? propertyChain;
            if (string.IsNullOrEmpty(result.PropertyName) && result.MemberName != null && result.MemberName.Contains("."))
            {
                result.PropertyName = result.MemberName.Substring(result.MemberName.LastIndexOf(".") + 1, result.MemberName.Length - result.MemberName.LastIndexOf(".") - 1);
            }
            result.Message = result.Message.Replace("@PropertyName", result.PropertyName);
            return result;
        }

        public virtual IValidationBuilder<T> SetValidator(IValidator validator)
        {
            Validator = validator;
            return (IValidationBuilder<T, TProperty>)Clone();
        }

        public virtual object Clone()
        {
            var builder = new ValidationBuilder<T, TProperty>(Expression)
            {
                ContainerName = ContainerName,
                BeforeValidation = BeforeValidation,
                AfterValidation = AfterValidation,
                StopChainOnError = StopChainOnError,
                Previous = this,
                Validator = ValidatorFactory.NullValidator,
                Next = null
            };

            Next = builder;
            return builder;
        }
    }
}