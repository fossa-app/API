namespace Fossa.API.Core.Messages.Commands;

public interface ITenantUserCommand<TEntityIdentity, out TUserIdentity, out TTenantIdentity>
  : ITenantCommand<TEntityIdentity, TTenantIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  TUserIdentity UserID { get; }
}
