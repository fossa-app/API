using Fossa.API.Core.Entities;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class CompanySettingsMongoMapper : IMapper<CompanySettingsMongoEntity, CompanySettingsEntity>, IMapper<CompanySettingsEntity, CompanySettingsMongoEntity>
{
  private readonly IMapper<long, CompanySettingsId> _dataIdentityToDomainIdentityMapper;
  private readonly IMapper<CompanySettingsId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<long, CompanyId> _companyDataIdentityToDomainIdentityMapper;
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IMapper<string, ColorSchemeId> _colorSchemeDataToDomainMapper;
  private readonly IMapper<ColorSchemeId, string> _colorSchemeDomainToDataMapper;

  public CompanySettingsMongoMapper(
    IMapper<long, CompanySettingsId> dataIdentityToDomainIdentityMapper,
    IMapper<CompanySettingsId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> companyDataIdentityToDomainIdentityMapper,
    IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper,
    IMapper<string, ColorSchemeId> colorSchemeDataToDomainMapper,
    IMapper<ColorSchemeId, string> colorSchemeDomainToDataMapper)
  {
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _companyDataIdentityToDomainIdentityMapper = companyDataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(companyDataIdentityToDomainIdentityMapper));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainIdentityToDataIdentityMapper));
    _colorSchemeDataToDomainMapper = colorSchemeDataToDomainMapper ?? throw new ArgumentNullException(nameof(colorSchemeDataToDomainMapper));
    _colorSchemeDomainToDataMapper = colorSchemeDomainToDataMapper ?? throw new ArgumentNullException(nameof(colorSchemeDomainToDataMapper));
  }

  public CompanySettingsEntity Map(CompanySettingsMongoEntity source)
  {
    return new CompanySettingsEntity(
      _dataIdentityToDomainIdentityMapper.Map(source.ID),
      _companyDataIdentityToDomainIdentityMapper.Map(source.CompanyId),
      _colorSchemeDataToDomainMapper.Map(source.ColorSchemeId ?? string.Empty));
  }

  public CompanySettingsMongoEntity Map(CompanySettingsEntity source)
  {
    return new CompanySettingsMongoEntity
    {
      ID = _domainIdentityToDataIdentityMapper.Map(source.ID),
      CompanyId = _companyDomainIdentityToDataIdentityMapper.Map(source.CompanyId),
      ColorSchemeId = _colorSchemeDomainToDataMapper.Map(source.ColorSchemeId),
    };
  }
}
