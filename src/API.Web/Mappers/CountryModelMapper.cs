using System.Globalization;
using Fossa.Bridge.Models.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class CountryModelMapper : IMapper<RegionInfo, CountryModel>
{
  public CountryModel Map(RegionInfo source)
  {
    return new CountryModel(source.EnglishName, source.TwoLetterISORegionName);
  }
}
