using Asp.Versioning;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
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

  [HttpGet("{id}")]
  public async Task<EmployeeRetrievalModel> GetAsync(
    [FromRoute] long id,
    [FromServices] IMapper<EmployeeEntity, EmployeeRetrievalModel> mapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    var entity = await _sender.Send(
      new EmployeeRetrievalQuery(
        _dataIdentityToDomainIdentityMapper.Map(id),
        tenantId,
        userId),
      cancellationToken);

    return mapper.Map(entity);
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
      new EmployeePagingQuery(tenantId, userId, requestModel.Search ?? string.Empty,
        new Page(
        requestModel.PageNumber ?? 0,
        requestModel.PageSize ?? 0)),
      cancellationToken);

    return mapper.Map(result);
  }

  [HttpPut]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PutAsync(
    [FromBody] EmployeeManagementModel model,
    [FromServices] IMapper<long, BranchId> branchDataIdentityToDomainIdentityMapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    await _sender.Send(
      new EmployeeManagementCommand(
        tenantId,
        userId,
        Optional(model.AssignedBranchId).Map(branchDataIdentityToDomainIdentityMapper.Map)),
      cancellationToken);
  }
}
