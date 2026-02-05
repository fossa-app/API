using TIKSN.Integration.Messages.Events;

namespace Fossa.API.Core.Messages.Events;

public interface IUserEvent<out TUserIdentity>
  : IEvent
  where TUserIdentity : IEquatable<TUserIdentity>
{
  TUserIdentity UserID { get; }
}
