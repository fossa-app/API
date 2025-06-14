﻿using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using MongoDB.Driver;
using TIKSN.Data;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class BranchMongoRepository
  : MongoRepository<BranchMongoEntity, long>, IBranchMongoRepository
{
  public BranchMongoRepository(
    IMongoClientSessionProvider mongoClientSessionProvider,
    IMongoDatabaseProvider mongoDatabaseProvider) : base(
    mongoClientSessionProvider,
    mongoDatabaseProvider,
    "Branches")
  {
  }

  protected override SortDefinition<BranchMongoEntity> PageSortDefinition
    => Builders<BranchMongoEntity>.Sort.Ascending(x => x.ID);

  public Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken)
  {
    var indexKeysDefinition =
      Builders<BranchMongoEntity>
      .IndexKeys
      .Text(x => x.Name);
    var indexOptions = new CreateIndexOptions
    {
      Name = "text_index",
      Collation = MongoRepositoryHelper.CreateDefaultCollation(),
    };
    var indexModel = new CreateIndexModel<BranchMongoEntity>(indexKeysDefinition, indexOptions);
    return Collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
  }

  public async Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken)
  {
    var filter =
      Builders<BranchMongoEntity>.Filter.Eq(item => item.CompanyId, companyId);

    var firstEntity = await FirstOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return firstEntity is not null;
  }

  public Task<PageResult<BranchMongoEntity>> PageAsync(
    TenantBranchPageQuery pageQuery,
    CancellationToken cancellationToken)
  {
    var filterByTenantId =
      Builders<BranchMongoEntity>.Filter.Eq(item => item.TenantID, pageQuery.TenantId);

    var search = pageQuery.Search.Trim();
    var filter = string.IsNullOrEmpty(search)
      ? filterByTenantId
      : Builders<BranchMongoEntity>.Filter.And(
        filterByTenantId,
        Builders<BranchMongoEntity>.Filter.Text(search));

    return this.PageAsync(filter, pageQuery, cancellationToken);
  }

  public async Task<int> CountAllAsync(long companyId, CancellationToken cancellationToken)
  {
    var filter =
      Builders<BranchMongoEntity>.Filter.Eq(item => item.CompanyId, companyId);

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
