using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.TimeZone;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class BranchCreationApiCommandHandler :
    ApiMessageHandler<BranchId, BranchCreationApiCommand, Unit, BranchCreationCommand, Unit>
{
  private readonly IMapper<AddressModel, Address> _addressModelToDomainMapper;
  private readonly IDateTimeZoneProvider _dateTimeZoneProvider;

  public BranchCreationApiCommandHandler(
        IDateTimeZoneProvider dateTimeZoneProvider,
        IMapper<AddressModel, Address> addressModelToDomainMapper,
        ISender sender,
        ITenantIdProvider<Guid> tenantIdProvider,
        IUserIdProvider<Guid> userIdProvider,
        IMapper<BranchId, long> domainIdentityToDataIdentityMapper,
        IMapper<long, BranchId> dataIdentityToDomainIdentityMapper) : base(
            sender,
            tenantIdProvider,
            userIdProvider,
            domainIdentityToDataIdentityMapper,
            dataIdentityToDomainIdentityMapper)
  {
    _dateTimeZoneProvider = dateTimeZoneProvider ?? throw new ArgumentNullException(nameof(dateTimeZoneProvider));
    _addressModelToDomainMapper = addressModelToDomainMapper ?? throw new ArgumentNullException(nameof(addressModelToDomainMapper));
  }

  protected override Unit MapToApiResponse(Unit domainResponse) => domainResponse;

  protected override BranchCreationCommand MapToDomainRequest(BranchCreationApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    var timeZone = _dateTimeZoneProvider.GetDateTimeZoneById(apiRequest.TimeZoneId ?? string.Empty);

    return new BranchCreationCommand(
        tenantId,
        userId,
        apiRequest.Name ?? string.Empty,
        timeZone,
        Optional(apiRequest.Address).Map(_addressModelToDomainMapper.Map));
  }
}
