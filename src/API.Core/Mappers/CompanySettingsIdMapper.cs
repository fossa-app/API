using Fossa.API.Core.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Core.Mappers;

public class CompanySettingsIdMapper : IMapper<CompanySettingsId, long>, IMapper<long, CompanySettingsId>
{
  public long Map(CompanySettingsId source)
  {
    return source.AsPrimitive();
  }

  public CompanySettingsId Map(long source)
  {
    return new CompanySettingsId(source);
  }
}
