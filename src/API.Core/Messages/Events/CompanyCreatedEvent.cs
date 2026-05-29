using Fossa.API.Core.Entities;
using TIKSN.Globalization;

namespace Fossa.API.Core.Messages.Events;

public record CompanyCreatedEvent(
    Guid TenantID,
    CompanyId CompanyId,
    string Name,
    CountryInfo Country)
  : CompanyEvent<Guid>(TenantID, CompanyId);
