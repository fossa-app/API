using EasyDoubles;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;

namespace Fossa.API.FunctionalTests.Repositories;

public class SystemPropertiesEasyRepository :
  EasyRepository<SystemPropertiesEntity, SystemPropertiesId>,
  ISystemPropertiesRepository,
  ISystemPropertiesQueryRepository
{
  public SystemPropertiesEasyRepository(IEasyStores easyStores) : base(easyStores)
  {
  }
}
