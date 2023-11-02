using Fossa.API.Core.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Core.Mappers;

public class CompanyIdMapper : IMapper<CompanyId, long>, IMapper<long, CompanyId>
{
  public long Map(CompanyId source)
  {
    return source.AsPrimitive();
  }

  public CompanyId Map(long source)
  {
    return new CompanyId(source);
  }
}
