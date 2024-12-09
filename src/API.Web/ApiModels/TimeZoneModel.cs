namespace Fossa.API.Web.ApiModels;

public record TimeZoneModel(
  string Id,
  string Name,
  string CountryCode,
  TimeSpan CurrentOffset);
