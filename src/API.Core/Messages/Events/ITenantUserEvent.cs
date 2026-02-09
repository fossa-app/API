namespace Fossa.API.Core.Messages.Events;

public interface ITenantUserEvent<out TTenantIdentity, out TUserIdentity>
  : ITenantEvent<TTenantIdentity>
  , IUserEvent<TUserIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>;
