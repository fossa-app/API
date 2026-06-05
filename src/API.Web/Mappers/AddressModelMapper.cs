using FluentValidation;
using Fossa.API.Core.Entities;
using Fossa.Bridge.Models.ApiModels;
using TIKSN.Globalization;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class AddressModelMapper :
  IMapper<Address, AddressModel>,
  IMapper<AddressModel, Address>
{
  private readonly ICountryFactory _countryFactory;

  public AddressModelMapper(
    ICountryFactory countryFactory)
  {
    _countryFactory = countryFactory ?? throw new ArgumentNullException(nameof(countryFactory));
  }

  public AddressModel Map(Address source)
  {
    return new AddressModel(
      source.Line1.Trim(),
      source.Line2.MatchUnsafe(x => x.Trim(), () => null),
      source.City.Trim(),
      source.Subdivision.Trim(),
      source.PostalCode.Trim(),
      source.Country.PrincipalRegion.TwoLetterISORegionName);
  }

  public Address Map(AddressModel source)
  {
    return new Address(
      source.Line1 ?? string.Empty,
      Optional(source.Line2),
      source.City ?? string.Empty,
      source.Subdivision ?? string.Empty,
      source.PostalCode ?? string.Empty,
      CreateCountry(source.CountryCode ?? string.Empty));
  }

  private CountryInfo CreateCountry(string countryCode)
  {
    if (_countryFactory.TryCreate(countryCode, out var country))
    {
      return country;
    }

    throw new ValidationException($"Country code '{countryCode}' is invalid.");
  }
}
