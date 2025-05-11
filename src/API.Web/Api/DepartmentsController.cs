using Asp.Versioning;
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
public class DepartmentsController : BaseApiController
{
  public DepartmentsController(
      ISender sender,
      IPublisher publisher)
      : base(sender, publisher)
  {
  }

  [HttpDelete("{id}")]
  [Authorize(Roles = Roles.Administrator)]
  public Task DeleteAsync(
      long id,
      CancellationToken cancellationToken)
  {
    return _sender.Send(
        new DepartmentDeletionApiCommand(id),
        cancellationToken);
  }

  [HttpGet("{id}")]
  public Task<DepartmentRetrievalModel> GetAsync(
        [FromRoute] long id,
        CancellationToken cancellationToken)
  {
    return _sender.Send(
        new DepartmentRetrievalApiQuery(id),
        cancellationToken);
  }

  [HttpPost]
  [Authorize(Roles = Roles.Administrator)]
  public Task PostAsync(
      [FromBody] DepartmentModificationModel model,
      CancellationToken cancellationToken)
  {
    return _sender.Send(
        new DepartmentCreationApiCommand(
            model.Name,
            model.ParentDepartmentId,
            model.ManagerId),
        cancellationToken);
  }

  [HttpPut("{id}")]
  [Authorize(Roles = Roles.Administrator)]
  public Task PutAsync(
      long id,
      [FromBody] DepartmentModificationModel model,
      CancellationToken cancellationToken)
  {
    return _sender.Send(
        new DepartmentModificationApiCommand(
            id,
            model.Name,
            model.ParentDepartmentId,
            model.ManagerId),
        cancellationToken);
  }

  [HttpGet]
  public Task<PagingResponseModel<DepartmentRetrievalModel>> QueryAsync(
      [FromQuery] DepartmentQueryRequestModel requestModel,
      CancellationToken cancellationToken)
  {
    return _sender.Send(
        new DepartmentPagingApiQuery(
            requestModel.Id,
            requestModel.Search,
            requestModel.PageNumber,
            requestModel.PageSize),
        cancellationToken);
  }
}
