namespace Fossa.API.Infrastructure.Models;

public record IdentityClient(
  Guid ClientId,
  string ClientName,
  Guid TenantId);
