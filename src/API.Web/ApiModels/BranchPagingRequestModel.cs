namespace Fossa.API.Web.ApiModels;

public record BranchPagingRequestModel(
  int? PageNumber, int? PageSize);
