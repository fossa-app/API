using EasyDoubles;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Persistence.Mongo.Repositories;
using TIKSN.Data;

namespace Fossa.API.FunctionalTests.Repositories;

public class BranchMongoEasyRepository :
  EasyRepository<BranchMongoEntity, long>,
  IBranchMongoRepository
{
  public BranchMongoEasyRepository(IEasyStores easyStores) : base(easyStores)
  {
  }

  public Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  public Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken)
  {
    return Task.FromResult(EasyStore.Entities.Values.Any(x => x.CompanyId == companyId));
  }

  public Task<PageResult<BranchMongoEntity>> PageAsync(TenantBranchPageQuery pageQuery, CancellationToken cancellationToken)
  {
    var allItems = EasyStore.Entities.Values.Where(x => x.TenantID == pageQuery.TenantId).ToList();

    var pageItems = allItems.Skip((pageQuery.Page.Number - 1) * pageQuery.Page.Size).Take(pageQuery.Page.Size).ToList();

    return Task.FromResult(new PageResult<BranchMongoEntity>(pageQuery.Page, pageItems, allItems.Length()));
  }
}
