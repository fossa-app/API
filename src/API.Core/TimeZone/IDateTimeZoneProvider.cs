using NodaTime;

namespace Fossa.API.Core.TimeZone;

public interface IDateTimeZoneProvider
{
  DateTimeZone GetDateTimeZoneById(string timeZoneId);
}
