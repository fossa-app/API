namespace Fossa.API.Core.Identity;

public interface IIdentityGenerator<out T>
{
  T Generate();
}
