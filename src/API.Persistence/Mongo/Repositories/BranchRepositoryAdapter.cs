using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using LanguageExt;
using TIKSN.Data;
using TIKSN.Data.Mongo;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class BranchRepositoryAdapter
  : MongoRepositoryAdapter<BranchEntity, BranchId, BranchMongoEntity, long>
    , IBranchRepository, IBranchQueryRepository
{
  private readonly IBranchMongoRepository _dataRepository;

  public BranchRepositoryAdapter(
    IMapper<BranchEntity, BranchMongoEntity> domainEntityToDataEntityMapper,
    IMapper<BranchMongoEntity, BranchEntity> dataEntityToDomainEntityMapper,
    IMapper<BranchId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, BranchId> dataIdentityToDomainIdentityMapper,
    IBranchMongoRepository dataRepository) : base(
    domainEntityToDataEntityMapper,
    dataEntityToDomainEntityMapper,
    domainIdentityToDataIdentityMapper,
    dataIdentityToDomainIdentityMapper,
    dataRepository)
  {
    _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
  }

  public async Task<PageResult<BranchEntity>> PageAsync(
    TenantBranchPageQuery pageQuery,
    CancellationToken cancellationToken)
  {
    var pageResult = await _dataRepository.PageAsync(pageQuery, cancellationToken).ConfigureAwait(false);

    return new PageResult<BranchEntity>(
      pageResult.Page,
      pageResult.Items.Select(Map).ToArray(),
      pageResult.TotalItems);
  }
}
