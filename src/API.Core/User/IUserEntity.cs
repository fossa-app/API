using TIKSN.Data;

namespace Fossa.API.Core.User;

public interface IUserEntity<TEntityIdentity, TUserIdentity>
  : IEntity<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
{
  TUserIdentity UserID { get; }
}
