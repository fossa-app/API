using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public interface IBranchMongoRepository : IMongoRepository<BranchMongoEntity, long>
{
  Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken);
  Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken);

  Task<PageResult<BranchMongoEntity>> PageAsync(
    TenantBranchPageQuery pageQuery,
    CancellationToken cancellationToken);
}
