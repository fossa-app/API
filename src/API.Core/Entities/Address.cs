using TIKSN.Globalization;

namespace Fossa.API.Core.Entities;

public record Address(
  string Line1,
  Option<string> Line2,
  string City,
  string Subdivision,
  string PostalCode,
  CountryInfo Country);
