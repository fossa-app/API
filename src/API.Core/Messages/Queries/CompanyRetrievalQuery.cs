using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record CompanyRetrievalQuery(
  Guid TenantID)
  : ITenantQuery<Guid, CompanyEntity>;
