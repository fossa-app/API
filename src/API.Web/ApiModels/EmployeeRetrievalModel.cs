namespace Fossa.API.Web.ApiModels;

public record EmployeeRetrievalModel(
  long Id, long CompanyId,
  long? AssignedBranchId, long? AssignedDepartmentId,
  long? ReportsToId,
  string FirstName, string LastName, string FullName);
