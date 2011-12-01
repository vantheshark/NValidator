
namespace NValidator.Builders
{
    public interface IPreInitFluentValidationBuilder<T, TProperty>
    {
        IValidationBuilder<T, TProperty> ToBuilder();
    }

    public interface IFluentValidationBuilder<T, TProperty> : IPreInitFluentValidationBuilder<T, TProperty>
    {
        IValidationBuilder<T, TProperty> SetValidator<TValidator>() where TValidator : class, IValidator<TProperty>;
        IValidationBuilder<T, TProperty> SetValidator(IValidator<TProperty> validator);
    }

    public interface IPostInitFluentValidationBuilder<T, TProperty> : IFluentValidationBuilder<T, TProperty>
    { }
}
