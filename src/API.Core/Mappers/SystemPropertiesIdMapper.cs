using Fossa.API.Core.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Core.Mappers;

public class SystemPropertiesIdMapper : IMapper<SystemPropertiesId, long>, IMapper<long, SystemPropertiesId>
{
  public long Map(SystemPropertiesId source)
  {
    return source.AsPrimitive();
  }

  public SystemPropertiesId Map(long source)
  {
    return new SystemPropertiesId(source);
  }
}
