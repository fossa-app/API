using EasyDoubles;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Persistence.Mongo.Repositories;
using LanguageExt;
using TIKSN.Data;
using static LanguageExt.Prelude;

namespace Fossa.API.FunctionalTests.Repositories;

public class CompanyMongoEasyRepository :
  EasyRepository<CompanyMongoEntity, long>,
  ICompanyMongoRepository
{
  public CompanyMongoEasyRepository(IEasyStores easyStores) : base(easyStores)
  {
  }

  public Task<int> CountAllAsync(CancellationToken cancellationToken)
  {
    return Task.FromResult(EasyStore.Entities.Count);
  }

  public Task<Option<CompanyMongoEntity>> FindByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
  {
    return Task.FromResult(Optional(EasyStore.Entities.Values.FirstOrDefault(x => x.TenantID == tenantId)));
  }

  public Task<CompanyMongoEntity> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
  {
    var entity = EasyStore.Entities.Values.FirstOrDefault(x => x.TenantID == tenantId);
    if (entity is null)
    {
      return Task.FromException<CompanyMongoEntity>(new EntityNotFoundException());
    }

    return Task.FromResult(entity);
  }
}
