using TIKSN.Integration.Messages.Events;

namespace Fossa.API.Core.Messages.Events;

public interface ITenantEvent<out TTenantIdentity>
  : IEvent
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  TTenantIdentity TenantID { get; }
}
