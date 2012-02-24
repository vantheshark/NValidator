
namespace NValidator.Test.Models
{
    public class OrderDetail
    {
        public double Price { get; set; }
        public string ProductCode { get; set; }
        public int Amount { get; set; }
    }

    public class OrderDetailValidator : TypeValidator<OrderDetail>
    {
        public OrderDetailValidator()
        {
            Logger.Debug("Creating rules for OrderDetail Validator");
            RuleFor(x => x.ProductCode).Not().Null().NotEmpty().Length(3, 5);
        }
    }
}
