using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public interface ICompanyEmployeeEvent<out TTenantIdentity, out TUserIdentity>
  : ITenantUserEvent<TTenantIdentity, TUserIdentity>, ICompanyEvent<TTenantIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>

{
  EmployeeId EmployeeId { get; }
}
