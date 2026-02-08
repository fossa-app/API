using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record CompanySettingsDeletedEvent(
    Guid TenantID,
    CompanySettingsId CompanySettingsId,
    CompanyId CompanyId)
  : CompanyEvent<Guid>(TenantID, CompanyId);
