using Fossa.Bridge.Models.ApiModels;
using TIKSN.Globalization;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class CountryModelMapper : IMapper<CountryInfo, CountryModel>
{
  public CountryModel Map(CountryInfo source)
  {
    return new CountryModel(source.PrincipalRegion.EnglishName, source.PrincipalRegion.TwoLetterISORegionName);
  }
}
