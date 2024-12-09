namespace Fossa.API.Web.ApiModels;

public record TimeZoneModel(
  string Id,
  string Name,
  CountryModel Country,
  TimeSpan CurrentOffset);
