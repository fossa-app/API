using System.Net.Http.Json;
using Fossa.API.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Infrastructure.RestClients;

public partial class FusionAuthRestClient : IFusionAuthRestClient
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<FusionAuthRestClient> _logger;

  public FusionAuthRestClient(
    HttpClient httpClient,
    ILogger<FusionAuthRestClient> logger)
  {
    _httpClient = httpClient;
    _logger = logger;
  }

  [LoggerMessage(EventId = 20786118, Level = LogLevel.Information, Message = "Retrieved `{ApplicationCount}` applications for `{TenantCount}` tenants")]
  public static partial void LogRetrievedApplications(ILogger logger, int applicationCount, int tenantCount);

  public async Task<FusionAuthApplicationsListingResponse?> GetApplicationsAsync(CancellationToken cancellationToken)
  {
    var applicationsListingResponse = await _httpClient.GetFromJsonAsync<FusionAuthApplicationsListingResponse>("api/application", cancellationToken).ConfigureAwait(false);

    int applicationCount = applicationsListingResponse?.Applications?.Length ?? 0;
    int tenantCount = applicationsListingResponse?.Applications?.DistinctBy(x => x.TenantId).Count() ?? 0;
    LogRetrievedApplications(_logger, applicationCount, tenantCount);

    return applicationsListingResponse;
  }
}
