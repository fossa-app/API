using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using LanguageExt;
using TIKSN.Data;
using TIKSN.Data.Mongo;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class CompanyRepositoryAdapter
  : MongoRepositoryAdapter<CompanyEntity, CompanyId, CompanyMongoEntity, long>
    , ICompanyRepository, ICompanyQueryRepository
{
  private readonly ICompanyMongoRepository _dataRepository;

  public CompanyRepositoryAdapter(
    IMapper<CompanyEntity, CompanyMongoEntity> domainEntityToDataEntityMapper,
    IMapper<CompanyMongoEntity, CompanyEntity> dataEntityToDomainEntityMapper,
    IMapper<CompanyId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> dataIdentityToDomainIdentityMapper,
    ICompanyMongoRepository dataRepository)
    : base(
      domainEntityToDataEntityMapper,
      dataEntityToDomainEntityMapper,
      domainIdentityToDataIdentityMapper,
      dataIdentityToDomainIdentityMapper,
      dataRepository)
  {
    _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
  }

  public async Task<Option<CompanyEntity>> FindByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
  {
    var entity = await _dataRepository.FindByTenantIdAsync(
      tenantId, cancellationToken).ConfigureAwait(false);

    return entity.Map(Map);
  }

  public async Task<CompanyEntity> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
  {
    var entity = await _dataRepository.GetByTenantIdAsync(
      tenantId, cancellationToken).ConfigureAwait(false);

    return entity is null ? throw new EntityNotFoundException() : Map(entity);
  }

  public Task<int> CountAllAsync(CancellationToken cancellationToken)
  {
    return _dataRepository.CountAllAsync(cancellationToken);
  }
}
