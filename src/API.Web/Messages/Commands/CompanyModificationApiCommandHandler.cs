using FluentValidation;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Globalization;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class CompanyModificationApiCommandHandler : ApiMessageHandler<CompanyId, CompanyModificationApiCommand, Unit, CompanyModificationCommand, Unit>
{
  private readonly ICountryFactory _countryFactory;

  public CompanyModificationApiCommandHandler(
    ICountryFactory countryFactory,
    ISender sender,
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    IMapper<CompanyId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> dataIdentityToDomainIdentityMapper) : base(
      sender,
      tenantIdProvider,
      userIdProvider,
      domainIdentityToDataIdentityMapper,
      dataIdentityToDomainIdentityMapper)
  {
    _countryFactory = countryFactory ?? throw new ArgumentNullException(nameof(countryFactory));
  }

  protected override Unit MapToApiResponse(Unit domainResponse) => domainResponse;

  protected override CompanyModificationCommand MapToDomainRequest(CompanyModificationApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    return new CompanyModificationCommand(
      tenantId,
      apiRequest.Name ?? string.Empty,
      CreateCountry(apiRequest.CountryCode ?? string.Empty));
  }

  private CountryInfo CreateCountry(string countryCode)
  {
    if (_countryFactory.TryCreate(countryCode, out var country))
    {
      return country;
    }

    throw new ValidationException($"Country code '{countryCode}' is invalid.");
  }
}
