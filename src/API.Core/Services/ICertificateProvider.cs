using System.Security.Cryptography.X509Certificates;

namespace Fossa.API.Core.Services;

public interface ICertificateProvider
{
  Task<X509Certificate2> GetCertificateAsync(CancellationToken cancellationToken);
}
