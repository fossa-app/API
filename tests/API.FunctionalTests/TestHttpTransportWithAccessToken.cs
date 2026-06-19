using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fossa.API.Web;
using Fossa.Bridge.Models.ApiModels;
using Fossa.Bridge.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Fossa.API.FunctionalTests;

public class TestHttpTransportWithAccessToken : IHttpTransport, IAccessTokenProvider, IAccessTokenContext
{
  private readonly HttpClient _httpClient;
  private Option<string> _accessToken;

  public TestHttpTransportWithAccessToken(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    ArgumentNullException.ThrowIfNull(factory);
    _httpClient = factory.CreateClient();
  }

  public void ClearAccessToken()
  {
    _accessToken = None;
  }

  public async Task<ClientUnitResult> DeleteAsync(
    string endpointUrl,
    EndpointSecurity endpointSecurity,
    CancellationToken cancellationToken)
  {
    var requestUri = NormalizeRequestUri(endpointUrl);
    ApplyAccessToken(endpointSecurity);
    var response = await _httpClient.DeleteAsync(requestUri, cancellationToken);
    if (!response.IsSuccessStatusCode)
    {
      return ClientUnitResult.NewFailure(await ReadProblemDetailsAsync(response, cancellationToken));
    }

    return ClientUnitResult.Success;
  }

  public async Task<ClientResult<TResponse>> GetAsync<TResponse>(
    string endpointUrl,
    EndpointSecurity endpointSecurity,
    CancellationToken cancellationToken)
    where TResponse : class
  {
    var requestUri = NormalizeRequestUri(endpointUrl);
    ApplyAccessToken(endpointSecurity);
    var response = await _httpClient.GetAsync(requestUri, cancellationToken);
    if (!response.IsSuccessStatusCode)
    {
      return ClientResult<TResponse>.NewFailure(await ReadProblemDetailsAsync(response, cancellationToken));
    }

    var result = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    return ClientResult<TResponse>.NewSuccess(result ?? throw new InvalidOperationException("Response content is null."));
  }

  public Task<string> GetTokenAsync(CancellationToken cancellationToken)
    => _accessToken.Match(
      token => Task.FromResult(token),
      () => throw new InvalidOperationException("Access token is not available.")
    );

  public async Task<ClientUnitResult> PatchAsync<TRequest>(
    string endpointUrl,
    EndpointSecurity endpointSecurity,
    TRequest request,
    CancellationToken cancellationToken)
    where TRequest : notnull
  {
    var requestUri = NormalizeRequestUri(endpointUrl);
    ApplyAccessToken(endpointSecurity);
    var response = await _httpClient.PatchAsJsonAsync(requestUri, request, cancellationToken);
    if (!response.IsSuccessStatusCode)
    {
      return ClientUnitResult.NewFailure(await ReadProblemDetailsAsync(response, cancellationToken));
    }

    return ClientUnitResult.Success;
  }

  public async Task<ClientUnitResult> PostAsync<TRequest>(
    string endpointUrl,
    EndpointSecurity endpointSecurity,
    TRequest request,
    CancellationToken cancellationToken)
    where TRequest : notnull
  {
    var requestUri = NormalizeRequestUri(endpointUrl);
    ApplyAccessToken(endpointSecurity);
    var response = await _httpClient.PostAsJsonAsync(requestUri, request, cancellationToken);
    if (!response.IsSuccessStatusCode)
    {
      return ClientUnitResult.NewFailure(await ReadProblemDetailsAsync(response, cancellationToken));
    }

    return ClientUnitResult.Success;
  }

  public async Task<ClientUnitResult> PutAsync<TRequest>(
    string endpointUrl,
    EndpointSecurity endpointSecurity,
    TRequest request,
    CancellationToken cancellationToken)
    where TRequest : notnull
  {
    var requestUri = NormalizeRequestUri(endpointUrl);
    ApplyAccessToken(endpointSecurity);
    var response = await _httpClient.PutAsJsonAsync(requestUri, request, cancellationToken);
    if (!response.IsSuccessStatusCode)
    {
      return ClientUnitResult.NewFailure(await ReadProblemDetailsAsync(response, cancellationToken));
    }

    return ClientUnitResult.Success;
  }

  public void SetAccessToken(string accessToken)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
    _accessToken = Some(accessToken);
  }

  private static string NormalizeRequestUri(string endpointUrl)
    => endpointUrl.StartsWith('/') ? endpointUrl : "/" + endpointUrl;

  private static async Task<ProblemDetailsModel> ReadProblemDetailsAsync(
    System.Net.Http.HttpResponseMessage response,
    CancellationToken cancellationToken)
    => await response.Content.ReadFromJsonAsync<ProblemDetailsModel>(cancellationToken: cancellationToken)
      ?? throw new InvalidOperationException("Problem details response content is null.");

  private void ApplyAccessToken(EndpointSecurity endpointSecurity)
  {
    var endpointAccessToken = endpointSecurity.IsRequireToken ? _accessToken : None;
    endpointAccessToken.IfSome(token => _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token));
    endpointAccessToken.IfNone(() => _httpClient.DefaultRequestHeaders.Authorization = null);
  }
}
