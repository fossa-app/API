using System.Globalization;
using FluentValidation;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Services;

namespace Fossa.API.Core.Validators;

public class AddressValidator : AbstractValidator<Address>
{
  public AddressValidator(IPostalCodeParser postalCodeParser)
  {
    RuleFor(x => x.Line1).NotEmpty();
    RuleFor(x => x.City).NotEmpty();
    RuleFor(x => x.Subdivision).NotEmpty();
    RuleFor(x => x.PostalCode).NotEmpty();
    RuleFor(x => x.Country).NotEmpty();
    RuleFor(x => x.PostalCode).Must(
      (command, postalCode) => BranchAddressPostalCodeMustBeValidForCounpanyAddressCountry(
        postalCodeParser,
        command.Country,
        postalCode))
        .WithMessage(BranchAddressPostalCodeMustBeValidForCounpanyAddressCountryErrorMessage);
  }

  private static bool BranchAddressPostalCodeMustBeValidForCounpanyAddressCountry(
    IPostalCodeParser postalCodeParser,
    RegionInfo country,
    string postalCode)
  {
    ArgumentNullException.ThrowIfNull(postalCodeParser);

    var parserResult = postalCodeParser.ParsePostalCode(country, postalCode);
    return !parserResult.IsFaulted;
  }

  private static string BranchAddressPostalCodeMustBeValidForCounpanyAddressCountryErrorMessage(
    Address address, string postalCode)
  {
    return $"Postal Code '{postalCode}' for Country '{address.Country.TwoLetterISORegionName} - [{address.Country.EnglishName}]' is invalid.";
  }
}
