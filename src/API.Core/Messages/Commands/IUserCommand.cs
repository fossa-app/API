namespace Fossa.API.Core.Messages.Commands;

public interface IUserCommand<out TUserIdentity>
  : ICommand
  where TUserIdentity : IEquatable<TUserIdentity>
{
  TUserIdentity UserID { get; }
}
