using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record CompanySettingsCreatedEvent(
    Guid TenantID,
    CompanySettingsId CompanySettingsId,
    CompanyId CompanyId,
    ColorSchemeId ColorSchemeId)
  : CompanyEvent<Guid>(TenantID, CompanyId);
