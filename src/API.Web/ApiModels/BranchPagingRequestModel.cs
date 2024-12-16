namespace Fossa.API.Web.ApiModels;

public record BranchPagingRequestModel(
  string? Search,
  int? PageNumber,
  int? PageSize);
