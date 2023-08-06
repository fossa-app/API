using Fossa.API.Core.User;

namespace Fossa.API.Core.Entities;

public record UserEntity<TEntityIdentity, TUserIdentity>(
  TEntityIdentity ID,
  TUserIdentity UserID)
  : IUserEntity<TEntityIdentity, TUserIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>;
