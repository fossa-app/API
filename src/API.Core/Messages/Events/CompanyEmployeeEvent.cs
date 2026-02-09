using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public abstract record CompanyEmployeeEvent<TTenantIdentity, TUserIdentity> : TenantUserEvent<TTenantIdentity, TUserIdentity>, ICompanyEmployeeEvent<TTenantIdentity, TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
{
  protected CompanyEmployeeEvent(TTenantIdentity tenantID, CompanyId companyId, TUserIdentity userID, EmployeeId employeeId) : base(tenantID, userID)
  {
    CompanyId = companyId;
    EmployeeId = employeeId;
  }

  public CompanyId CompanyId { get; }
  public EmployeeId EmployeeId { get; }
}
