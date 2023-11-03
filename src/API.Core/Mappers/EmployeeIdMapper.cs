using Fossa.API.Core.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Core.Mappers;

public class EmployeeIdMapper : IMapper<EmployeeId, long>, IMapper<long, EmployeeId>
{
  public long Map(EmployeeId source)
  {
    return source.AsPrimitive();
  }

  public EmployeeId Map(long source)
  {
    return new EmployeeId(source);
  }
}
