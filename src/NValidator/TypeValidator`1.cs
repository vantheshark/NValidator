using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NValidator.Builders;

namespace NValidator
{
    /// <summary>
    /// A base class for validating a type without performing deep validation on its properties even though the properties have validators registered
    /// <para>For example, Order has a list of OrderDetails. There are OrderValidator and OrderDetailValidator registered.</para>
    /// <para>If we perform validation on Order using OrderValidator, it will not perform validation on those OrderDetails using their OrderDetailValidators</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TypeValidator<T> : BaseValidator<T>, IHaveContainer
    {
        public string ContainerName { get; protected set; }
        /// <summary>
        /// Gets or sets the default type of the validation builder.
        /// The default type can be set via ValidatorFactory.DefaultValidationBuilderType, default value is typeof(ValidationBuilder&lt;,&gt;) or set in the constructor
        /// </summary>
        /// <value>
        /// The default type of the builder.
        /// </value>
        public Type DefaultBuilderType { get; set; }

        protected List<IValidationBuilder<T>> ValidationBuilders { get; set; }

        /// <summary>
        /// Gets or sets the contextual validation builders.
        /// This is the list of builders that can be added to the predefined ValidationBuilders base on the validation context.
        /// <para>This list is used to keep track of new added builders to ValidationBuilders and reset the original ValidationBuilders to the previous state after validation</para>
        /// </summary>
        /// <value>
        /// The contextual validation builders.
        /// </value>
        protected List<IValidationBuilder<T>> ContextualValidationBuilders { get; set; }

        protected TypeValidator()
        {
            ValidationBuilders = new List<IValidationBuilder<T>>();
            DefaultBuilderType = ValidatorFactory.DefaultValidationBuilderType;
        }

        public void UpdateContainerName(string containerName)
        {
            ContainerName = containerName;
            foreach (var v in ValidationBuilders)
            {
                if (v != null)
                {
                    v.UpdateContainerName(containerName);
                }
            }
        }

        /// <summary>
        /// Create rules for property of the same type
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expressions">The expressions.</param>
        /// <returns></returns>
        public IFluentValidationBuilder<T, TProperty> RulesFor<TProperty>(params Expression<Func<T, TProperty>>[] expressions)
        {
            Expression<Func<T, TProperty>> nullExpression = null;
            var dummyBuilder = CreateGenericBuilder(nullExpression);
            dummyBuilder.UpdateContainerName(ContainerName);
            var sharedRulesBuilder = new SharedRulesValidationBuilder<T, TProperty>(dummyBuilder, expressions);
            ValidationBuilders.Add(sharedRulesBuilder);
            return sharedRulesBuilder.InternalBuilder;
        }

        public IFluentValidationBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var newBuilder = CreateGenericBuilder(expression);
            newBuilder.UpdateContainerName(ContainerName);
            ValidationBuilders.Add(newBuilder);
            return newBuilder;
        }

        public void When(Func<T, bool> condition, Action ruleAction)
        {
            var originalBuilders = new List<IValidationBuilder<T>>(ValidationBuilders);
            ruleAction();
            var setOfValidationBuilders = new ConditionalValidationBuilder<T>(condition)
            {
                InternalBuilders = ValidationBuilders.Except(originalBuilders).ToList()
            };

            ValidationBuilders.Add(setOfValidationBuilders);
            ValidationBuilders.RemoveAll(setOfValidationBuilders.InternalBuilders.Contains);
        }

        public IFluentValidationBuilder<T, TItem> RuleForEach<TItem>(Expression<Func<T, IEnumerable<TItem>>> expression)
        {
            var dummyBuilder = CreateGenericBuilder(expression);
            dummyBuilder.UpdateContainerName(ContainerName);
            var compositeBuilder = new EnumerableAdapterValidationBuilder<T, IEnumerable<TItem>, TItem>(dummyBuilder);
            ValidationBuilders.Add(compositeBuilder);
            return compositeBuilder.InternalBuilder;
        }

        public sealed override IEnumerable<ValidationResult> GetValidationResult(T value, ValidationContext validationContext)
        {
            BuildContextualValidationBuilders(value);
            var results = InternalGetValidationResult(value, validationContext).ToList();
            ValidationBuilders.RemoveAll(x => ContextualValidationBuilders.Contains(x));
            ContextualValidationBuilders.Clear();
            return results;
        }

        protected virtual void BuildContextualValidationBuilders(T value)
        {
            if (ContextualValidationBuilders == null)
            {
                ContextualValidationBuilders = new List<IValidationBuilder<T>>();
            }
        }

        internal virtual protected IEnumerable<ValidationResult> InternalGetValidationResult(T value, ValidationContext validationContext)
        {
            return ValidationBuilders.GetValidationResults(value, ContainerName, validationContext);
        }

        protected virtual IValidationBuilder<T, TProperty> CreateGenericBuilder<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return ValidationBuilderHelpers.CreateGenericBuilder(expression, DefaultBuilderType);
        }
    }
}
