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
public class DepartmentsController : BaseApiController<DepartmentId>
{
  private readonly ITenantIdProvider<Guid> _tenantIdProvider;
  private readonly IUserIdProvider<Guid> _userIdProvider;

  public DepartmentsController(
      ITenantIdProvider<Guid> tenantIdProvider,
      IUserIdProvider<Guid> userIdProvider,
      ISender sender,
      IPublisher publisher,
      IMapper<DepartmentId, long> domainIdentityToDataIdentityMapper,
      IMapper<long, DepartmentId> dataIdentityToDomainIdentityMapper)
      : base(sender, publisher, domainIdentityToDataIdentityMapper, dataIdentityToDomainIdentityMapper)
  {
    _tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
    _userIdProvider = userIdProvider ?? throw new ArgumentNullException(nameof(userIdProvider));
  }

  [HttpDelete("{id}")]
  [Authorize(Roles = Roles.Administrator)]
  public async Task DeleteAsync(
      long id,
      CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    await _sender.Send(
        new DepartmentDeletionCommand(
            _dataIdentityToDomainIdentityMapper.Map(id),
            tenantId,
            userId),
        cancellationToken);
  }

  [HttpGet("{id}")]
  public async Task<DepartmentRetrievalModel> GetAsync(
      [FromRoute] long id,
      [FromServices] IMapper<DepartmentEntity, DepartmentRetrievalModel> mapper,
      CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    var entity = await _sender.Send(
        new DepartmentRetrievalQuery(
            _dataIdentityToDomainIdentityMapper.Map(id),
            tenantId,
            userId),
        cancellationToken);

    return mapper.Map(entity);
  }

  [HttpPost]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PostAsync(
      [FromBody] DepartmentModificationModel model,
      [FromServices] IMapper<long, EmployeeId> employeeDataIdentityToDomainIdentityMapper,
      CancellationToken cancellationToken)
  {
    if (model.ManagerId == null)
    {
      throw new ArgumentNullException(nameof(model), "ManagerId cannot be null.");
    }
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    await _sender.Send(
        new DepartmentCreationCommand(
            tenantId,
            userId,
            model.Name ?? string.Empty,
            Optional(model.ParentDepartmentId).Map(_dataIdentityToDomainIdentityMapper.Map),
            employeeDataIdentityToDomainIdentityMapper.Map(model.ManagerId.Value)),
        cancellationToken);
  }

  [HttpPut("{id}")]
  [Authorize(Roles = Roles.Administrator)]
    public async Task PutAsync(
        long id,
        [FromBody] DepartmentModificationModel model,
        [FromServices] IMapper<long, EmployeeId> employeeDataIdentityToDomainIdentityMapper,
        CancellationToken cancellationToken)
    {
        if (model.ManagerId == null)
        {
            throw new ArgumentNullException(nameof(model), "ManagerId cannot be null.");
        }
        var tenantId = _tenantIdProvider.GetTenantId();
        var userId = _userIdProvider.GetUserId();
        await _sender.Send(
            new DepartmentModificationCommand(
                _dataIdentityToDomainIdentityMapper.Map(id),
                tenantId,
                userId,
                model.Name ?? string.Empty,
                Optional(model.ParentDepartmentId).Map(_dataIdentityToDomainIdentityMapper.Map),
                employeeDataIdentityToDomainIdentityMapper.Map(model.ManagerId.Value)),
            cancellationToken);
    }

  [HttpGet]
  public Task<PagingResponseModel<DepartmentRetrievalModel>> QueryAsync(
      [FromQuery] DepartmentQueryRequestModel requestModel,
      [FromServices] IMapper<PageResult<DepartmentEntity>, PagingResponseModel<DepartmentRetrievalModel>> pagingMapper,
      [FromServices] IMapper<Seq<DepartmentEntity>, PagingResponseModel<DepartmentRetrievalModel>> listingMapper,
      CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();

    return (requestModel.Id ?? [])
        .ToSeq()
        .Map(_dataIdentityToDomainIdentityMapper.Map)
        .Match<Either<DepartmentListingQuery, DepartmentPagingQuery>>(
            () => new DepartmentPagingQuery(
                tenantId,
                userId,
                requestModel.Search ?? string.Empty,
                new Page(
                    requestModel.PageNumber ?? 0,
                    requestModel.PageSize ?? 0)),
            ids => new DepartmentListingQuery(
                ids,
                tenantId,
                userId))
        .Match<EitherAsync<Seq<DepartmentEntity>, PageResult<DepartmentEntity>>>(
            query => _sender.Send(query, cancellationToken),
            query => _sender.Send(query, cancellationToken))
        .BiMap(pagingMapper.Map, listingMapper.Map)
        .Match(x => x, x => x);
  }
}
