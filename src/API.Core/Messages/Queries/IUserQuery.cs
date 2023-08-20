namespace Fossa.API.Core.Messages.Queries;

public interface IUserQuery<out TUserIdentity, out TResult>
  : IQuery<TResult>
  where TUserIdentity : IEquatable<TUserIdentity>
{
  TUserIdentity UserID { get; }
}
