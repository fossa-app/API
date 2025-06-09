using TIKSN.Data;

namespace Fossa.API.Core.Entities;

public record CompanySettingsEntity(
    CompanySettingsId ID,
    CompanyId CompanyId,
    ColorSchemeId ColorSchemeId) : IEntity<CompanySettingsId>;
