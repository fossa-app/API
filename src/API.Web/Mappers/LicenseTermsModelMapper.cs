using TIKSN.Licensing;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class LicenseTermsModelMapper : IMapper<LicenseTerms, LicenseTermsModel>
{
  private readonly IMapper<Party, PartyModel> _partyModelMapper;

  public LicenseTermsModelMapper(IMapper<Party, PartyModel> partyModelMapper)
  {
    _partyModelMapper = partyModelMapper ?? throw new ArgumentNullException(nameof(partyModelMapper));
  }

  public LicenseTermsModel Map(LicenseTerms source)
  {
    return new LicenseTermsModel(
      _partyModelMapper.Map(source.Licensor),
      _partyModelMapper.Map(source.Licensee),
      source.NotBefore,
      source.NotAfter);
  }
}
