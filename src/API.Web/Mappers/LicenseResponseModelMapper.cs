using Fossa.API.Web.ApiModels;
using TIKSN.Licensing;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class LicenseResponseModelMapper<TEntitlements, TEntitlementsModel>
  : IMapper<License<TEntitlements>, LicenseResponseModel<TEntitlementsModel>>
{
  private readonly IMapper<TEntitlements, TEntitlementsModel> _entitlementsMapper;
  private readonly IMapper<LicenseTerms, LicenseTermsModel> _licenseTermMapper;

  public LicenseResponseModelMapper(
    IMapper<TEntitlements, TEntitlementsModel> entitlementsMapper,
    IMapper<LicenseTerms, LicenseTermsModel> licenseTermMapper)
  {
    _entitlementsMapper = entitlementsMapper ?? throw new ArgumentNullException(nameof(entitlementsMapper));
    _licenseTermMapper = licenseTermMapper ?? throw new ArgumentNullException(nameof(licenseTermMapper));
  }

  public LicenseResponseModel<TEntitlementsModel> Map(License<TEntitlements> source)
  {
    return new LicenseResponseModel<TEntitlementsModel>(
      _licenseTermMapper.Map(source.Terms),
      _entitlementsMapper.Map(source.Entitlements));
  }
}
