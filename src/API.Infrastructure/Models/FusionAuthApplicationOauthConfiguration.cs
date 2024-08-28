using System.Text.Json.Serialization;

namespace Fossa.API.Infrastructure.Models;

public partial class FusionAuthApplicationOauthConfiguration
{
  [JsonPropertyName("authorizedOriginURLs")]
  public string[]? AuthorizedOriginUrLs { get; set; }

  [JsonPropertyName("authorizedRedirectURLs")]
  public string[]? AuthorizedRedirectUrLs { get; set; }

  [JsonPropertyName("authorizedURLValidationPolicy")]
  public string? AuthorizedUrlValidationPolicy { get; set; }

  [JsonPropertyName("clientAuthenticationPolicy")]
  public string? ClientAuthenticationPolicy { get; set; }

  [JsonPropertyName("clientId")]
  public Guid ClientId { get; set; }

  [JsonPropertyName("clientSecret")]
  public string? ClientSecret { get; set; }

  [JsonPropertyName("consentMode")]
  public string? ConsentMode { get; set; }

  [JsonPropertyName("debug")]
  public bool Debug { get; set; }

  [JsonPropertyName("enabledGrants")]
  public string[]? EnabledGrants { get; set; }

  [JsonPropertyName("generateRefreshTokens")]
  public bool GenerateRefreshTokens { get; set; }

  [JsonPropertyName("logoutBehavior")]
  public string? LogoutBehavior { get; set; }

  [JsonPropertyName("proofKeyForCodeExchangePolicy")]
  public string? ProofKeyForCodeExchangePolicy { get; set; }

  [JsonPropertyName("relationship")]
  public string? Relationship { get; set; }

  [JsonPropertyName("requireClientAuthentication")]
  public bool RequireClientAuthentication { get; set; }

  [JsonPropertyName("requireRegistration")]
  public bool RequireRegistration { get; set; }

  [JsonPropertyName("scopeHandlingPolicy")]
  public string? ScopeHandlingPolicy { get; set; }

  [JsonPropertyName("unknownScopePolicy")]
  public string? UnknownScopePolicy { get; set; }

  [JsonPropertyName("logoutURL")]
  public string? LogoutUrl { get; set; }
}
