using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public abstract record CompanyEmployeeEvent<TUserIdentity, TTenantIdentity> : TenantUserEvent<TUserIdentity, TTenantIdentity>, ICompanyEmployeeEvent<TUserIdentity, TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  protected CompanyEmployeeEvent(TTenantIdentity tenantID, CompanyId companyId, TUserIdentity userID, EmployeeId employeeId) : base(tenantID, userID)
  {
    CompanyId = companyId;
    EmployeeId = employeeId;
  }

  public CompanyId CompanyId { get; }
  public EmployeeId EmployeeId { get; }
}
