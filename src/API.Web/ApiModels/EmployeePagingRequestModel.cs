namespace Fossa.API.Web.ApiModels;

public record EmployeePagingRequestModel(
  int? PageNumber, int? PageSize);
