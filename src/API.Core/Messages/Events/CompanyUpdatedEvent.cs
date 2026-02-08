using System.Globalization;
using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record CompanyUpdatedEvent(
    Guid TenantID,
    CompanyId CompanyId,
    string Name,
    RegionInfo Country)
  : CompanyEvent<Guid>(TenantID, CompanyId);
