namespace Fossa.API.Web.ApiModels;

public record BranchQueryRequestModel(
  IReadOnlyList<long> Id,
  string? Search,
  int? PageNumber,
  int? PageSize);
