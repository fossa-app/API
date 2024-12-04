namespace Fossa.API.Web.ApiModels;

public record CompanyRetrievalModel(
  long Id,
  string Name,
  CountryModel Country);
