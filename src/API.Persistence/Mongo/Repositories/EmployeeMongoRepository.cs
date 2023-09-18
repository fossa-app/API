using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using LanguageExt;
using MongoDB.Driver;
using TIKSN.Data;
using TIKSN.Data.Mongo;
using static LanguageExt.Prelude;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class EmployeeMongoRepository
  : MongoRepository<EmployeeMongoEntity, long>, IEmployeeMongoRepository
{
  public EmployeeMongoRepository(
    IMongoClientSessionProvider mongoClientSessionProvider,
    IMongoDatabaseProvider mongoDatabaseProvider) : base(
    mongoClientSessionProvider,
    mongoDatabaseProvider,
    "Employees")
  {
  }

  public async Task<Option<EmployeeMongoEntity>> FindByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken)
  {
    var filter =
      Builders<EmployeeMongoEntity>.Filter.Eq(item => item.UserID, userId);

    var entity = await SingleOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return Optional(entity);
  }

  public async Task<EmployeeMongoEntity> GetByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken)
  {
    var filter =
      Builders<EmployeeMongoEntity>.Filter.Eq(item => item.UserID, userId);

    var entity = await SingleOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return entity ?? throw new EntityNotFoundException();
  }

  public Task<PageResult<EmployeeMongoEntity>> PageAsync(
    TenantEmployeePageQuery pageQuery,
    CancellationToken cancellationToken)
  {
    var filter =
      Builders<EmployeeMongoEntity>.Filter.Eq(item => item.TenantID, pageQuery.TenantId);

    return this.PageAsync(filter, pageQuery, cancellationToken);
  }
}
