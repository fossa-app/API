using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Queries;

public record SystemLicenseRetrievalQuery : IQuery<License<SystemEntitlements>>;
