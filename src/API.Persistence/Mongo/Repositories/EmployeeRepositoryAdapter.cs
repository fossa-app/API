using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using LanguageExt;
using TIKSN.Data;
using TIKSN.Data.Mongo;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class EmployeeRepositoryAdapter
  : MongoRepositoryAdapter<EmployeeEntity, EmployeeId, EmployeeMongoEntity, long>
    , IEmployeeRepository, IEmployeeQueryRepository
{
  private readonly IEmployeeMongoRepository _dataRepository;

  public EmployeeRepositoryAdapter(
    IMapper<EmployeeEntity, EmployeeMongoEntity> domainEntityToDataEntityMapper,
    IMapper<EmployeeMongoEntity, EmployeeEntity> dataEntityToDomainEntityMapper,
    IMapper<EmployeeId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, EmployeeId> dataIdentityToDomainIdentityMapper,
    IEmployeeMongoRepository dataRepository) : base(
    domainEntityToDataEntityMapper,
    dataEntityToDomainEntityMapper,
    domainIdentityToDataIdentityMapper,
    dataIdentityToDomainIdentityMapper,
    dataRepository)
  {
    _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
  }

  public async Task<Option<EmployeeEntity>> FindByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken)
  {
    var entity = await _dataRepository.FindByUserIdAsync(
      userId, cancellationToken).ConfigureAwait(false);

    return entity.Map(Map);
  }

  public async Task<EmployeeEntity> GetByUserIdAsync(
    Guid tenantId,
    CancellationToken cancellationToken)
  {
    var entity = await _dataRepository.GetByUserIdAsync(
      tenantId, cancellationToken).ConfigureAwait(false);

    return entity is null ? throw new EntityNotFoundException() : Map(entity);
  }

  public async Task<PageResult<EmployeeEntity>> PageAsync(
    TenantEmployeePageQuery pageQuery,
    CancellationToken cancellationToken)
  {
    var pageResult = await _dataRepository.PageAsync(pageQuery, cancellationToken).ConfigureAwait(false);

    return new PageResult<EmployeeEntity>(
      pageResult.Page,
      pageResult.Items.Select(Map).ToArray(),
      pageResult.TotalItems);
  }
}
