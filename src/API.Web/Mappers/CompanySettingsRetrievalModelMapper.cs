using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class CompanySettingsRetrievalModelMapper : IMapper<CompanySettingsEntity, CompanySettingsRetrievalModel>
{
  private readonly IMapper<CompanySettingsId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IMapper<ColorSchemeId, string> _colorSchemeDomainToDataMapper;

  public CompanySettingsRetrievalModelMapper(
    IMapper<CompanySettingsId, long> domainIdentityToDataIdentityMapper,
    IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper,
    IMapper<ColorSchemeId, string> colorSchemeDomainToDataMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainIdentityToDataIdentityMapper));
    _colorSchemeDomainToDataMapper = colorSchemeDomainToDataMapper ?? throw new ArgumentNullException(nameof(colorSchemeDomainToDataMapper));
  }

  public CompanySettingsRetrievalModel Map(CompanySettingsEntity source)
  {
    return new CompanySettingsRetrievalModel(
      _domainIdentityToDataIdentityMapper.Map(source.ID),
      _companyDomainIdentityToDataIdentityMapper.Map(source.CompanyId),
      _colorSchemeDomainToDataMapper.Map(source.ColorSchemeId));
  }
}
