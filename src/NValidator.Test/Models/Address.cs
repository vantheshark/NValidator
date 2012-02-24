
namespace NValidator.Test.Models
{
    public class Address
    {
        public string Number { get; set; }
        public string Street { get; set; }
        public string PostCode { get; set; }
        public string Suburb { get; set; }
        public string Country { get; set; }
    }

    public class AddressValidator : TypeValidator<Address>
    {
        public AddressValidator()
        {
            Logger.Debug("Creating rules for Address Validator");
            RuleFor(address => address.Number)
                .Match(@"[\w\d]+");

            RuleFor(address => address.Street)
                .Match(@"[\w\d]{1, 200}");

            RuleFor(address => address.PostCode)
                .StopOnFirstError()
                .Length(4, 5)
                .Match(@"\d]{4, 5}")
                .AllWithMessage(address => string.Format("'{0}' is not an valid postcode.", address.PostCode));

            RuleFor(address => address.Suburb)
                .StopOnFirstError()
                .NotNull()
                .NotEmpty();

            RuleFor(address => address.Country).Must(BeACountry);

        }

        private static bool BeACountry(string country)
        {
            // ..... Code to check country here
            ValidatorFactory.Current.Register<AddressValidator>(true /* Will be resolved in any CompositeValidator*/);
            ValidatorFactory.Current.Register<UserValidator>(); /* UserValidator won't be resolved in any CompositeValidator*/
            return true;
        }
    }
}
