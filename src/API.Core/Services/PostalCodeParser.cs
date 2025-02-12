using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Fossa.API.Core.Services;

[SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed")]
public class PostalCodeParser : IPostalCodeParser
{
  private const string GenericCountryCode = "001";

  public PostalCodeParser()
  {
    PostalCodeParsers = new[]
    {
          ("CA", CanadianPostalCode),
          ("US", USZipCode),
          (GenericCountryCode, GenericPostalCode),
        }.ToFrozenDictionary(k => k.Item1, v => CreateFinalParser(v.Item2), StringComparer.OrdinalIgnoreCase);
  }

  private static Parser<char> AlphaNumeric => either(AsciiDigit, AsciiLetter);
  private static Parser<char> AsciiDigit => satisfy(char.IsAsciiDigit);

  private static Parser<char> AsciiLetter => satisfy(char.IsAsciiLetter);

  [SuppressMessage("Minor Code Smell", "S2234:Parameters to 'Seq' have the same names but not the same order as the method arguments.")]
  private static Parser<Seq<char>> CanadianPostalCode =>
    from a in letter
    from b in digit
    from c in letter
    from _ in spaces
    from d in digit
    from e in letter
    from f in digit
    select Seq(a, b, c, ' ', d, e, f);

  private static Parser<Seq<char>> GenericPostalCode
  {
    get
    {
      var codePart =
          from chars in many1(AlphaNumeric)
          select chars;

      var hyphenSep =
          from _1 in spaces
          from h in ch('-')
          from _2 in spaces
          select Seq1(h);

      var spaceSep =
          from _ in many1(space)
          select Seq1(' ');

      var hyphenOrSpaceSep = either(hyphenSep, spaceSep);

      var element = either(hyphenOrSpaceSep, codePart);

      var parser =
          from firstPart in codePart
          from restParts in optional(flatten(many(chain(hyphenOrSpaceSep, codePart))))
          select restParts.Match(rest => firstPart.Concat(rest.Flatten()), firstPart);

      return parser;
    }
  }

  private static Parser<Seq<char>> USZipCode
  {
    get
    {
      var fiveDigitZip =
            from digits in count(5, AsciiDigit)
            select digits;

      var fourDigitExtension =
          from hyphen in ch('-')
          from _ in spaces
          from digits in count(4, AsciiDigit)
          select digits.Prepend(hyphen);

      var zipCodeParser =
          from zip in fiveDigitZip
          from _ in spaces
          from ext in optional(fourDigitExtension)
          select ext.Match(s => zip.Concat(s), zip);

      return zipCodeParser;
    }
  }

  private IReadOnlyDictionary<string, Parser<string>> PostalCodeParsers { get; }

  public ParserResult<string> ParsePostalCode(
    RegionInfo country,
    string inputPostalCode)
  {
    var key = country.TwoLetterISORegionName;

    if (PostalCodeParsers.TryGetValue(key, out var parser))
    {
      return parser.Parse(inputPostalCode);
    }

    return PostalCodeParsers[GenericCountryCode].Parse(inputPostalCode);
  }

  private static Parser<string> CreateFinalParser(Parser<Seq<char>> postalCodeParser)
  {
    var finalPostalCodeParser =
            from code in between(spaces, spaces, postalCodeParser)
            from _ in eof
            select new string([.. code]).ToUpperInvariant();

    return finalPostalCodeParser;
  }
}
