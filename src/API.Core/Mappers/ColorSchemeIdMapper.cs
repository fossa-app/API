using Fossa.API.Core.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Core.Mappers;

public class ColorSchemeIdMapper : IMapper<ColorSchemeId, string>, IMapper<string, ColorSchemeId>
{
  public string Map(ColorSchemeId source)
  {
    return source.AsPrimitive();
  }

  public ColorSchemeId Map(string source)
  {
    return new ColorSchemeId(source);
  }
}
