using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data;
using TIKSN.Data.Mongo;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class CompanySettingsRepositoryAdapter
  : MongoRepositoryAdapter<CompanySettingsEntity, CompanySettingsId, CompanySettingsMongoEntity, long>
    , ICompanySettingsRepository, ICompanySettingsQueryRepository
{
  private readonly ICompanySettingsMongoRepository _dataRepository;
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;

  public CompanySettingsRepositoryAdapter(
    IMapper<CompanySettingsEntity, CompanySettingsMongoEntity> domainEntityToDataEntityMapper,
    IMapper<CompanySettingsMongoEntity, CompanySettingsEntity> dataEntityToDomainEntityMapper,
    IMapper<CompanySettingsId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanySettingsId> dataIdentityToDomainIdentityMapper,
    ICompanySettingsMongoRepository dataRepository,
    IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper) : base(
    domainEntityToDataEntityMapper,
    dataEntityToDomainEntityMapper,
    domainIdentityToDataIdentityMapper,
    dataIdentityToDomainIdentityMapper,
    dataRepository)
  {
    _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainIdentityToDataIdentityMapper));
  }

  public async Task<Option<CompanySettingsEntity>> FindByCompanyIdAsync(
    CompanyId companyId,
    CancellationToken cancellationToken)
  {
    var dataCompanyId = _companyDomainIdentityToDataIdentityMapper.Map(companyId);
    var entity = await _dataRepository.FindByCompanyIdAsync(
      dataCompanyId, cancellationToken).ConfigureAwait(false);

    return entity.Map(Map);
  }

  public async Task<CompanySettingsEntity> GetByCompanyIdAsync(
    CompanyId companyId,
    CancellationToken cancellationToken)
  {
    var dataCompanyId = _companyDomainIdentityToDataIdentityMapper.Map(companyId);
    var entity = await _dataRepository.GetByCompanyIdAsync(
      dataCompanyId, cancellationToken).ConfigureAwait(false);

    return entity is null ? throw new EntityNotFoundException() : Map(entity);
  }
}
