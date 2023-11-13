namespace Fossa.API.Web.ApiModels;

public record SystemEntitlementsModel(
  string EnvironmentName,
  string EnvironmentKind,
  int MaximumCompanyCount);
