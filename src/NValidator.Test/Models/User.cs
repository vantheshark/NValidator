using System.Linq;

namespace NValidator.Test.Models
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public Address Address { get; set; }
    }

    public class UserValidator : TypeValidator<User>
    {
        public UserValidator()
        {
            Logger.Debug("Creating rules for OrderDetail Validator");

            RuleFor(user => user.Id)
                .NotEmpty()
                .NotNull();

            RuleFor(user => user.UserName)
                .NotNull()
                .Length(6, 10)
                .Must(x => !x.Contains(" ")).WithMessage("@PropertyName cannot contain empty string.");

            RuleFor(user => user.Password)
                .StopOnFirstError()
                .NotEmpty()
                .Length(6, 20)
                .Must(x => x.Any(c => c >= 'a' && c <= 'z') &&
                           x.Any(c => c >= '0' && c <= '9') &&
                           x.Any(c => c >= 'A' && c <= 'Z')).WithMessage("@PropertyName must contain both letter and digit.");

            RuleFor(user => user.Title)
                .In("MR", "MS", "MRS", "DR", "PROF", "REV", "OTHER");

            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("Please specify the first name.");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Please specify the last name.");
            
            RuleFor(user => user.Email)
                .Email();

            RuleFor(user => user.Address)
                .SetValidator<AddressValidator>()
                .When(x => x != null);
        }
    }
}
