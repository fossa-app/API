namespace Fossa.API.Web.ApiModels;

public record CompanySettingsRetrievalModel(
  long Id,
  long CompanyId,
  string ColorSchemeId);
