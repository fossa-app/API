using Asp.Versioning;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIKSN.Data;
using TIKSN.Mapping;

namespace Fossa.API.Web.Api;

[Authorize]
[ApiVersion(1.0)]
[Route("api/{version:apiVersion}/[controller]")]
[ApiController]
public class EmployeesController : BaseApiController<EmployeeId>
{
  private readonly ITenantIdProvider<Guid> _tenantIdProvider;
  private readonly IUserIdProvider<Guid> _userIdProvider;

  public EmployeesController(
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

  [HttpGet]
  public async Task<PagingResponseModel<EmployeeRetrievalModel>> PageAsync(
    [FromQuery] EmployeePagingRequestModel requestModel,
    [FromServices] IMapper<PageResult<EmployeeEntity>, PagingResponseModel<EmployeeRetrievalModel>> mapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    var result = await _sender.Send(
      new EmployeePagingQuery(tenantId, userId, new Page(requestModel.PageNumber, requestModel.PageSize)),
      cancellationToken);

    return mapper.Map(result);
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
        tenantId, userId, model.FirstName, model.LastName, model.FullName),
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
        _dataIdentityToDomainIdentityMapper.Map(id), tenantId, userId, model.FirstName, model.LastName, model.FullName),
      cancellationToken);
  }
}
