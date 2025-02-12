using Asp.Versioning;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIKSN.Mapping;

namespace Fossa.API.Web.Api;

[Authorize]
[ApiVersion(1.0)]
[Route("api/{version:apiVersion}/[controller]")]
[ApiController]
public class EmployeeController : BaseApiController<EmployeeId>
{
  private readonly ITenantIdProvider<Guid> _tenantIdProvider;
  private readonly IUserIdProvider<Guid> _userIdProvider;

  public EmployeeController(
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    ISender sender,
    IPublisher publisher,
    IMapper<EmployeeId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, EmployeeId> dataIdentityToDomainIdentityMapper)
    : base(sender, publisher, domainIdentityToDataIdentityMapper, dataIdentityToDomainIdentityMapper)
  {
    _tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
    _userIdProvider = userIdProvider ?? throw new ArgumentNullException(nameof(userIdProvider));
  }

  [HttpDelete]
  public async Task DeleteAsync(
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    await _sender.Send(
      new EmployeeDeletionCommand(
        tenantId,
        userId),
      cancellationToken);
  }

  [HttpGet]
  public async Task<EmployeeRetrievalModel> GetAsync(
    [FromServices] IMapper<EmployeeEntity, EmployeeRetrievalModel> mapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    var entity = await _sender.Send(
      new CurrentEmployeeRetrievalQuery(tenantId, userId),
      cancellationToken);

    return mapper.Map(entity);
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
        tenantId,
        userId,
        model.FirstName ?? string.Empty,
        model.LastName ?? string.Empty,
        model.FullName ?? string.Empty),
      cancellationToken);
  }

  [HttpPut]
  public async Task PutAsync(
    [FromBody] EmployeeModificationModel model,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    await _sender.Send(
      new EmployeeModificationCommand(
        tenantId,
        userId,
        model.FirstName ?? string.Empty,
        model.LastName ?? string.Empty,
        model.FullName ?? string.Empty),
      cancellationToken);
  }
}
