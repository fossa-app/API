using Fossa.API.Core;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data.Mongo;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class CompanyRepositoryAdapter
       : MongoRepositoryAdapter<CompanyEntity, long, CompanyMongoEntity, long>
       , ICompanyRepository, ICompanyQueryRepository
{
  private readonly ICompanyMongoRepository _dataRepository;

  public CompanyRepositoryAdapter(
    IMapper<CompanyEntity, CompanyMongoEntity> domainEntityToDataEntityMapper,
    IMapper<CompanyMongoEntity, CompanyEntity> dataEntityToDomainEntityMapper,
    ICompanyMongoRepository dataRepository) : base(
      domainEntityToDataEntityMapper,
      dataEntityToDomainEntityMapper,
      IdentityMapper<long>.Instance,
      IdentityMapper<long>.Instance,
      dataRepository)
  {
    _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
  }

  public async Task<CompanyEntity> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
  {
    var entity = await _dataRepository.GetByTenantIdAsync(
      tenantId, cancellationToken).ConfigureAwait(false);

    return entity == null ? throw new EntityNotFoundException() : Map(entity);
  }
}
