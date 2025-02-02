using System.Globalization;
using Fossa.API.Core.Services;
using Xunit.Abstractions;

namespace Fossa.API.UnitTests.Services;

public class PostalCodeParserTests
{
  private readonly ITestOutputHelper testOutputHelper;

  public PostalCodeParserTests(
    ITestOutputHelper testOutputHelper)
  {
    this.testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
  }

  [Theory]
  [InlineData("CA", "K1A 0B1", "K1A 0B1")]
  [InlineData("CA", " K1A 0B1", "K1A 0B1")]
  [InlineData("CA", "K1A 0B1 ", "K1A 0B1")]
  [InlineData("CA", " K1A 0B1 ", "K1A 0B1")]
  [InlineData("CA", "K1A  0B1", "K1A 0B1")]
  [InlineData("CA", "K1A0B1", "K1A 0B1")]
  [InlineData("CA", "k1a 0b1", "K1A 0B1")]
  [InlineData("CA", "ABC 012", null)]
  [InlineData("CA", "<K1A 0B1", null)]
  [InlineData("CA", "K1A 0B1>", null)]
  [InlineData("CA", "<K1A 0B1>", null)]
  [InlineData("US", "12345", "12345")]
  [InlineData("US", "12345-1234", "12345-1234")]
  [InlineData("US", " 12345", "12345")]
  [InlineData("US", "12345 ", "12345")]
  [InlineData("US", " 12345 ", "12345")]
  [InlineData("US", " 12345-1234", "12345-1234")]
  [InlineData("US", "12345-1234 ", "12345-1234")]
  [InlineData("US", " 12345-1234 ", "12345-1234")]
  [InlineData("US", "12345 -1234", "12345-1234")]
  [InlineData("US", "12345- 1234", "12345-1234")]
  [InlineData("US", "12345 - 1234", "12345-1234")]
  [InlineData("US", "1234A", null)]
  [InlineData("US", "A2345", null)]
  [InlineData("US", "1234", null)]
  [InlineData("US", "12345-123", null)]
  [InlineData("PL", "02-392", "02-392")]
  [InlineData("UA", "04111", "04111")]
  [InlineData("001", "ABC123", "ABC123")]
  [InlineData("001", "abc123", "ABC123")]
  [InlineData("001", "123ABC", "123ABC")]
  [InlineData("001", "123abc", "123ABC")]
  [InlineData("001", "abc 123", "ABC 123")]
  [InlineData("001", "abc  123", "ABC 123")]
  [InlineData("001", "abc   123", "ABC 123")]
  [InlineData("001", "  ABC  123  DEF  456  ", "ABC 123 DEF 456")]
  [InlineData("001", "  ABC - 123 DEF - 456  ", "ABC-123 DEF-456")]
  public void ParsePostalCode(string countryCode, string inputPostalCode, string? expectedOutputPostalCode)
  {
    // Arrange

    var parser = new PostalCodeParser();

    // Act

    var result = parser.ParsePostalCode(new RegionInfo(countryCode), inputPostalCode);
    var actualOutputPostalCode = result.IsFaulted ? null : result.Reply.Result;

    // Assert

    testOutputHelper.WriteLine($"Input: \"{inputPostalCode}\"");
    var indecies = string.Concat(Enumerable
      .Range(1, inputPostalCode.Length)
      .Select(x => x % 10)
      .Select(x => x.ToString(CultureInfo.InvariantCulture))
      .ToArray());
    testOutputHelper.WriteLine($"Index: \"{indecies}\"");

    if (result.IsFaulted)
    {
      testOutputHelper.WriteLine($"Error: {result.Reply.Error}");
    }

    testOutputHelper.WriteLine($"State: {result.Reply.State}");

    Assert.Equal(expectedOutputPostalCode, actualOutputPostalCode);

    if (!result.IsFaulted)
    {
      Assert.Equal(ResultTag.Consumed, result.Tag);
    }
  }
}
