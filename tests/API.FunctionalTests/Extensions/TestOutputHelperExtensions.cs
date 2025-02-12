using System.Text.Json;
using Xunit.Abstractions;

namespace Fossa.API.FunctionalTests.Extensions;

public static class TestOutputHelperExtensions
{
  public static async Task WriteAsync(
    this ITestOutputHelper testOutputHelper,
    HttpResponseMessage httpResponseMessage)
  {
    var contentString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
    using JsonDocument jsonDocument = JsonDocument.Parse(contentString);

    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
    };

    string indentedContentString = JsonSerializer.Serialize(jsonDocument.RootElement, options);
    testOutputHelper.WriteLine(indentedContentString);
  }
}
