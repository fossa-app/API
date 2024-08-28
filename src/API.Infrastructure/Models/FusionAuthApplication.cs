using System.Text.Json.Serialization;

namespace Fossa.API.Infrastructure.Models;

public partial class FusionAuthApplication
{
  [JsonPropertyName("active")]
  public bool Active { get; set; }

  [JsonPropertyName("id")]
  public Guid Id { get; set; }

  [JsonPropertyName("insertInstant")]
  public long InsertInstant { get; set; }

  [JsonPropertyName("lastUpdateInstant")]
  public long LastUpdateInstant { get; set; }

  [JsonPropertyName("name")]
  public string? Name { get; set; }

  [JsonPropertyName("oauthConfiguration")]
  public FusionAuthApplicationOauthConfiguration? OauthConfiguration { get; set; }

  [JsonPropertyName("scopes")]
  public string[]? Scopes { get; set; }

  [JsonPropertyName("state")]
  public string? State { get; set; }

  [JsonPropertyName("tenantId")]
  public Guid TenantId { get; set; }

  [JsonPropertyName("verifyRegistration")]
  public bool VerifyRegistration { get; set; }

  [JsonPropertyName("verificationStrategy")]
  public string? VerificationStrategy { get; set; }
}
