using System.Globalization;

namespace Fossa.API.Core.Services;

public interface IPostalCodeParser
{
  ParserResult<string> ParsePostalCode(
    RegionInfo country,
    string inputPostalCode);
}
