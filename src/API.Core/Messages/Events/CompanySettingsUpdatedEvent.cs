using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record CompanySettingsUpdatedEvent(
    Guid TenantID,
    CompanySettingsId CompanySettingsId,
    CompanyId CompanyId,
    ColorSchemeId ColorSchemeId)
  : ICompanyEvent<Guid>;
