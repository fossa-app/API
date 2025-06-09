using Fossa.API.Core.Entities;
using Shouldly;

namespace Fossa.API.UnitTests.Entities;

public class ColorSchemeIdBasicTests
{
  [Fact]
  public void Constructor_WithValidSimpleValue_ShouldSucceed()
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId("abc"));

    // Assert
    exception.ShouldBeNull();
    
    var colorSchemeId = new ColorSchemeId("abc");
    colorSchemeId.AsPrimitive().ShouldBe("abc");
  }

  [Fact]
  public void Constructor_WithValidHyphenValue_ShouldSucceed()
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId("dark-theme"));

    // Assert
    exception.ShouldBeNull();
    
    var colorSchemeId = new ColorSchemeId("dark-theme");
    colorSchemeId.AsPrimitive().ShouldBe("dark-theme");
  }

  [Fact]
  public void Constructor_WithInvalidShortValue_ShouldThrowException()
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId("ab"));

    // Assert
    exception.ShouldNotBeNull();
  }

  [Fact]
  public void Constructor_WithInvalidUppercaseValue_ShouldThrowException()
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId("ABC"));

    // Assert
    exception.ShouldNotBeNull();
  }

  [Fact]
  public void Constructor_WithInvalidDoubleHyphenValue_ShouldThrowException()
  {
    // Arrange & Act
    var exception = Record.Exception(() => new ColorSchemeId("test--theme"));

    // Assert
    exception.ShouldNotBeNull();
  }
}
