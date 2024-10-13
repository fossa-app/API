namespace Fossa.API.Web.ApiModels;

public record CompanyEntitlementsModel(
  long CompanyId,
  int MaximumBranchCount,
  int MaximumEmployeeCount);
