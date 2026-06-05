using Fossa.API.Core.Entities;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Globalization;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class AddressMongoMapper :
  IMapper<Address, AddressMongo>,
  IMapper<AddressMongo, Address>
{
  private readonly ICountryFactory _countryFactory;

  public AddressMongoMapper(
    ICountryFactory countryFactory)
  {
    _countryFactory = countryFactory ?? throw new ArgumentNullException(nameof(countryFactory));
  }

  public AddressMongo Map(Address source)
  {
    return new AddressMongo
    {
      Line1 = source.Line1.Trim(),
      Line2 = source.Line2.MatchUnsafe(x => x.Trim(), () => null),
      City = source.City.Trim(),
      Subdivision = source.Subdivision.Trim(),
      PostalCode = source.PostalCode.Trim(),
      CountryCode = source.Country.PrincipalRegion.TwoLetterISORegionName,
    };
  }

  public Address Map(AddressMongo source)
  {
    return new Address(
      source.Line1 ?? string.Empty,
      Optional(source.Line2),
      source.City ?? string.Empty,
      source.Subdivision ?? string.Empty,
      source.PostalCode ?? string.Empty,
      _countryFactory.Create(source.CountryCode ?? string.Empty));
  }
}
