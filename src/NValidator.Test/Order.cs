
namespace NValidator.Test
{
    public class Order
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public OrderDetail[] OrderDetails { get; set; }
        public OrderDetail FirstOrderDetail { get; set; }
    }

    public class OrderValidator : TypeValidator<Order>
    {
        public OrderValidator()
        {
            // NOTE: In order to use OrderDetailValidator, we must register it using ValidatorFactory.Current.Register<OrderDetailValidator>();
            RuleFor(x => x.FirstOrderDetail).SetValidator<OrderDetailValidator>().When(x => x.FirstOrderDetail != null);
        }
    }

    public class CompositeOrderValidator : CompositeValidator<Order>
    {
    }
}
