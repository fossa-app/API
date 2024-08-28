using System.Text.Json.Serialization;

namespace Fossa.API.Infrastructure.Models;

public partial class FusionAuthApplicationsListingResponse
{
  [JsonPropertyName("applications")]
  public FusionAuthApplication[]? Applications { get; set; }
}
