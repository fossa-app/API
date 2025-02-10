namespace Fossa.API.Web.ApiModels;

public record EmployeeRetrievalModel(
  long Id, long CompanyId, long? AssignedBranchId,
  string FirstName, string LastName, string FullName);
