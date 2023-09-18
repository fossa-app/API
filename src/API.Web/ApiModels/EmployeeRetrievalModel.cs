namespace Fossa.API.Web.ApiModels;

public record EmployeeRetrievalModel(
  long Id, long CompanyId,
  string FirstName, string LastName, string FullName);
