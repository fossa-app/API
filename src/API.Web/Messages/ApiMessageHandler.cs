using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages;

public abstract class ApiMessageHandler<TEntityIdentity, TApiRequest, TApiResponse> : IRequestHandler<TApiRequest, TApiResponse>
  where TApiRequest : IRequest<TApiResponse>
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

  public abstract Task<TApiResponse> Handle(TApiRequest request, CancellationToken cancellationToken);
}

public abstract class ApiMessageHandler<TEntityIdentity, TApiRequest, TApiResponse, TDomainRequest, TDomainResponse>
  : ApiMessageHandler<TEntityIdentity, TApiRequest, TApiResponse>
  where TApiRequest : IRequest<TApiResponse>
  where TDomainRequest : IRequest<TDomainResponse>
{
  protected ApiMessageHandler(
    ISender sender,
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    IMapper<TEntityIdentity, long> domainIdentityToDataIdentityMapper,
    IMapper<long, TEntityIdentity> dataIdentityToDomainIdentityMapper)
    : base(sender, tenantIdProvider, userIdProvider, domainIdentityToDataIdentityMapper, dataIdentityToDomainIdentityMapper)
  {
  }

  public override async Task<TApiResponse> Handle(TApiRequest request, CancellationToken cancellationToken)
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

public abstract class ApiMessageHandler<TEntityIdentity, TApiRequest, TApiResponse, TDomainRequest1, TDomainResponse1, TDomainRequest2, TDomainResponse2>
  : ApiMessageHandler<TEntityIdentity, TApiRequest, TApiResponse>
  where TApiRequest : IRequest<TApiResponse>
  where TDomainRequest1 : IRequest<TDomainResponse1>
  where TDomainRequest2 : IRequest<TDomainResponse2>
{
  protected ApiMessageHandler(
    ISender sender,
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    IMapper<TEntityIdentity, long> domainIdentityToDataIdentityMapper,
    IMapper<long, TEntityIdentity> dataIdentityToDomainIdentityMapper)
  : base(sender, tenantIdProvider, userIdProvider, domainIdentityToDataIdentityMapper, dataIdentityToDomainIdentityMapper)
  {
  }

  public override Task<TApiResponse> Handle(TApiRequest request, CancellationToken cancellationToken)
  {
    if (request is null)
    {
      throw new ArgumentNullException(nameof(request));
    }

    return MapToDomainRequest(request)
      .Match<EitherAsync<TDomainResponse1, TDomainResponse2>>(
        query => _sender.Send(query, cancellationToken),
        query => _sender.Send(query, cancellationToken))
      .BiMap(MapToApiResponse, MapToApiResponse)
      .Match(x => x, x => x);
  }

  protected abstract TApiResponse MapToApiResponse(TDomainResponse1 domainResponse);

  protected abstract TApiResponse MapToApiResponse(TDomainResponse2 domainResponse);

  protected abstract Either<TDomainRequest1, TDomainRequest2> MapToDomainRequest(TApiRequest apiRequest);
}
