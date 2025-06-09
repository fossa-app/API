using EasyDoubles;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Persistence.Mongo.Repositories;
using TIKSN.Data;

namespace Fossa.API.FunctionalTests.Repositories;

public class CompanySettingsMongoEasyRepository :
  EasyRepository<CompanySettingsMongoEntity, long>,
  ICompanySettingsMongoRepository
{
  public CompanySettingsMongoEasyRepository(IEasyStores easyStores) : base(easyStores)
  {
  }

  public Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  public Task<Option<CompanySettingsMongoEntity>> FindByCompanyIdAsync(
    long companyId,
    CancellationToken cancellationToken)
  {
    return Task.FromResult(Optional(EasyStore.Entities.Values.FirstOrDefault(x => x.CompanyId == companyId)));
  }

  public Task<CompanySettingsMongoEntity> GetByCompanyIdAsync(
    long companyId,
    CancellationToken cancellationToken)
  {
    var entity = EasyStore.Entities.Values.FirstOrDefault(x => x.CompanyId == companyId);
    return entity is null ? throw new EntityNotFoundException() : Task.FromResult(entity);
  }

  public Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken)
  {
    return Task.FromResult(EasyStore.Entities.Values.Any(x => x.CompanyId == companyId));
  }
}
