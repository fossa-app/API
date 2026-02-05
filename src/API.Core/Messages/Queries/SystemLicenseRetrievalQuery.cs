using Fossa.Licensing;
using TIKSN.Integration.Messages.Queries;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Queries;

public record SystemLicenseRetrievalQuery : IQuery<License<SystemEntitlements>>;
