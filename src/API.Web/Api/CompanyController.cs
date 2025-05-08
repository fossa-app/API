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
public class CompanyController : BaseApiController
{
  public CompanyController(
    ISender sender,
    IPublisher publisher)
    : base(sender, publisher)
  {
  }

  [HttpDelete]
  [Authorize(Roles = Roles.Administrator)]
  public async Task DeleteAsync(
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new CompanyDeletionApiCommand(),
      cancellationToken);
  }

  [HttpGet]
  public async Task<CompanyRetrievalModel> GetAsync(
    CancellationToken cancellationToken)
  {
    return await _sender.Send(
      new CompanyRetrievalApiQuery(),
      cancellationToken);
  }

  [HttpPost]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PostAsync(
    [FromBody] CompanyModificationModel model,
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new CompanyCreationApiCommand(model.Name, model.CountryCode),
      cancellationToken);
  }

  [HttpPut]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PutAsync(
    [FromBody] CompanyModificationModel model,
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new CompanyModificationApiCommand(
        model.Name,
        model.CountryCode),
      cancellationToken);
  }
}
