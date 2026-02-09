using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record CompanyDeletedEvent(
    Guid TenantID,
    CompanyId CompanyId)
  : CompanyEvent<Guid>(TenantID, CompanyId);
