![NValidator](http://i42.tinypic.com/wv7ea1.jpg)

NValidator, Release 1.0.1.x (since Feb 12, 2012)
-----------------------------------------------------------------------
* http://nvalidator.codeplex.com/
* https://github.com/vanthoainguyen/NValidator

##1. INTRODUCTION
**NValidator** is a lightweight extensible validation library for .NET that support fluent syntax. The implementation and fluent syntax are inspired from the well-known library _Fluent Validation_. However, I implemented it using _Chain of Reponsibility_ pattern and made it fully support Dependency Injection for validator classes.

Here is a simple example that can help you to start:

```clj
public class UserValidator : TypeValidator<User>
{
	public UserValidator()
	{
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
```	

##2. DOCUMENT
Please checkout the [document](http://nvalidator.codeplex.com/documentation) and [other topics](http://nvalidator.codeplex.com/wikipage?title=Table%20of%20contents) to get started.

The NuGet package [**NValidator**](http://nuget.org/packages/NValidator) is also available.



##3. LICENCE
http://sam.zoy.org/wtfpl/COPYING 
![Troll](http://i40.tinypic.com/2m4vl2x.jpg) 