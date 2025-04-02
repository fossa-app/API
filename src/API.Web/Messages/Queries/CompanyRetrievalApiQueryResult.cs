namespace Fossa.API.Web.Messages.Queries;

public record CompanyRetrievalApiQueryResult(
  long Id,
  string Name,
  string CountryCode);
