using Fossa.API.Core.Entities;
using Shouldly;
using Xunit.Abstractions;

namespace Fossa.API.UnitTests.Entities;

public class ColorSchemeIdTests
{
  private readonly ITestOutputHelper _testOutputHelper;

  public ColorSchemeIdTests(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
  }

  [Theory]
  [InlineData("abc")] // Minimum valid length
  [InlineData("theme")] // Simple lowercase
  [InlineData("dark-theme")] // With hyphen in middle
  [InlineData("light-mode")] // Another hyphen example
  [InlineData("verylongthemename")] // Long name
  [InlineData("a-b")] // Minimum with hyphen (3 chars total)
  [InlineData("simple")] // Simple word
  [InlineData("corporate-branding")] // Longer with hyphen
  [InlineData("minimalist")] // No hyphen, longer
  [InlineData("red")] // Exactly 3 characters
  [InlineData("blue-sky")] // Common theme name pattern
  public void Constructor_WithValidColorSchemeId_ShouldSucceed(string validColorSchemeId)
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(validColorSchemeId));

    // Assert
    exception.ShouldBeNull($"ColorSchemeId '{validColorSchemeId}' should be valid");

    var colorSchemeId = new ColorSchemeId(validColorSchemeId);
    colorSchemeId.AsPrimitive().ShouldBe(validColorSchemeId);

    _testOutputHelper.WriteLine($"✓ Valid: '{validColorSchemeId}'");
  }

  [Theory]
  [InlineData("ab")] // Too short (less than 3 characters)
  [InlineData("a")] // Single character
  [InlineData("")] // Empty string
  [InlineData("AB")] // Uppercase letters
  [InlineData("ABC")] // All uppercase
  [InlineData("Test")] // Mixed case
  [InlineData("test123")] // Contains numbers
  [InlineData("test--theme")] // Multiple consecutive hyphens
  [InlineData("-test")] // Starts with hyphen
  [InlineData("test-")] // Ends with hyphen
  [InlineData("test theme")] // Contains space
  [InlineData("test_theme")] // Contains underscore
  [InlineData("test@theme")] // Contains special character
  [InlineData("test.theme")] // Contains dot
  [InlineData("test/theme")] // Contains slash
  [InlineData("test\\theme")] // Contains backslash
  [InlineData("test+theme")] // Contains plus
  [InlineData("test=theme")] // Contains equals
  [InlineData("test&theme")] // Contains ampersand
  [InlineData("test%theme")] // Contains percent
  [InlineData("test$theme")] // Contains dollar
  [InlineData("test#theme")] // Contains hash
  [InlineData("test!theme")] // Contains exclamation
  [InlineData("test?theme")] // Contains question mark
  [InlineData("test*theme")] // Contains asterisk
  [InlineData("test(theme")] // Contains parenthesis
  [InlineData("test)theme")] // Contains parenthesis
  [InlineData("test[theme")] // Contains bracket
  [InlineData("test]theme")] // Contains bracket
  [InlineData("test{theme")] // Contains brace
  [InlineData("test}theme")] // Contains brace
  [InlineData("test|theme")] // Contains pipe
  [InlineData("test:theme")] // Contains colon
  [InlineData("test;theme")] // Contains semicolon
  [InlineData("test\"theme")] // Contains quote
  [InlineData("test'theme")] // Contains apostrophe
  [InlineData("test<theme")] // Contains less than
  [InlineData("test>theme")] // Contains greater than
  [InlineData("test,theme")] // Contains comma
  [InlineData("test`theme")] // Contains backtick
  [InlineData("test~theme")] // Contains tilde

  [InlineData("12")] // Numbers only, too short
  [InlineData("123")] // Numbers only
  [InlineData("1a2")] // Mixed numbers and letters
  [InlineData("a1b")] // Mixed letters and numbers
  public void Constructor_WithInvalidColorSchemeId_ShouldThrowException(string invalidColorSchemeId)
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(invalidColorSchemeId));

    // Assert
    exception.ShouldNotBeNull($"ColorSchemeId '{invalidColorSchemeId}' should be invalid");

    _testOutputHelper.WriteLine($"✗ Invalid: '{invalidColorSchemeId}' - {exception.Message}");
  }

  [Fact]
  public void Constructor_WithNullValue_ShouldThrowException()
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId(null!));

    // Assert
    exception.ShouldNotBeNull();
  }

  [Fact]
  public void AsPrimitive_ShouldReturnOriginalValue()
  {
    // Arrange
    const string originalValue = "test-theme";
    var colorSchemeId = new ColorSchemeId(originalValue);

    // Act
    var result = colorSchemeId.AsPrimitive();

    // Assert
    result.ShouldBe(originalValue);
  }

  [Fact]
  public void ToString_ShouldReturnOriginalValue()
  {
    // Arrange
    const string originalValue = "dark-mode";
    var colorSchemeId = new ColorSchemeId(originalValue);

    // Act
    var result = colorSchemeId.ToString();

    // Assert
    result.ShouldBe(originalValue);
  }

  [Fact]
  public void Equals_WithSameValue_ShouldReturnTrue()
  {
    // Arrange
    const string value = "theme-one";
    var colorSchemeId1 = new ColorSchemeId(value);
    var colorSchemeId2 = new ColorSchemeId(value);

    // Act & Assert
    colorSchemeId1.Equals(colorSchemeId2).ShouldBeTrue();
    (colorSchemeId1 == colorSchemeId2).ShouldBeTrue();
    (colorSchemeId1 != colorSchemeId2).ShouldBeFalse();
  }

  [Fact]
  public void Equals_WithDifferentValue_ShouldReturnFalse()
  {
    // Arrange
    var colorSchemeId1 = new ColorSchemeId("theme-one");
    var colorSchemeId2 = new ColorSchemeId("theme-two");

    // Act & Assert
    colorSchemeId1.Equals(colorSchemeId2).ShouldBeFalse();
    (colorSchemeId1 == colorSchemeId2).ShouldBeFalse();
    (colorSchemeId1 != colorSchemeId2).ShouldBeTrue();
  }

  [Fact]
  public void GetHashCode_WithSameValue_ShouldReturnSameHashCode()
  {
    // Arrange
    const string value = "consistent-theme";
    var colorSchemeId1 = new ColorSchemeId(value);
    var colorSchemeId2 = new ColorSchemeId(value);

    // Act
    var hashCode1 = colorSchemeId1.GetHashCode();
    var hashCode2 = colorSchemeId2.GetHashCode();

    // Assert
    hashCode1.ShouldBe(hashCode2);
  }

  [Fact]
  public void ExplicitCast_FromString_ShouldCreateValidColorSchemeId()
  {
    // Arrange
    const string value = "cast-theme";

    // Act
    var colorSchemeId = (ColorSchemeId)value;

    // Assert
    colorSchemeId.AsPrimitive().ShouldBe(value);
  }

  [Fact]
  public void ExplicitCast_ToString_ShouldReturnOriginalValue()
  {
    // Arrange
    const string originalValue = "cast-back";
    var colorSchemeId = new ColorSchemeId(originalValue);

    // Act
    var result = (string)colorSchemeId;

    // Assert
    result.ShouldBe(originalValue);
  }
}
