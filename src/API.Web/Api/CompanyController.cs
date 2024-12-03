using Asp.Versioning;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Web.ApiModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIKSN.Globalization;
using TIKSN.Mapping;

namespace Fossa.API.Web.Api;

[Authorize]
[ApiVersion(1.0)]
[Route("api/{version:apiVersion}/[controller]")]
[ApiController]
public class CompanyController : BaseApiController<CompanyId>
{
  private readonly ITenantIdProvider<Guid> _tenantIdProvider;

  public CompanyController(
    ITenantIdProvider<Guid> tenantIdProvider,
    ISender sender,
    IPublisher publisher,
    IMapper<CompanyId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> dataIdentityToDomainIdentityMapper)
    : base(sender, publisher, domainIdentityToDataIdentityMapper, dataIdentityToDomainIdentityMapper)
  {
    _tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
  }

  [HttpDelete]
  [Authorize(Roles = Roles.Administrator)]
  public async Task DeleteAsync(
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyDeletionCommand(tenantId),
      cancellationToken);
  }

  [HttpGet]
  public async Task<CompanyRetrievalModel> GetAsync(
    [FromServices] IMapper<CompanyEntity, CompanyRetrievalModel> mapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var entity = await _sender.Send(
      new CompanyRetrievalQuery(tenantId),
      cancellationToken);

    return mapper.Map(entity);
  }

  [HttpPost]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PostAsync(
    [FromBody] CompanyModificationModel model,
    [FromServices] IRegionFactory regionFactory,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyCreationCommand(tenantId, model.Name ?? string.Empty,
      regionFactory.Create(model.CountryCode ?? string.Empty)),
      cancellationToken);
  }

  [HttpPut]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PutAsync(
    [FromBody] CompanyModificationModel model,
    [FromServices] IRegionFactory regionFactory,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyModificationCommand(
        tenantId,
        model.Name ?? string.Empty,
        regionFactory.Create(model.CountryCode ?? string.Empty)),
      cancellationToken);
  }
}
