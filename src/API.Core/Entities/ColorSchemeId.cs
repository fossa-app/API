using Fossa.API.Core.Extensions;
using UnitGenerator;

namespace Fossa.API.Core.Entities;

[UnitOf(typeof(string), UnitGenerateOptions.Validate)]
public readonly partial struct ColorSchemeId
{
  private static readonly Parser<string> _parser = CreateParser();

  private static Parser<string> CreateParser()
  {
    return from first3Letters in count(3, lower)
           from restOfLetters in many(lower)
           select new string([.. first3Letters, .. restOfLetters]);
  }

  private partial void Validate()
  {
    _parser.Parse(value).GetOrThrow();
  }
}
