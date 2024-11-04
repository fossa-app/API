namespace Fossa.API.Web.ApiModels;

public record SystemEntitlementsModel(
  string EnvironmentName,
  string EnvironmentKind,
  IReadOnlyList<CountryModel> Countries,
  int MaximumCompanyCount);
