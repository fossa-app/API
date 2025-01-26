using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Globalization;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class AddressModelMapper :
  IMapper<Address, AddressModel>,
  IMapper<AddressModel, Address>
{
  private readonly IRegionFactory _regionFactory;

  public AddressModelMapper(
    IRegionFactory regionFactory)
  {
    _regionFactory = regionFactory ?? throw new ArgumentNullException(nameof(regionFactory));
  }

  public AddressModel Map(Address source)
  {
    return new AddressModel(
      source.Line1.Trim(),
      source.Line2.MatchUnsafe(x => x.Trim(), () => null),
      source.City.Trim(),
      source.Subdivision.Trim(),
      source.PostalCode.Trim(),
      source.Country.TwoLetterISORegionName);
  }

  public Address Map(AddressModel source)
  {
    return new Address(
      source.Line1 ?? string.Empty,
      Optional(source.Line2),
      source.City ?? string.Empty,
      source.Subdivision ?? string.Empty,
      source.PostalCode ?? string.Empty,
      _regionFactory.Create(source.CountryCode ?? string.Empty));
  }
}
