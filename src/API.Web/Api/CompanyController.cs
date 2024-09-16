using Asp.Versioning;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Web.ApiModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

  [HttpGet]
  public async Task<CompanyRetrievalModel> GetAsync(
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var entity = await _sender.Send(
      new CompanyRetrievalQuery(tenantId),
      cancellationToken);

    return new CompanyRetrievalModel(_domainIdentityToDataIdentityMapper.Map(entity.ID), entity.Name);
  }

  [HttpPost]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PostAsync(
    [FromBody] CompanyModificationModel model,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyCreationCommand(tenantId, model.Name ?? string.Empty),
      cancellationToken);
  }

  [HttpPut]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PutAsync(
    [FromBody] CompanyModificationModel model,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyModificationCommand(
        tenantId,
        model.Name ?? string.Empty),
      cancellationToken);
  }
}
