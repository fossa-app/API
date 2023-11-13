using System.Security.Cryptography.X509Certificates;
using Fossa.API.Core.Properties;

namespace Fossa.API.Core.Services;

public class CertificateProvider : ICertificateProvider
{
  public Task<X509Certificate2> GetCertificateAsync(CancellationToken cancellationToken)
  {
    var certificate = new X509Certificate2(Resources.FossaFirstPartyLicensing);
    return Task.FromResult(certificate);
  }
}
