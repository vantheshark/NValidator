
namespace NValidator.Builders
{
    public interface IChainOfValidationBuilder<T>
    {
        IValidationBuilder<T> Next { get; set; }
        IValidationBuilder<T> Previous { get; }
        bool StopChainOnError { get; set; }
        string GetChainName();
    }
}
