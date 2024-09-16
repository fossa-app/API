using Fossa.API.Core.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Core.Mappers;

public class BranchIdMapper : IMapper<BranchId, long>, IMapper<long, BranchId>
{
  public long Map(BranchId source)
  {
    return source.AsPrimitive();
  }

  public BranchId Map(long source)
  {
    return new BranchId(source);
  }
}
