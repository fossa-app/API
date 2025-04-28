using EasyDoubles;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Persistence.Mongo.Repositories;
using MongoDB.Driver;
using TIKSN.Data;

namespace Fossa.API.FunctionalTests.Repositories;

public class DepartmentMongoEasyRepository : EasyRepository<DepartmentMongoEntity, long>, IDepartmentMongoRepository
{
  public DepartmentMongoEasyRepository(IEasyStores easyStores) : base(easyStores)
  {
  }

  public Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  public Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken)
  {
    return Task.FromResult(EasyStore.Entities.Values.Any(x => x.CompanyId == companyId));
  }

  public Task<bool> HasDependencyOnManagerAsync(long managerId, CancellationToken cancellationToken)
  {
    return Task.FromResult(EasyStore.Entities.Values.Any(x => x.ManagerId == managerId));
  }

  public Task<bool> HasDependencyOnParentAsync(long parentId, CancellationToken cancellationToken)
  {
    return Task.FromResult(EasyStore.Entities.Values.Any(x => x.ParentDepartmentId == parentId));
  }

  public Task<PageResult<DepartmentMongoEntity>> PageAsync(
      TenantDepartmentPageQuery pageQuery,
      CancellationToken cancellationToken)
  {
    var allItems = EasyStore.Entities.Values.Where(x => x.TenantID == pageQuery.TenantId).ToList();

    var pageItems = allItems.Skip((pageQuery.Page.Number - 1) * pageQuery.Page.Size).Take(pageQuery.Page.Size).ToList();

    return Task.FromResult(new PageResult<DepartmentMongoEntity>(pageQuery.Page, pageItems, allItems.Length()));
  }
}
