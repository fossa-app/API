using Fossa.API.Core.Messages.Queries;
using Fossa.API.Infrastructure.Models;
using LanguageExt;

namespace Fossa.API.Infrastructure.Messages.Queries;

public record IdentityClientRetrievalQuery(
  Option<Uri> Origin) : IQuery<IdentityClient>;
