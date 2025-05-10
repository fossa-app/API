using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages;

public abstract class ApiMessageHandler<TEntityIdentity, TApiRequest, TApiResponse, TDomainRequest, TDomainResponse> : IRequestHandler<TApiRequest, TApiResponse>
  where TApiRequest : IRequest<TApiResponse>
  where TDomainRequest : IRequest<TDomainResponse>
{
  protected readonly IMapper<long, TEntityIdentity> _dataIdentityToDomainIdentityMapper;
  protected readonly IMapper<TEntityIdentity, long> _domainIdentityToDataIdentityMapper;
  protected readonly ISender _sender;
  protected readonly ITenantIdProvider<Guid> _tenantIdProvider;
  protected readonly IUserIdProvider<Guid> _userIdProvider;

  protected ApiMessageHandler(
    ISender sender,
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    IMapper<TEntityIdentity, long> domainIdentityToDataIdentityMapper,
    IMapper<long, TEntityIdentity> dataIdentityToDomainIdentityMapper)
  {
    _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    _tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
    _userIdProvider = userIdProvider ?? throw new ArgumentNullException(nameof(userIdProvider));
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
  }

  public async Task<TApiResponse> Handle(TApiRequest request, CancellationToken cancellationToken)
  {
    if (request is null)
    {
      throw new ArgumentNullException(nameof(request));
    }
    var domainRequest = MapToDomainRequest(request);

    var domainResponse = await _sender.Send(domainRequest, cancellationToken)
      ?? throw new InvalidOperationException("Domain response cannot be null.");

    var apiResponse = MapToApiResponse(domainResponse)
      ?? throw new InvalidOperationException("API response cannot be null.");

    return apiResponse;
  }

  protected abstract TApiResponse MapToApiResponse(TDomainResponse domainResponse);

  protected abstract TDomainRequest MapToDomainRequest(TApiRequest apiRequest);
}
