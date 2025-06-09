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
    var letterSequence = many1(satisfy(char.IsAsciiLetterLower));

    var withoutHyphen =
      letterSequence.Bind(letters =>
        eof.Map(_ => new string([.. letters])));

    var withHyphen =
      letterSequence.Bind(firstPart =>
        ch('-').Bind(hyphen =>
          letterSequence.Bind(secondPart =>
            eof.Map(_ => new string([.. firstPart, hyphen, .. secondPart])))));

    var parser = either<string>(withHyphen, withoutHyphen);

    return from result in parser
           where result.Length >= 3
           select result;
  }

  private partial void Validate()
  {
    _parser.Parse(value).GetOrThrow();
  }
}
