using Asp.Versioning;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.TimeZone;
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
public class BranchesController : BaseApiController<BranchId>
{
  private readonly ITenantIdProvider<Guid> _tenantIdProvider;
  private readonly IUserIdProvider<Guid> _userIdProvider;

  public BranchesController(
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    ISender sender,
    IPublisher publisher,
    IMapper<BranchId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, BranchId> dataIdentityToDomainIdentityMapper)
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
      new BranchDeletionCommand(
        _dataIdentityToDomainIdentityMapper.Map(id),
        tenantId,
        userId),
      cancellationToken);
  }

  [HttpGet("{id}")]
  public async Task<BranchRetrievalModel> GetAsync(
    [FromRoute] long id,
    [FromServices] IMapper<BranchEntity, BranchRetrievalModel> mapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    var entity = await _sender.Send(
      new BranchRetrievalQuery(
        _dataIdentityToDomainIdentityMapper.Map(id),
        tenantId,
        userId),
      cancellationToken);

    return mapper.Map(entity);
  }

  [HttpPost]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PostAsync(
    [FromBody] BranchModificationModel model,
    [FromServices] IDateTimeZoneProvider dateTimeZoneProvider,
    [FromServices] IMapper<AddressModel, Address> addressModelToDomainMapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    var timeZone = dateTimeZoneProvider.GetDateTimeZoneById(model.TimeZoneId ?? string.Empty);
    await _sender.Send(
      new BranchCreationCommand(
        tenantId,
        userId,
        model.Name ?? string.Empty,
        timeZone,
        Optional(model.Address).Map(addressModelToDomainMapper.Map)),
      cancellationToken);
  }

  [HttpPut("{id}")]
  [Authorize(Roles = Roles.Administrator)]
  public async Task PutAsync(
    long id,
    [FromBody] BranchModificationModel model,
    [FromServices] IDateTimeZoneProvider dateTimeZoneProvider,
    [FromServices] IMapper<AddressModel, Address> addressModelToDomainMapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    var timeZone = dateTimeZoneProvider.GetDateTimeZoneById(model.TimeZoneId ?? string.Empty);
    await _sender.Send(
      new BranchModificationCommand(
        _dataIdentityToDomainIdentityMapper.Map(id),
        tenantId,
        userId,
        model.Name ?? string.Empty,
        timeZone,
        Optional(model.Address).Map(addressModelToDomainMapper.Map)),
      cancellationToken);
  }

  [HttpGet]
  public async Task<QueryResponseModel<BranchRetrievalModel>> QueryAsync(
    [FromQuery] BranchQueryRequestModel requestModel,
    [FromServices] IMapper<PageResult<BranchEntity>, PagingResponseModel<BranchRetrievalModel>> pagingMapper,
    [FromServices] IMapper<BranchEntity, BranchRetrievalModel> itemMapper,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();

    Either<BranchListingQuery, BranchPagingQuery> command = requestModel.Id?.Count switch
    {
      null or 0 => new BranchPagingQuery(
        tenantId,
        userId,
        requestModel.Search ?? string.Empty,
        new Page(
          requestModel.PageNumber ?? 0,
          requestModel.PageSize ?? 0)),
      _ => new BranchListingQuery(
        requestModel.Id.Select(_dataIdentityToDomainIdentityMapper.Map).ToSeq(),
        tenantId,
        userId),
    };

    var result = await command.Match<EitherAsync<Seq<BranchEntity>, PageResult<BranchEntity>>>(
      query => _sender.Send(query, cancellationToken),
      query => _sender.Send(query, cancellationToken))
      .BiMap(pagingMapper.Map, x => x.Map(itemMapper.Map))
      .Match(x => new QueryResponseModel<BranchRetrievalModel>(List: null, x), x => new QueryResponseModel<BranchRetrievalModel>(x, Page: null));

    return result;
  }
}
