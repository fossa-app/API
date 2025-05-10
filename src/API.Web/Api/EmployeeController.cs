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
public class EmployeeController : BaseApiController
{
  public EmployeeController(
    ISender sender,
    IPublisher publisher)
    : base(sender, publisher)
  {
  }

  [HttpDelete]
  public async Task DeleteAsync(
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new EmployeeDeletionApiCommand(),
      cancellationToken);
  }

  [HttpGet]
  public async Task<EmployeeRetrievalModel> GetAsync(
    CancellationToken cancellationToken)
  {
    return await _sender.Send(
      new CurrentEmployeeRetrievalApiQuery(),
      cancellationToken);
  }

  [HttpPost]
  public async Task PostAsync(
    [FromBody] EmployeeModificationModel model,
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new EmployeeCreationApiCommand(
        model.FirstName,
        model.LastName,
        model.FullName),
      cancellationToken);
  }

  [HttpPut]
  public async Task PutAsync(
    [FromBody] EmployeeModificationModel model,
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new EmployeeModificationApiCommand(
        model.FirstName,
        model.LastName,
        model.FullName),
      cancellationToken);
  }
}
