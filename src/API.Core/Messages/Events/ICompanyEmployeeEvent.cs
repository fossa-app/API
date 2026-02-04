using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public interface ICompanyEmployeeEvent<out TUserIdentity, out TTenantIdentity>
  : ITenantUserEvent<TUserIdentity, TTenantIdentity>, ICompanyEvent<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>

{
  EmployeeId EmployeeId { get; }
}
