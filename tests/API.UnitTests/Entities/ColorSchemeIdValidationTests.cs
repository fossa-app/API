using Fossa.API.Core.Entities;
using Shouldly;
using Xunit.Abstractions;

namespace Fossa.API.UnitTests.Entities;

public class ColorSchemeIdValidationTests
{
  private readonly ITestOutputHelper _testOutputHelper;

  public ColorSchemeIdValidationTests(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
  }

  [Theory]
  [InlineData("abc", true)] // Minimum length
  [InlineData("abcd", true)] // One more than minimum
  [InlineData("abcdefghijklmnopqrstuvwxyz", true)] // All lowercase letters
  [InlineData("a-b", true)] // Minimum with hyphen
  [InlineData("ab-c", true)] // Hyphen at position 2
  [InlineData("abc-d", true)] // Hyphen at position 3
  [InlineData("abcd-efgh", true)] // Hyphen in middle of longer string
  [InlineData("verylongthemename-withextension", true)] // Long with hyphen
  public void ValidateColorSchemeId_BoundaryConditions_ShouldBehaveCorrectly(string input, bool shouldBeValid)
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(input));

    // Assert
    if (shouldBeValid)
    {
      exception.ShouldBeNull($"'{input}' should be valid");
      var colorSchemeId = new ColorSchemeId(input);
      colorSchemeId.AsPrimitive().ShouldBe(input);
      _testOutputHelper.WriteLine($"✓ Valid boundary case: '{input}'");
    }
    else
    {
      exception.ShouldNotBeNull($"'{input}' should be invalid");
      _testOutputHelper.WriteLine($"✗ Invalid boundary case: '{input}' - {exception?.Message}");
    }
  }

  [Theory]
  [InlineData("ab")] // One less than minimum
  [InlineData("a")] // Much less than minimum
  [InlineData("")] // Empty
  public void ValidateColorSchemeId_TooShort_ShouldBeInvalid(string input)
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(input));

    // Assert
    exception.ShouldNotBeNull($"'{input}' should be invalid (too short)");
    _testOutputHelper.WriteLine($"✗ Too short: '{input}' - {exception.Message}");
  }

  [Theory]
  [InlineData("A")] // Single uppercase
  [InlineData("ABC")] // All uppercase
  [InlineData("Abc")] // First letter uppercase
  [InlineData("aBc")] // Middle letter uppercase
  [InlineData("abC")] // Last letter uppercase
  [InlineData("test-Theme")] // Uppercase after hyphen
  [InlineData("Test-theme")] // Uppercase before hyphen
  [InlineData("TEST-THEME")] // All uppercase with hyphen
  public void ValidateColorSchemeId_WithUppercaseLetters_ShouldBeInvalid(string input)
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(input));

    // Assert
    exception.ShouldNotBeNull($"'{input}' should be invalid (contains uppercase)");
    _testOutputHelper.WriteLine($"✗ Contains uppercase: '{input}' - {exception.Message}");
  }

  [Theory]
  [InlineData("0")] // Single digit
  [InlineData("123")] // All digits
  [InlineData("abc123")] // Letters then digits
  [InlineData("123abc")] // Digits then letters
  [InlineData("a1b")] // Mixed
  [InlineData("test1")] // Letters with trailing digit
  [InlineData("1test")] // Digit with trailing letters
  [InlineData("test-123")] // Letters, hyphen, digits
  [InlineData("123-test")] // Digits, hyphen, letters
  public void ValidateColorSchemeId_WithDigits_ShouldBeInvalid(string input)
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(input));

    // Assert
    exception.ShouldNotBeNull($"'{input}' should be invalid (contains digits)");
    _testOutputHelper.WriteLine($"✗ Contains digits: '{input}' - {exception.Message}");
  }

  [Theory]
  [InlineData("--")] // Double hyphen only
  [InlineData("test--theme")] // Double hyphen in middle
  [InlineData("---")] // Triple hyphen
  [InlineData("a--b")] // Double hyphen with minimum letters
  [InlineData("test---theme")] // Triple hyphen in middle
  public void ValidateColorSchemeId_WithMultipleConsecutiveHyphens_ShouldBeInvalid(string input)
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(input));

    // Assert
    exception.ShouldNotBeNull($"'{input}' should be invalid (multiple consecutive hyphens)");
    _testOutputHelper.WriteLine($"✗ Multiple consecutive hyphens: '{input}' - {exception.Message}");
  }

  [Theory]
  [InlineData("-")] // Hyphen only
  [InlineData("-test")] // Starts with hyphen
  [InlineData("test-")] // Ends with hyphen
  [InlineData("-test-")] // Starts and ends with hyphen
  [InlineData("-a")] // Hyphen with single letter
  [InlineData("a-")] // Single letter with hyphen
  public void ValidateColorSchemeId_WithHyphenAtStartOrEnd_ShouldBeInvalid(string input)
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(input));

    // Assert
    exception.ShouldNotBeNull($"'{input}' should be invalid (hyphen at start or end)");
    _testOutputHelper.WriteLine($"✗ Hyphen at start/end: '{input}' - {exception.Message}");
  }

  [Theory]
  [InlineData("a-b-c")] // Multiple hyphens (not consecutive)
  [InlineData("test-theme-mode")] // Two hyphens
  [InlineData("a-b-c-d")] // Three hyphens
  [InlineData("one-two-three-four")] // Multiple words
  public void ValidateColorSchemeId_WithMultipleNonConsecutiveHyphens_ShouldBeInvalid(string input)
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(input));

    // Assert
    exception.ShouldNotBeNull($"'{input}' should be invalid (multiple hyphens not allowed)");
    _testOutputHelper.WriteLine($"✗ Multiple hyphens: '{input}' - {exception.Message}");
  }

  [Theory]
  [InlineData(" ")] // Space only
  [InlineData("test theme")] // Space in middle
  [InlineData(" test")] // Leading space
  [InlineData("test ")] // Trailing space
  [InlineData("test  theme")] // Multiple spaces
  [InlineData("\t")] // Tab
  [InlineData("test\ttheme")] // Tab in middle
  [InlineData("\n")] // Newline
  [InlineData("test\ntheme")] // Newline in middle
  [InlineData("\r")] // Carriage return
  [InlineData("test\rtheme")] // Carriage return in middle
  public void ValidateColorSchemeId_WithWhitespace_ShouldBeInvalid(string input)
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(input));

    // Assert
    exception.ShouldNotBeNull($"'{input}' should be invalid (contains whitespace)");
    _testOutputHelper.WriteLine($"✗ Contains whitespace: '{input.Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r")}' - {exception.Message}");
  }

  [Fact]
  public void ValidateColorSchemeId_WithUnicodeCharacters_ShouldBeInvalid()
  {
    // Arrange
    var unicodeInputs = new[]
    {
      "café", // Accented character
      "naïve", // Diaeresis
      "résumé", // Multiple accents
      "测试", // Chinese characters
      "тест", // Cyrillic characters
      "🎨", // Emoji
      "test🎨", // Emoji mixed with letters
      "αβγ", // Greek letters
    };

    foreach (var input in unicodeInputs)
    {
      // Act
      var exception = Record.Exception(() => new ColorSchemeId(input));

      // Assert
      exception.ShouldNotBeNull($"'{input}' should be invalid (contains non-ASCII characters)");
      _testOutputHelper.WriteLine($"✗ Unicode: '{input}' - {exception.Message}");
    }
  }

  [Fact]
  public void ValidateColorSchemeId_PerformanceTest_ShouldHandleManyValidations()
  {
    // Arrange
    const int iterations = 1000;
    var validInputs = new[] { "abc", "test-theme", "simple", "dark-mode", "verylongthemename" };
    var invalidInputs = new[] { "AB", "test theme", "test--theme", "-test", "test123" };

    // Act & Assert
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    for (int i = 0; i < iterations; i++)
    {
      // Test valid inputs
      foreach (var validInput in validInputs)
      {
        var colorSchemeId = new ColorSchemeId(validInput);
        colorSchemeId.AsPrimitive().ShouldBe(validInput);
      }

      // Test invalid inputs
      foreach (var invalidInput in invalidInputs)
      {
        var exception = Record.Exception(() => new ColorSchemeId(invalidInput));
        exception.ShouldNotBeNull();
      }
    }

    stopwatch.Stop();
    _testOutputHelper.WriteLine($"Performance test completed: {iterations} iterations in {stopwatch.ElapsedMilliseconds}ms");
    
    // Should complete reasonably quickly (less than 1 second for 1000 iterations)
    stopwatch.ElapsedMilliseconds.ShouldBeLessThan(1000);
  }
}
