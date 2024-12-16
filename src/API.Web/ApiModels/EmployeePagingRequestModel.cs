namespace Fossa.API.Web.ApiModels;

public record EmployeePagingRequestModel(
  string? Search,
  int? PageNumber,
  int? PageSize);
