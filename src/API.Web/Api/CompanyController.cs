using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Web.ApiModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fossa.API.Web.Api;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CompanyController : BaseApiController
{
  private readonly ITenantIdProvider<Guid> _tenantIdProvider;

  public CompanyController(
    ITenantIdProvider<Guid> tenantIdProvider,
    ISender sender,
    IPublisher publisher) : base(sender, publisher)
  {
    _tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
  }

  [HttpDelete("{id}")]
  public async Task DeleteAsync(long id, CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyDeletionCommand(id, tenantId),
      cancellationToken);
  }

  [HttpGet]
  public async Task<CompanyEntity> GetAsync(
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    return await _sender.Send(
      new CompanyRetrievalQuery(tenantId),
      cancellationToken);
  }

  [HttpPost]
  public async Task PostAsync(
    [FromBody] CompanyModel model,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyCreationCommand(tenantId, model.Name),
      cancellationToken);
  }

  [HttpPut("{id}")]
  public async Task PutAsync(
    long id,
    [FromBody] CompanyModel model,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyModificationCommand(id, tenantId, model.Name),
      cancellationToken);
  }
}
