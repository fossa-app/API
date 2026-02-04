using System.Globalization;
using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record CompanyCreatedEvent(
    Guid TenantID,
    CompanyId CompanyId,
    string Name,
    RegionInfo Country)
  : ICompanyEvent<Guid>;
