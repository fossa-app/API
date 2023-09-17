﻿using Fossa.API.Core.Entities;
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
  [Authorize(Roles = "administrator")]
  public async Task DeleteAsync(long id, CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyDeletionCommand(id, tenantId),
      cancellationToken);
  }

  [HttpGet]
  public async Task<CompanyRetrievalModel> GetAsync(
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var entity = await _sender.Send(
      new CompanyRetrievalQuery(tenantId),
      cancellationToken);

    return new CompanyRetrievalModel(entity.ID, entity.Name);
  }
  
  [HttpPost]
  [Authorize(Roles = "administrator")]
  public async Task PostAsync(
    [FromBody] CompanyModificationModel model,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyCreationCommand(tenantId, model.Name),
      cancellationToken);
  }
  
  [HttpPut("{id}")]
  [Authorize(Roles = "administrator")]
  public async Task PutAsync(
    long id,
    [FromBody] CompanyModificationModel model,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    await _sender.Send(
      new CompanyModificationCommand(id, tenantId, model.Name),
      cancellationToken);
  }
}
