using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;

namespace Fossa.API.Core.Entities;

public record EmployeeEntity(
    long ID,
    Guid TenantID,
    Guid UserID,
    long CompanyId,
    string FirstName,
    string LastName,
    string FullName)
  : ITenantEntity<long, Guid>, IUserEntity<long, Guid>;
