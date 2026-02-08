using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public abstract record CompanyEvent<TTenantIdentity> : TenantEvent<TTenantIdentity>, ICompanyEvent<TTenantIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  protected CompanyEvent(TTenantIdentity tenantID, CompanyId companyId) : base(tenantID)
  {
    CompanyId = companyId;
  }

  public CompanyId CompanyId { get; }
}
