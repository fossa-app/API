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

  [HttpGet]
  public async Task<CompanySettingsRetrievalModel> GetAsync(
    CancellationToken cancellationToken)
  {
    return await _sender.Send(
      new CompanySettingsRetrievalApiQuery(),
      cancellationToken);
  }

  [HttpPut("{id}")]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PutAsync(
    [FromRoute] long id,
    [FromBody] CompanySettingsModificationModel model,
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new CompanySettingsModificationApiCommand(id, model.ColorSchemeId),
      cancellationToken);
  }

  [HttpDelete("{id}")]
  [Authorize(Roles = Roles.Administrator)]
  public async Task DeleteAsync(
    [FromRoute] long id,
    CancellationToken cancellationToken)
  {
    await _sender.Send(
      new CompanySettingsDeletionApiCommand(id),
      cancellationToken);
  }
}
