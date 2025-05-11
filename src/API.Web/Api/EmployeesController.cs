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
public class EmployeesController : BaseApiController
{
  public EmployeesController(
    ISender sender,
    IPublisher publisher)
    : base(sender, publisher)
  {
  }

  [HttpGet("{id}")]
  public async Task<EmployeeRetrievalModel> GetAsync(
    [FromRoute] long id,
    CancellationToken cancellationToken)
  {
    return await _sender.Send(
      new EmployeeRetrievalApiQuery(id),
      cancellationToken);
  }

  [HttpPut("{id}")]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PutAsync(
    [FromRoute] long id,
    [FromBody] EmployeeManagementModel model,
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new EmployeeManagementApiCommand(
        id,
        model.AssignedBranchId,
        model.AssignedDepartmentId),
      cancellationToken);
  }

  [HttpGet]
  public async Task<PagingResponseModel<EmployeeRetrievalModel>> QueryAsync(
    [FromQuery] EmployeeQueryRequestModel requestModel,
    CancellationToken cancellationToken)
  {
    return await _sender.Send(
      new EmployeePagingApiQuery(
        requestModel.Id,
        requestModel.Search,
        requestModel.PageNumber,
        requestModel.PageSize),
      cancellationToken);
  }
}
