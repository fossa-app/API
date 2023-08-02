namespace Fossa.API.Core.Messages.Commands;

public interface ITenantUserCommand<out TUserIdentity, out TTenantIdentity>
  : ITenantCommand<TTenantIdentity>
{
  TUserIdentity UserID { get; }
}
