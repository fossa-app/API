using TIKSN.Identity;
using TIKSN.Mapping;

namespace Fossa.API.Core.Identity;

public class MapperIdentityGenerator<TSource, TDestination> : IIdentityGenerator<TDestination>
{
  private readonly IMapper<TSource, TDestination> _mapper;
  private readonly IIdentityGenerator<TSource> _identityGenerator;

  public MapperIdentityGenerator(
    IMapper<TSource, TDestination> mapper,
    IIdentityGenerator<TSource> identityGenerator)
  {
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
  }

  public TDestination Generate()
  {
    return _mapper.Map(_identityGenerator.Generate());
  }
}
