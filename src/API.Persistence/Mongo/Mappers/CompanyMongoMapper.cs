using Fossa.API.Core.Entities;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Globalization;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class CompanyMongoMapper : IMapper<CompanyMongoEntity, CompanyEntity>, IMapper<CompanyEntity, CompanyMongoEntity>
{
  private readonly IMapper<long, CompanyId> _dataIdentityToDomainIdentityMapper;
  private readonly IMapper<CompanyId, long> _domainIdentityToDataIdentityMapper;
  private readonly IRegionFactory _regionFactory;

  public CompanyMongoMapper(
    IMapper<CompanyId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> dataIdentityToDomainIdentityMapper,
    IRegionFactory regionFactory)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ??
                                          throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ??
                                          throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
    _regionFactory = regionFactory ?? throw new ArgumentNullException(nameof(regionFactory));
  }

  public CompanyEntity Map(CompanyMongoEntity source)
  {
    return new CompanyEntity(
      _dataIdentityToDomainIdentityMapper.Map(source.ID),
      source.TenantID,
      source.Name ?? string.Empty,
      _regionFactory.Create(source.CountryCode ?? string.Empty));
  }

  public CompanyMongoEntity Map(CompanyEntity source)
  {
    return new CompanyMongoEntity
    {
      ID = _domainIdentityToDataIdentityMapper.Map(source.ID),
      TenantID = source.TenantID,
      Name = source.Name,
      CountryCode = source.Country.TwoLetterISORegionName,
    };
  }
}
