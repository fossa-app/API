namespace Fossa.API.Web.ApiModels;

public record AddressModel(
  string? Line1,
  string? Line2,
  string? City,
  string? Subdivision,
  string? PostalCode,
  string? CountryCode);
