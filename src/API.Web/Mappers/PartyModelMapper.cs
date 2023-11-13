using System.ComponentModel.DataAnnotations;
using TIKSN.Licensing;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class PartyModelMapper : IMapper<Party, PartyModel>
{
  public PartyModel Map(Party source)
  {
    return source switch
    {
      OrganizationParty organizationParty => new PartyModel(organizationParty.LongName, organizationParty.ShortName),
      IndividualParty individualParty => new PartyModel(individualParty.FullName,
        $"{individualParty.FirstName} {individualParty.LastName}"),
      _ => throw new ValidationException("Unknown party kind")
    };
  }
}
