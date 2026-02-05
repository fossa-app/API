using Fossa.API.Infrastructure.Models;
using TIKSN.Integration.Messages.Queries;

namespace Fossa.API.Infrastructure.Messages.Queries;

public record IdentityClientRetrievalQuery(
  Option<Uri> Origin) : IQuery<IdentityClient>;
