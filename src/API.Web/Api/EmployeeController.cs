using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIKSN.Mapping;

namespace Fossa.API.Web.Api;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EmployeeController : BaseApiController
{
  private readonly ITenantIdProvider<Guid> _tenantIdProvider;
  private readonly IUserIdProvider<Guid> _userIdProvider;

  public EmployeeController(
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    ISender sender,
    IPublisher publisher) : base(sender, publisher)
  {
    _tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
    _userIdProvider = userIdProvider ?? throw new ArgumentNullException(nameof(userIdProvider));
  }

  [HttpGet]
  public async Task<EmployeeRetrievalModel> GetAsync(
    [FromServices] IMapper<EmployeeEntity, EmployeeRetrievalModel> mapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    var entity = await _sender.Send(
      new EmployeeRetrievalQuery(tenantId, userId),
      cancellationToken);

    return mapper.Map(entity);
  }
}
