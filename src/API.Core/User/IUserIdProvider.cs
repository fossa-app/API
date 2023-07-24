using LanguageExt;

namespace Fossa.API.Core.User;

public interface IUserIdProvider<T>
{
  Option<T> FindUserId();

  T GetUserId();
}
