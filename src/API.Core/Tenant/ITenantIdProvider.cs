using LanguageExt;

namespace Fossa.API.Core.Tenant;

public interface ITenantIdProvider<T>
{
  Option<T> FindTenantId();

  T GetTenantId();
}
