// Ignore Spelling: Api

using MediatR;
using Microsoft.AspNetCore.Mvc;
using TIKSN.Mapping;

namespace Fossa.API.Web.Api;

/// <summary>
/// If your API controllers will use a consistent route convention and the [ApiController] attribute (they should)
/// then it's a good idea to define and use a common base controller class like this one.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController<TEntityIdentity> : Controller
{
  protected readonly IPublisher _publisher;
  protected readonly IMapper<TEntityIdentity, long> _domainIdentityToDataIdentityMapper;
  protected readonly IMapper<long, TEntityIdentity> _dataIdentityToDomainIdentityMapper;
  protected readonly ISender _sender;

  protected BaseApiController(
    ISender sender,
    IPublisher publisher,
    IMapper<TEntityIdentity, long> domainIdentityToDataIdentityMapper,
    IMapper<long, TEntityIdentity> dataIdentityToDomainIdentityMapper)
  {
    _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ??
                                          throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ??
                                          throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
  }
}
