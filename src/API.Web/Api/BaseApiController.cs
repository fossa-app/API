// Ignore Spelling: Api

using MediatR;
using Microsoft.AspNetCore.Mvc;
using TIKSN.Mapping;

namespace Fossa.API.Web.Api;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController : Controller
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

[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController<TEntityIdentity> : BaseApiController
{
  protected readonly IMapper<long, TEntityIdentity> _dataIdentityToDomainIdentityMapper;
  protected readonly IMapper<TEntityIdentity, long> _domainIdentityToDataIdentityMapper;

  protected BaseApiController(
    ISender sender,
    IPublisher publisher,
    IMapper<TEntityIdentity, long> domainIdentityToDataIdentityMapper,
    IMapper<long, TEntityIdentity> dataIdentityToDomainIdentityMapper)
    : base(sender, publisher)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ??
                                          throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ??
                                          throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
  }
}
