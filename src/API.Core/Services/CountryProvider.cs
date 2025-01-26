using System.Globalization;
using FluentValidation;
using TIKSN.Globalization;

namespace Fossa.API.Core.Services;

public class CountryProvider : ICountryProvider
{
  private readonly IRegionFactory _regionFactory;

  public CountryProvider(
    IRegionFactory regionFactory)
  {
    _regionFactory = regionFactory ?? throw new ArgumentNullException(nameof(regionFactory));
  }

  public RegionInfo GetCountry(string countryCode)
  {
    try
    {
      return _regionFactory.Create(countryCode);
    }
    catch (ArgumentException ex)
    {
      throw new ValidationException(ex.Message);
    }
  }
}
