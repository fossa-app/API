using Asp.Versioning;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Web.ApiModels;
using Fossa.API.Web.Messages.Commands;
using Fossa.API.Web.Messages.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fossa.API.Web.Api;

[Authorize]
[ApiVersion(1.0)]
[Route("api/{version:apiVersion}/[controller]")]
[ApiController]
public class BranchesController : BaseApiController
{
  public BranchesController(
    ISender sender,
    IPublisher publisher)
    : base(sender, publisher)
  {
  }

  [HttpDelete("{id}")]
  [Authorize(Roles = Roles.Administrator)]
  public async Task DeleteAsync(
    long id,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    await _sender.Send(
      new BranchDeletionCommand(
        _dataIdentityToDomainIdentityMapper.Map(id),
        tenantId,
        userId),
      cancellationToken);
  }

  [HttpGet("{id}")]
  public async Task<BranchRetrievalModel> GetAsync(
    [FromRoute] long id,
    CancellationToken cancellationToken)
  {
    return await _sender.Send(
      new BranchRetrievalApiQuery(id),
      cancellationToken);
  }

  [HttpPost]
  [Authorize(Roles = Roles.Administrator)]
  public Task PostAsync(
    [FromBody] BranchModificationModel model,
    CancellationToken cancellationToken)
  {
    return _sender.Send(
      new BranchCreationApiCommand(
        model.Name,
        model.TimeZoneId,
        model.Address),
      cancellationToken);
  }

  [HttpPut("{id}")]
  [Authorize(Roles = Roles.Administrator)]
  public Task PutAsync(
    long id,
    [FromBody] BranchModificationModel model,
    CancellationToken cancellationToken)
  {
    return _sender.Send(
      new BranchModificationApiCommand(
        id,
        model.Name,
        model.TimeZoneId,
        model.Address),
      cancellationToken);
  }

  [HttpGet]
  public Task<PagingResponseModel<BranchRetrievalModel>> QueryAsync(
    [FromQuery] BranchQueryRequestModel requestModel,
    CancellationToken cancellationToken)
  {
    return _sender.Send(
      new BranchPagingApiQuery(
        requestModel.Id,
        requestModel.Search,
        requestModel.PageNumber,
        requestModel.PageSize),
      cancellationToken);
  }
}
