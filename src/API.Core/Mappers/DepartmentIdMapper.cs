using Fossa.API.Core.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Core.Mappers;

public class DepartmentIdMapper : IMapper<DepartmentId, long>, IMapper<long, DepartmentId>
{
  public long Map(DepartmentId source)
  {
    return source.AsPrimitive();
  }

  public DepartmentId Map(long source)
  {
    return new DepartmentId(source);
  }
}
