namespace Fossa.API.Web.ApiModels;

public record EmployeeQueryRequestModel(
  IReadOnlyList<long>? Id,
  string? Search,
  int? PageNumber,
  int? PageSize,
  long? ReportsToId,
  bool? TopLevelOnly);
