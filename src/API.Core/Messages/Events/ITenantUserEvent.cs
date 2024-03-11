namespace Fossa.API.Core.Messages.Events;

public interface ITenantUserEvent<out TUserIdentity, out TTenantIdentity>
  : ITenantEvent<TTenantIdentity>
  , IUserEvent<TUserIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>;
