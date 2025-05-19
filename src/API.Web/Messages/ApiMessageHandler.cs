using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
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

  private static readonly ApiTypeMap _apiResponseMap = new();

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

  protected async Task<TApiResponse> SendAsync<TDomainRequest, TDomainResponse>(
    TDomainRequest domainRequest,
    Func<TDomainResponse, TApiResponse> mapToApiResponse,
    CancellationToken cancellationToken)
    where TDomainRequest : IRequest<TDomainResponse>
  {
    ArgumentNullException.ThrowIfNull(mapToApiResponse);

    try
    {
      var domainResponse = await _sender.Send(domainRequest, cancellationToken)
        ?? throw new InvalidOperationException("Domain response cannot be null.");

      var apiResponse = mapToApiResponse(domainResponse)
        ?? throw new InvalidOperationException("API response cannot be null.");

      return apiResponse;
    }
    catch (ValidationException ex)
    {
      throw MapValidationException<TDomainRequest>(ex);
    }
  }

  private static Type? FindPropertyType(Type type, string propertyName)
  {
    if (type.IsGenericType &&
      (type.GetGenericTypeDefinition() == typeof(Nullable<>) ||
      type.GetGenericTypeDefinition() == typeof(Option<>)))
    {
      type = type.GetGenericArguments()[0];
    }

    var propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

    return propertyInfo?.PropertyType;
  }

  private static Type GetPropertyType(Type type, string propertyName)
  {
    return FindPropertyType(type, propertyName)
      ?? throw new InvalidOperationException($"Property '{propertyName}' not found in type '{type.Name}'.");
  }

  private static string MapPropertyName(
    string domainPropertyPath,
    Type domainType,
    Type apiType)
  {
    var propertyPathSegments = domainPropertyPath.Split('.') ?? [];
    var parentPathSegments = propertyPathSegments[..^1];
    var domainPropertyName = propertyPathSegments[^1];

    var (domainPropertyType, apiPropertyType) =
      parentPathSegments.Aggregate((domainType, apiType), (current, parentPathSegment) =>
      {
        var nextDomainTypePropertyType = GetPropertyType(domainType, parentPathSegment);
        var nextApiTypePropertyType = GetPropertyType(apiType, parentPathSegment);

        return (nextDomainTypePropertyType, nextApiTypePropertyType);
      });

    _ = GetPropertyType(domainPropertyType, domainPropertyName);
    var apiChildPropertyType = FindPropertyType(apiPropertyType, domainPropertyName);

    if (apiChildPropertyType is not null)
    {
      return domainPropertyPath;
    }

    var apiChildIdPropertyType = FindPropertyType(apiPropertyType, $"{domainPropertyName}Id");

    if (apiChildIdPropertyType is not null)
    {
      return string.Join('.', parentPathSegments.Append($"{domainPropertyName}Id"));
    }

    var apiChildCodePropertyType = FindPropertyType(apiPropertyType, $"{domainPropertyName}Code");

    if (apiChildCodePropertyType is not null)
    {
      return string.Join('.', parentPathSegments.Append($"{domainPropertyName}Code"));
    }

    throw new InvalidOperationException($"Property '{domainPropertyName}' or its equivalent not found in type '{apiType.Name}'.");
  }

  private static ValidationException MapValidationException<TDomainRequest>(ValidationException ex)
  {
    var apiRequestType = typeof(TApiRequest);
    var domainRequestType = typeof(TDomainRequest);

    return new ValidationException(ex.Errors.Map(x => new ValidationFailure
    {
      PropertyName = _apiResponseMap
          .GetOrAdd(apiRequestType, _ => new DomainTypeMap())
          .GetOrAdd(domainRequestType, _ => new PropertyNameMap())
          .GetOrAdd(x.PropertyName, x => MapPropertyName(x, domainRequestType, apiRequestType)),
      ErrorMessage = x.ErrorMessage,
      AttemptedValue = x.AttemptedValue,
      Severity = x.Severity,
      CustomState = x.CustomState,
      ErrorCode = x.ErrorCode,
    }));
  }

  public class ApiTypeMap : ConcurrentDictionary<Type, DomainTypeMap>;

  public class DomainTypeMap : ConcurrentDictionary<Type, PropertyNameMap>;

  public class PropertyNameMap : ConcurrentDictionary<string, string>
  {
    public PropertyNameMap() : base(StringComparer.OrdinalIgnoreCase)
    {
    }
  }
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

  public override Task<TApiResponse> Handle(TApiRequest request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var domainRequest = MapToDomainRequest(request)
      ?? throw new InvalidOperationException("Domain request cannot be null.");

    return SendAsync<TDomainRequest, TDomainResponse>(
      domainRequest,
      MapToApiResponse,
      cancellationToken);
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
    ArgumentNullException.ThrowIfNull(request);

    return MapToDomainRequest(request)
      .MatchAsync(
        query => SendAsync<TDomainRequest2, TDomainResponse2>(query, MapToApiResponse, cancellationToken),
        query => SendAsync<TDomainRequest1, TDomainResponse1>(query, MapToApiResponse, cancellationToken));
  }

  protected abstract TApiResponse MapToApiResponse(TDomainResponse1 domainResponse);

  protected abstract TApiResponse MapToApiResponse(TDomainResponse2 domainResponse);

  protected abstract Either<TDomainRequest1, TDomainRequest2> MapToDomainRequest(TApiRequest apiRequest);
}
