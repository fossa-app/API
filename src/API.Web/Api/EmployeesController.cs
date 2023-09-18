using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fossa.API.Web.Api;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : BaseApiController
{
  private readonly ITenantIdProvider<Guid> _tenantIdProvider;
  private readonly IUserIdProvider<Guid> _userIdProvider;

  public EmployeesController(
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    ISender sender,
    IPublisher publisher) : base(sender, publisher)
  {
    _tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
    _userIdProvider = userIdProvider ?? throw new ArgumentNullException(nameof(userIdProvider));
  }

  [HttpPost]
  public async Task PostAsync(
    [FromBody] EmployeeModificationModel model,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    await _sender.Send(
      new EmployeeCreationCommand(
        tenantId, userId,model.FirstName, model.LastName, model.FullName),
      cancellationToken);
  }

  [HttpPut("{id}")]
  public async Task PutAsync(
    long id,
    [FromBody] EmployeeModificationModel model,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    await _sender.Send(
      new EmployeeModificationCommand(
        id, tenantId, userId,model.FirstName, model.LastName, model.FullName),
      cancellationToken);
  }
}
