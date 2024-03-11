using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Data;
using TIKSN.Data.BareEntityResolvers;

namespace Fossa.API.Core.Messages;

public class TenantRequestBehavior<TEntityIdentity, TTenantIdentity, TRequest, TResponse>
  : IPipelineBehavior<TRequest, TResponse>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TRequest : notnull
{
#pragma warning disable S2743 // Static fields should not be used in generic types
  private static readonly Type _entityAwareBareEntityResolverType = typeof(IBareEntityResolver<,,>);
#pragma warning restore S2743 // Static fields should not be used in generic types

  private readonly IServiceProvider _serviceProvider;
  private readonly ITenantIdProvider<TTenantIdentity> _tenantIdProvider;

  public TenantRequestBehavior(
    ITenantIdProvider<TTenantIdentity> tenantIdProvider,
    IServiceProvider serviceProvider)
  {
    _tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
    _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
  }

  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
  {
    await InspectRequestAsync(request, cancellationToken).ConfigureAwait(false);

    var response = await next().ConfigureAwait(false);

    await InspectResponseAsync(response, cancellationToken).ConfigureAwait(false);

    return response;
  }

  private async Task InspectAffectingTenantEntitiesAsync(
    IEnumerable<AffectingEntity<TEntityIdentity>> affectingEntities,
    TTenantIdentity tenantId,
    CancellationToken cancellationToken)
  {
    var bareEntityResolvers = affectingEntities
      .Select(x => x.EntityType)
      .Distinct()
      .ToDictionary(k => k, v => _entityAwareBareEntityResolverType
        .MakeGenericType(
          v,
          typeof(TenantEntity<TEntityIdentity, TTenantIdentity>),
          typeof(TEntityIdentity)));

    foreach (var affectingTenantEntity in affectingEntities)
    {
      var bareEntityResolver =
        (IBareEntityResolver<TenantEntity<TEntityIdentity, TTenantIdentity>, TEntityIdentity>)
        _serviceProvider.GetRequiredService(bareEntityResolvers[affectingTenantEntity.EntityType]);

      var bareTenantEntity = await bareEntityResolver.ResolveAsync(affectingTenantEntity.EntityID, cancellationToken)
        .ConfigureAwait(false);
      if (bareTenantEntity.TenantID == null || !bareTenantEntity.TenantID.Equals(tenantId))
      {
        throw new CrossTenantInboundUnauthorizedAccessException();
      }
    }
  }

  private Task InspectRequestAsync(
    TRequest request,
    CancellationToken cancellationToken)
  {
    if (request is ITenantCommand<TEntityIdentity, TTenantIdentity> tenantCommand)
    {
      return InspectTenantCommandAsync(tenantCommand, cancellationToken);
    }

    if (request is ITenantQuery<TEntityIdentity, TTenantIdentity, TResponse> tenantQuery)
    {
      return InspectTenantQueryAsync(tenantQuery, cancellationToken);
    }

    throw new InvalidOperationException("Request is not Tenant Command or Tenant Query");
  }

  private async Task InspectResponseAsync(
    TResponse? response,
    CancellationToken cancellationToken)
  {
    if (response is not null && response is not Unit)
    {
      var tenantId = _tenantIdProvider.GetTenantId();

      if (response is ITenantEntity<TEntityIdentity, TTenantIdentity> tenantEntity)
      {
        await InspectTenantEntityAsync(tenantId, tenantEntity, cancellationToken).ConfigureAwait(false);
      }
      else if (response is IEnumerable<ITenantEntity<TEntityIdentity, TTenantIdentity>> tenantEntities)
      {
        foreach (var currentTenantEntity in tenantEntities)
        {
          await InspectTenantEntityAsync(tenantId, currentTenantEntity, cancellationToken).ConfigureAwait(false);
        }
      }
      else if (response is IPageResult<ITenantEntity<TEntityIdentity, TTenantIdentity>> tenantEntitiesPageResult)
      {
        foreach (var currentTenantEntity in tenantEntitiesPageResult.Items)
        {
          await InspectTenantEntityAsync(tenantId, currentTenantEntity, cancellationToken).ConfigureAwait(false);
        }
      }
      else
      {
        throw new InvalidOperationException($"Unknown response type {response?.GetType()?.FullName}");
      }
    }
  }

  private async Task InspectTenantCommandAsync(
    ITenantCommand<TEntityIdentity, TTenantIdentity> tenantCommand,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();

    if (tenantCommand.TenantID == null || !tenantCommand.TenantID.Equals(tenantId))
    {
      throw new CrossTenantInboundUnauthorizedAccessException();
    }

    await InspectAffectingTenantEntitiesAsync(
      tenantCommand.AffectingTenantEntities,
      tenantId,
      cancellationToken).ConfigureAwait(false);
  }

  private static Task InspectTenantEntityAsync(
    TTenantIdentity tenantId,
    ITenantEntity<TEntityIdentity, TTenantIdentity> tenantEntity,
    CancellationToken cancellationToken)
  {
    if (tenantEntity.TenantID == null || !tenantEntity.TenantID.Equals(tenantId))
    {
      throw new CrossTenantOutboundUnauthorizedAccessException();
    }

    cancellationToken.ThrowIfCancellationRequested();

    return Task.CompletedTask;
  }

  private async Task InspectTenantQueryAsync(
    ITenantQuery<TEntityIdentity, TTenantIdentity, TResponse> tenantQuery,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();

    if (tenantQuery.TenantID == null || !tenantQuery.TenantID.Equals(tenantId))
    {
      throw new CrossTenantInboundUnauthorizedAccessException();
    }

    await InspectAffectingTenantEntitiesAsync(
      tenantQuery.AffectingTenantEntities,
      tenantId,
      cancellationToken).ConfigureAwait(false);
  }
}
