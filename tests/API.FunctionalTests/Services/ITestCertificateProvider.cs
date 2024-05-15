using System.Security.Cryptography.X509Certificates;

namespace Fossa.API.FunctionalTests.Services;

public interface ITestCertificateProvider
{
  X509Certificate2 Certificate { get; }
}
