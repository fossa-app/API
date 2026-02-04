using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record EmployeeCreatedEvent(
    Guid TenantID,
    Guid UserID,
    EmployeeId EmployeeId,
    string FirstName,
    string LastName,
    string FullName,
    CompanyId CompanyId)
  : ICompanyEmployeeEvent<Guid, Guid>;
