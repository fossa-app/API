using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Fossa.API.Core.Services;

namespace Fossa.API.FunctionalTests.Services;

public class TestCertificateProvider : ICertificateProvider, ITestCertificateProvider
{
  public X509Certificate2 Certificate { get; } = GenerateSelfSignedCertificate();

  public Task<X509Certificate2> GetCertificateAsync(CancellationToken cancellationToken)
  {
    return Task.FromResult(Certificate);
  }

  private static X509Certificate2 GenerateSelfSignedCertificate()
  {
    const string subjectName = "Self-Signed-Cert-TEST";

    var rsa = RSA.Create();
    var certRequest =
      new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

    certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));

    var generatedCert = certRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10));

    return generatedCert;
  }
}
