using FluentValidation;
using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Validators;

public class AddressValidator : AbstractValidator<Address>
{
  public AddressValidator()
  {
    RuleFor(x => x.Line1).NotEmpty();
    RuleFor(x => x.City).NotEmpty();
    RuleFor(x => x.Subdivision).NotEmpty();
    RuleFor(x => x.PostalCode).NotEmpty();
    RuleFor(x => x.Country).NotEmpty();
  }
}
