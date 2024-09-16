namespace Fossa.API.Core.Messages.Commands;

public interface ITenantUserCommand<TEntityIdentity, out TTenantIdentity, out TUserIdentity>
  : ITenantCommand<TEntityIdentity, TTenantIdentity>
  , IUserCommand<TUserIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>;
