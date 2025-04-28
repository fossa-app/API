using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using MongoDB.Driver;
using TIKSN.Data;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class DepartmentMongoRepository
    : MongoRepository<DepartmentMongoEntity, long>, IDepartmentMongoRepository
{
  public DepartmentMongoRepository(
      IMongoClientSessionProvider mongoClientSessionProvider,
      IMongoDatabaseProvider mongoDatabaseProvider) : base(
      mongoClientSessionProvider,
      mongoDatabaseProvider,
      "Departments")
  {
  }

  protected override SortDefinition<DepartmentMongoEntity> PageSortDefinition
      => Builders<DepartmentMongoEntity>.Sort.Ascending(x => x.ID);

  public Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken)
  {
    var indexKeysDefinition =
        Builders<DepartmentMongoEntity>
        .IndexKeys
        .Text(x => x.Name);
    var indexOptions = new CreateIndexOptions
    {
      Name = "text_index",
      Collation = MongoRepositoryHelper.CreateDefaultCollation(),
    };
    var indexModel = new CreateIndexModel<DepartmentMongoEntity>(indexKeysDefinition, indexOptions);
    return Collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
  }

  public async Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken)
  {
    var filter = Builders<DepartmentMongoEntity>.Filter.Eq(x => x.CompanyId, companyId);
    var count = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);
    return count > 0;
  }

  public async Task<bool> HasDependencyOnManagerAsync(long managerId, CancellationToken cancellationToken)
  {
    var filter = Builders<DepartmentMongoEntity>.Filter.Eq(x => x.ManagerId, managerId);
    var count = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);
    return count > 0;
  }

  public async Task<bool> HasDependencyOnParentAsync(long parentId, CancellationToken cancellationToken)
  {
    var filter = Builders<DepartmentMongoEntity>.Filter.Eq(x => x.ParentDepartmentId, parentId);
    var count = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);
    return count > 0;
  }

  public Task<PageResult<DepartmentMongoEntity>> PageAsync(
      TenantDepartmentPageQuery pageQuery,
      CancellationToken cancellationToken)
  {
    var filterByTenantId =
        Builders<DepartmentMongoEntity>.Filter.Eq(item => item.TenantID, pageQuery.TenantId);

    var search = pageQuery.Search.Trim();
    var filter = string.IsNullOrEmpty(search)
        ? filterByTenantId
        : Builders<DepartmentMongoEntity>.Filter.And(
            filterByTenantId,
            Builders<DepartmentMongoEntity>.Filter.Text(search));

    return this.PageAsync(filter, pageQuery, cancellationToken);
  }
}
