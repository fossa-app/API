using Fossa.API.Web.Mappers;

namespace Fossa.API.Web.ApiModels;

public record LicenseResponseModel<TEntitlementsModel>(
  LicenseTermsModel Terms,
  TEntitlementsModel Entitlements);
