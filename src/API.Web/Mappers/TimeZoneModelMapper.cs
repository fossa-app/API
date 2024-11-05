using Fossa.API.Web.ApiModels;
using NodaTime;
using NodaTime.TimeZones;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class TimeZoneModelMapper : IMapper<DateTimeZone, TimeZoneModel>
{
  public TimeZoneModel Map(DateTimeZone source)
  {
    return new TimeZoneModel(source.Id, TzdbDateTimeZoneSource.Default.TzdbToWindowsIds[source.Id]);
  }
}
