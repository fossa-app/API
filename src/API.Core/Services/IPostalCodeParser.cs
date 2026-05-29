using TIKSN.Globalization;

namespace Fossa.API.Core.Services;

public interface IPostalCodeParser
{
  ParserResult<string> ParsePostalCode(
    CountryInfo country,
    string inputPostalCode);
}
