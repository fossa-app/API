using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public interface IEmployeeMongoRepository : IMongoRepository<EmployeeMongoEntity, long>
{
  Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken);
  Task<Option<EmployeeMongoEntity>> FindByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken);

  Task<EmployeeMongoEntity> GetByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken);

  Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken);

  Task<PageResult<EmployeeMongoEntity>> PageAsync(
    TenantEmployeePageQuery pageQuery,
    CancellationToken cancellationToken);
}
