using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data;
using TIKSN.Data.Mongo;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class DepartmentRepositoryAdapter
    : MongoRepositoryAdapter<DepartmentEntity, DepartmentId, DepartmentMongoEntity, long>
    , IDepartmentRepository, IDepartmentQueryRepository, IDepartmentIndexRepository
{
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IDepartmentMongoRepository _dataRepository;
  private readonly IMapper<DepartmentId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<EmployeeId, long> _employeeDomainIdentityToDataIdentityMapper;

  public DepartmentRepositoryAdapter(
      IMapper<DepartmentEntity, DepartmentMongoEntity> domainEntityToDataEntityMapper,
      IMapper<DepartmentMongoEntity, DepartmentEntity> dataEntityToDomainEntityMapper,
      IMapper<DepartmentId, long> domainIdentityToDataIdentityMapper,
      IMapper<long, DepartmentId> dataIdentityToDomainIdentityMapper,
      IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper,
      IMapper<EmployeeId, long> employeeDomainIdentityToDataIdentityMapper,
      IDepartmentMongoRepository dataRepository) : base(
      domainEntityToDataEntityMapper,
      dataEntityToDomainEntityMapper,
      domainIdentityToDataIdentityMapper,
      dataIdentityToDomainIdentityMapper,
      dataRepository)
  {
    _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainIdentityToDataIdentityMapper));
    _employeeDomainIdentityToDataIdentityMapper = employeeDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(employeeDomainIdentityToDataIdentityMapper));
  }

  public Task<bool> HasDependencyAsync(EmployeeId id, CancellationToken cancellationToken)
  {
    return _dataRepository.HasDependencyOnManagerAsync(_employeeDomainIdentityToDataIdentityMapper.Map(id), cancellationToken);
  }

  public Task<bool> HasDependencyAsync(DepartmentId id, CancellationToken cancellationToken)
  {
    return _dataRepository.HasDependencyOnParentAsync(_domainIdentityToDataIdentityMapper.Map(id), cancellationToken);
  }

  public Task<bool> HasDependencyAsync(CompanyId id, CancellationToken cancellationToken)
  {
    return _dataRepository.HasDependencyOnCompanyAsync(_companyDomainIdentityToDataIdentityMapper.Map(id), cancellationToken);
  }

  public async Task<PageResult<DepartmentEntity>> PageAsync(
      TenantDepartmentPageQuery pageQuery,
      CancellationToken cancellationToken)
  {
    var pageResult = await _dataRepository.PageAsync(pageQuery, cancellationToken).ConfigureAwait(false);

    return new PageResult<DepartmentEntity>(
        pageResult.Page,
        [.. pageResult.Items.Select(Map)],
        pageResult.TotalItems);
  }

  public Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken)
  {
    return _dataRepository.EnsureIndexesCreatedAsync(cancellationToken);
  }
}
