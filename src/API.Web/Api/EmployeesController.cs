using Asp.Versioning;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using Fossa.API.Web.Messages.Commands;
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
  public Task<PagingResponseModel<EmployeeRetrievalModel>> QueryAsync(
    [FromQuery] EmployeeQueryRequestModel requestModel,
    [FromServices] IMapper<PageResult<EmployeeEntity>, PagingResponseModel<EmployeeRetrievalModel>> pagingMapper,
    [FromServices] IMapper<Seq<EmployeeEntity>, PagingResponseModel<EmployeeRetrievalModel>> listingMapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();

    return (requestModel.Id ?? [])
      .ToSeq()
      .Map(_dataIdentityToDomainIdentityMapper.Map)
      .Match<Either<EmployeeListingQuery, EmployeePagingQuery>>(
        () => new EmployeePagingQuery(
          tenantId,
          userId,
          requestModel.Search ?? string.Empty,
          new Page(
            requestModel.PageNumber ?? 0,
            requestModel.PageSize ?? 0)),
        ids => new EmployeeListingQuery(
          ids,
          tenantId,
          userId))
      .Match<EitherAsync<Seq<EmployeeEntity>, PageResult<EmployeeEntity>>>(
        query => _sender.Send(query, cancellationToken),
        query => _sender.Send(query, cancellationToken))
      .BiMap(pagingMapper.Map, listingMapper.Map)
      .Match(x => x, x => x);
  }
}
