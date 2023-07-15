using IdGen;

namespace Fossa.API.Core.Identity;

public class IdGenIdentityGenerator : IIdentityGenerator<long>
{
  private readonly IIdGenerator<long> _idGenerator;

  public IdGenIdentityGenerator(IIdGenerator<long> idGenerator)
  {
    _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
  }

  public long Generate()
  {
    return _idGenerator.CreateId();
  }
}
