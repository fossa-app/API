using System.Globalization;

namespace Fossa.API.Core.Services;

public interface ICountryProvider
{
  RegionInfo GetCountry(string countryCode);
}
