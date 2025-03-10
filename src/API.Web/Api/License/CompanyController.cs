﻿using Asp.Versioning;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Web.ApiModels;
using Fossa.Licensing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIKSN.Licensing;
using TIKSN.Mapping;

namespace Fossa.API.Web.Api.License;

[Tags("Company License")]
[Authorize]
[ApiVersion(1.0)]
[Route("api/{version:apiVersion}/License/[controller]")]
[ApiController]
public class CompanyController : BaseApiController
{
  private readonly IMapper<License<CompanyEntitlements>, LicenseResponseModel<CompanyEntitlementsModel>> _licenseMapper;
  private readonly ITenantIdProvider<Guid> _tenantIdProvider;

  public CompanyController(
    ISender sender,
    IPublisher publisher,
    IMapper<License<CompanyEntitlements>, LicenseResponseModel<CompanyEntitlementsModel>> licenseMapper,
    ITenantIdProvider<Guid> tenantIdProvider)
    : base(sender, publisher)
  {
    _licenseMapper = licenseMapper ?? throw new ArgumentNullException(nameof(licenseMapper));
    _tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
  }

  [HttpPost]
  [Authorize(Roles = Roles.Administrator)]
  public async Task CreateAsync(
    IFormFile licenseFile,
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();

    await using var memoryStream = new MemoryStream();
    await licenseFile.CopyToAsync(memoryStream, cancellationToken);

    var licenseData = memoryStream.ToArray().ToSeq();

    await _sender.Send(
      new CompanyLicenseCreationCommand(
        tenantId,
        licenseData),
      cancellationToken);
  }

  [HttpGet]
  public async Task<LicenseResponseModel<CompanyEntitlementsModel>> GetAsync(
    CancellationToken cancellationToken)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var license = await _sender.Send(
      new CompanyLicenseRetrievalQuery(
        tenantId),
      cancellationToken);

    var model = _licenseMapper.Map(license);
    return model;
  }
}
