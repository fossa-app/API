using EasyDoubles;
using Fossa.API.Core.Repositories;
using TIKSN.Identity;

namespace Fossa.API.FunctionalTests.Repositories;

public class LicenseEasyFileRepository : EasyFileRepository<long, object>, ILicenseFileRepository
{
  public LicenseEasyFileRepository(
    IEasyStores easyStores,
    IIdentityGenerator<long> identityGenerator)
    : base(easyStores, identityGenerator, "License")
  {
  }
}
