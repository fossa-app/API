using Fossa.API.Core.Messages.Queries;
using Fossa.API.Web.ApiModels;
using Fossa.Licensing;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TIKSN.Licensing;
using TIKSN.Mapping;

namespace Fossa.API.Web.Api.License;

[Tags("System License")]
[Route("api/License/[controller]")]
[ApiController]
public class SystemController : BaseApiController
{
  private readonly IMapper<License<SystemEntitlements>, LicenseResponseModel<SystemEntitlementsModel>> _licenseMapper;

  public SystemController(
    ISender sender,
    IPublisher publisher,
    IMapper<License<SystemEntitlements>, LicenseResponseModel<SystemEntitlementsModel>> licenseMapper)
    : base(sender, publisher)
  {
    _licenseMapper = licenseMapper ?? throw new ArgumentNullException(nameof(licenseMapper));
  }

  [HttpGet]
  public async Task<LicenseResponseModel<SystemEntitlementsModel>> GetAsync(
    CancellationToken cancellationToken)
  {
    var license = await _sender.Send(
      new SystemLicenseRetrievalQuery(),
      cancellationToken);

    var model = _licenseMapper.Map(license);
    return model;
  }
}
