using Fossa.API.Core.Extensions;
using UnitGenerator;

namespace Fossa.API.Core.Entities;

[UnitOf(typeof(string), UnitGenerateOptions.Validate)]
public readonly partial struct ColorSchemeId
{
  private static readonly Parser<string> _parser = CreateParser();

  private static Parser<string> CreateParser()
  {
    // At least 3 characters long, only ASCII lowercase letters,
    // allow for a hyphen but only one consecutive hyphen in the middle
    var lowercaseLetter = satisfy(char.IsAsciiLetterLower);
    var letterSequence = many1(lowercaseLetter);

    var withoutHyphen =
      from letters in letterSequence
      select letters;

    var withHyphen =
      from firstPart in letterSequence
      from hyphen in ch('-')
      from secondPart in letterSequence
      select firstPart.Append(hyphen).Append(secondPart).ToSeq();

    var parser = either(attempt(withHyphen), withoutHyphen);

    return
      from result in parser
#pragma warning disable S1481
      from _ in eof
#pragma warning restore S1481
      where result.Length >= 3
      select new string([.. result]);
  }

  private partial void Validate()
  {
    _parser.Parse(value).GetOrThrow();
  }
}
