namespace Fossa.API.Web.Mappers;

public record LicenseTermsModel(
  PartyModel Licensor,
  PartyModel Licensee,
  DateTimeOffset NotBefore,
  DateTimeOffset NotAfter);
