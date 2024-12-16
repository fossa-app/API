using FluentValidation;
using NodaTime;

namespace Fossa.API.Core.TimeZone;

public class DateTimeZoneProvider : IDateTimeZoneProvider
{
  public DateTimeZone GetDateTimeZoneById(string timeZoneId)
  {
    return DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneId ?? string.Empty)
      ?? throw new ValidationException($"TimeZoneId `{timeZoneId}` is not valid.");
  }
}
