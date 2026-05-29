using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Fossa.API.Web;
using Fossa.Bridge.Services;

namespace Fossa.API.FunctionalTests;

public class TestHttpTransport : IHttpTransport
{
  private readonly HttpClient _httpClient;

  public TestHttpTransport(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    ArgumentNullException.ThrowIfNull(factory);
    _httpClient = factory.CreateClient();
  }

  public void SetAuthorizationToken(string scheme, string parameter)
  {
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);
  }

  public async Task DeleteAsync(string requestUri, CancellationToken cancellationToken)
  {
    if (!requestUri.StartsWith('/')) requestUri = "/" + requestUri;
    var response = await _httpClient.DeleteAsync(requestUri, cancellationToken);
    if (!response.IsSuccessStatusCode)
    {
      var body = await response.Content.ReadAsStringAsync(cancellationToken);
      throw new HttpRequestException($"DELETE {requestUri} failed with {response.StatusCode}. Body: {body}", null, response.StatusCode);
    }
  }

  public Task DeleteAsync(string requestUri)
    => DeleteAsync(requestUri, TestContext.Current.CancellationToken);

  Task IHttpTransport.DeleteAsync(string endpointUrl, EndpointSecurity endpointSecurity, CancellationToken cancellationToken)
    => DeleteAsync(endpointUrl, cancellationToken);

  public async Task<TResponse> GetAsync<TResponse>(string requestUri, CancellationToken cancellationToken)
  {
    if (!requestUri.StartsWith('/')) requestUri = "/" + requestUri;
    var response = await _httpClient.GetAsync(requestUri, cancellationToken);
    if (!response.IsSuccessStatusCode)
    {
      var body = await response.Content.ReadAsStringAsync(cancellationToken);
      throw new HttpRequestException($"GET {requestUri} failed with {response.StatusCode}. Body: {body}", null, response.StatusCode);
    }
    var result = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    return result ?? throw new InvalidOperationException("Response content is null.");
  }

  public Task<TResponse> GetAsync<TResponse>(string requestUri)
    => GetAsync<TResponse>(requestUri, TestContext.Current.CancellationToken);

  Task<TResponse> IHttpTransport.GetAsync<TResponse>(string endpointUrl, EndpointSecurity endpointSecurity, CancellationToken cancellationToken)
    => GetAsync<TResponse>(endpointUrl, cancellationToken);

  public async Task PostAsync<TRequest>(string requestUri, TRequest request, CancellationToken cancellationToken)
  {
    if (!requestUri.StartsWith('/')) requestUri = "/" + requestUri;
    var response = await _httpClient.PostAsJsonAsync(requestUri, request, cancellationToken);
    if (!response.IsSuccessStatusCode)
    {
      var body = await response.Content.ReadAsStringAsync(cancellationToken);
      throw new HttpRequestException($"POST {requestUri} failed with {response.StatusCode}. Body: {body}", null, response.StatusCode);
    }
  }

  public Task PostAsync<TRequest>(string requestUri, TRequest request)
    => PostAsync(requestUri, request, TestContext.Current.CancellationToken);

  Task IHttpTransport.PostAsync<TRequest>(string endpointUrl, EndpointSecurity endpointSecurity, TRequest request, CancellationToken cancellationToken)
    => PostAsync(endpointUrl, request, cancellationToken);

  public async Task PatchAsync<TRequest>(string requestUri, TRequest request, CancellationToken cancellationToken)
  {
    if (!requestUri.StartsWith('/')) requestUri = "/" + requestUri;
    var response = await _httpClient.PatchAsJsonAsync(requestUri, request, cancellationToken);
    if (!response.IsSuccessStatusCode)
    {
      var body = await response.Content.ReadAsStringAsync(cancellationToken);
      throw new HttpRequestException($"PATCH {requestUri} failed with {response.StatusCode}. Body: {body}", null, response.StatusCode);
    }
  }

  public Task PatchAsync<TRequest>(string requestUri, TRequest request)
    => PatchAsync(requestUri, request, TestContext.Current.CancellationToken);

  Task IHttpTransport.PatchAsync<TRequest>(string endpointUrl, EndpointSecurity endpointSecurity, TRequest request, CancellationToken cancellationToken)
    => PatchAsync(endpointUrl, request, cancellationToken);

  public async Task PutAsync<TRequest>(string requestUri, TRequest request, CancellationToken cancellationToken)
  {
    if (!requestUri.StartsWith('/')) requestUri = "/" + requestUri;
    var response = await _httpClient.PutAsJsonAsync(requestUri, request, cancellationToken);
    if (!response.IsSuccessStatusCode)
    {
      var body = await response.Content.ReadAsStringAsync(cancellationToken);
      throw new HttpRequestException($"PUT {requestUri} failed with {response.StatusCode}. Body: {body}", null, response.StatusCode);
    }
  }

  public Task PutAsync<TRequest>(string requestUri, TRequest request)
    => PutAsync(requestUri, request, TestContext.Current.CancellationToken);

  Task IHttpTransport.PutAsync<TRequest>(string endpointUrl, EndpointSecurity endpointSecurity, TRequest request, CancellationToken cancellationToken)
    => PutAsync(endpointUrl, request, cancellationToken);
}
