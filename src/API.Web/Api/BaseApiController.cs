// Ignore Spelling: Api

using Microsoft.AspNetCore.Mvc;

namespace Fossa.API.Web.Api;

[Route("api/{version:apiVersion}/[controller]")]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
  protected readonly IPublisher _publisher;
  protected readonly ISender _sender;

  protected BaseApiController(
    ISender sender,
    IPublisher publisher)
  {
    _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }
}
