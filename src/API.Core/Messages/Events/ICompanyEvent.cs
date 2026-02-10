using Fossa.API.Core.Entities;
using TIKSN.Integration.Messages.Events;

namespace Fossa.API.Core.Messages.Events;

public interface ICompanyEvent<out TTenantIdentity>
  : ITenantEvent<TTenantIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  CompanyId CompanyId { get; }
}
