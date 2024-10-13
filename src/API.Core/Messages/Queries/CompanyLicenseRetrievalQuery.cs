using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Queries;

public record CompanyLicenseRetrievalQuery(Guid TenantID) : IQuery<License<CompanyEntitlements>>;
