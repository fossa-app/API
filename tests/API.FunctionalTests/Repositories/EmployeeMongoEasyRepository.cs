using EasyDoubles;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Persistence.Mongo.Repositories;
using TIKSN.Data;

namespace Fossa.API.FunctionalTests.Repositories;

public class EmployeeMongoEasyRepository :
  EasyRepository<EmployeeMongoEntity, long>,
  IEmployeeMongoRepository
{
  public EmployeeMongoEasyRepository(IEasyStores easyStores) : base(easyStores)
  {
  }

  public Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  public Task<Option<EmployeeMongoEntity>> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    return Task.FromResult(Optional(EasyStore.Entities.Values.FirstOrDefault(x => x.UserID == userId)));
  }

  public Task<EmployeeMongoEntity> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    var entity = EasyStore.Entities.Values.FirstOrDefault(x => x.UserID == userId);
    if (entity is null)
    {
      return Task.FromException<EmployeeMongoEntity>(new EntityNotFoundException());
    }

    return Task.FromResult(entity);
  }

  public Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken)
  {
    return Task.FromResult(EasyStore.Entities.Values.Any(x => x.CompanyId == companyId));
  }

  public Task<PageResult<EmployeeMongoEntity>> PageAsync(TenantEmployeePageQuery pageQuery, CancellationToken cancellationToken)
  {
    var allItems = EasyStore.Entities.Values.Where(x => x.TenantID == pageQuery.TenantId).ToList();

    var pageItems = allItems.Skip((pageQuery.Page.Number - 1) * pageQuery.Page.Size).Take(pageQuery.Page.Size).ToList();

    return Task.FromResult(new PageResult<EmployeeMongoEntity>(pageQuery.Page, pageItems, allItems.Length()));
  }
}
