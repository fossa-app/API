using EasyDoubles;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Persistence.Mongo.Repositories;

namespace Fossa.API.FunctionalTests.Repositories;

public class SystemPropertiesMongoEasyRepository :
  EasyRepository<SystemPropertiesMongoEntity, long>,
  ISystemPropertiesMongoRepository
{
  public SystemPropertiesMongoEasyRepository(IEasyStores easyStores) : base(easyStores)
  {
  }
}
