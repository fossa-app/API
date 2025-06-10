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
public class CompanySettingsController : BaseApiController
{
  public CompanySettingsController(
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
      new CompanySettingsDeletionApiCommand(),
      cancellationToken);
  }

  [HttpGet]
  public async Task<CompanySettingsRetrievalModel> GetAsync(
    CancellationToken cancellationToken)
  {
    return await _sender.Send(
      new CompanySettingsRetrievalApiQuery(),
      cancellationToken);
  }

  [HttpPost]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PostAsync(
    [FromBody] CompanySettingsModificationModel model,
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new CompanySettingsCreationApiCommand(model.ColorSchemeId),
      cancellationToken);
  }

  [HttpPut]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PutAsync(
    [FromBody] CompanySettingsModificationModel model,
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new CompanySettingsModificationApiCommand(model.ColorSchemeId),
      cancellationToken);
  }
}
