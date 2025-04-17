using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public interface IDepartmentMongoRepository : IMongoRepository<DepartmentMongoEntity, long>
{
  Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken);
  Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken);
  Task<bool> HasDependencyOnManagerAsync(long managerId, CancellationToken cancellationToken);
  Task<bool> HasDependencyOnParentAsync(long parentId, CancellationToken cancellationToken);
  Task<PageResult<DepartmentMongoEntity>> PageAsync(
      TenantDepartmentPageQuery pageQuery,
      CancellationToken cancellationToken);
}
