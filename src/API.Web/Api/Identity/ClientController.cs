using Asp.Versioning;
using Fossa.API.Infrastructure.Messages.Queries;
using Fossa.API.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fossa.API.Web.Api.Identity;

[Tags("Identity Client")]
[ApiVersion(1.0)]
[Route("api/{version:apiVersion}/Identity/[controller]")]
[ApiController]
public class ClientController : BaseApiController
{
  public ClientController(
    ISender sender,
    IPublisher publisher)
    : base(sender, publisher)
  {
  }

  [HttpGet]
  public async Task<IdentityClient> GetAsync(
    [FromQuery] string? origin,
    CancellationToken cancellationToken)
  {
    var identityClient = await _sender.Send(
      new IdentityClientRetrievalQuery(
        Optional(origin).Map(x => new Uri(x))),
      cancellationToken);

    return identityClient;
  }
}
