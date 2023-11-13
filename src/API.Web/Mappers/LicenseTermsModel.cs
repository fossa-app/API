namespace Fossa.API.Web.Mappers;

public record LicenseTermsModel(
  PartyModel licensor,
  PartyModel licensee,
  DateTimeOffset notBefore,
  DateTimeOffset notAfter);
