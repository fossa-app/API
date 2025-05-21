using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using MongoDB.Driver;
using TIKSN.Data;
using TIKSN.Data.Mongo;

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

  protected override SortDefinition<EmployeeMongoEntity> PageSortDefinition
    => Builders<EmployeeMongoEntity>.Sort.Ascending(x => x.ID);

  public Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken)
  {
    var indexKeysDefinition =
      Builders<EmployeeMongoEntity>
      .IndexKeys
      .Text(x => x.FirstName)
      .Text(x => x.LastName)
      .Text(x => x.FullName);
    var indexOptions = new CreateIndexOptions
    {
      Name = "text_index",
      Collation = MongoRepositoryHelper.CreateDefaultCollation(),
    };
    var indexModel = new CreateIndexModel<EmployeeMongoEntity>(indexKeysDefinition, indexOptions);
    return Collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
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

  public async Task<bool> HasDependencyOnBranchAsync(long branchId, CancellationToken cancellationToken)
  {
    var filter =
      Builders<EmployeeMongoEntity>.Filter.Eq(item => item.AssignedBranchId, branchId);

    var firstEntity = await FirstOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return firstEntity is not null;
  }

  public async Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken)
  {
    var filter =
      Builders<EmployeeMongoEntity>.Filter.Eq(item => item.CompanyId, companyId);

    var firstEntity = await FirstOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return firstEntity is not null;
  }

  public async Task<bool> HasDependencyOnDepartmentAsync(long departmentId, CancellationToken cancellationToken)
  {
    var filter =
      Builders<EmployeeMongoEntity>.Filter.Eq(item => item.AssignedDepartmentId, departmentId);

    var firstEntity = await FirstOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return firstEntity is not null;
  }

  public Task<PageResult<EmployeeMongoEntity>> PageAsync(
    TenantEmployeePageQuery pageQuery,
    CancellationToken cancellationToken)
  {
    var filterByTenantId =
      Builders<EmployeeMongoEntity>.Filter.Eq(item => item.TenantID, pageQuery.TenantId);

    var search = pageQuery.Search.Trim();
    var filter = string.IsNullOrEmpty(search)
      ? filterByTenantId
      : Builders<EmployeeMongoEntity>.Filter.And(
        filterByTenantId,
        Builders<EmployeeMongoEntity>.Filter.Text(search));

    return this.PageAsync(filter, pageQuery, cancellationToken);
  }

  public async Task<int> CountAllAsync(long companyId, CancellationToken cancellationToken)
  {
    var filter =
      Builders<EmployeeMongoEntity>.Filter.Eq(item => item.CompanyId, companyId);

    var count = await MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync).ConfigureAwait(false);

    return (int)count;

    Task<long> SomeAsync(IClientSessionHandle clientSessionHandle)
    {
      return Collection
        .CountDocumentsAsync(clientSessionHandle, filter, options: null, cancellationToken);
    }

    Task<long> NoneAsync()
    {
      return Collection
        .CountDocumentsAsync(filter, options: null, cancellationToken);
    }
  }
}
