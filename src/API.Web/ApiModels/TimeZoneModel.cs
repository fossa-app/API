namespace Fossa.API.Web.ApiModels;

public record TimeZoneModel(
  string Id,
  string Name,
  TimeSpan CurrentOffset,
  double CurrentOffsetTotalHours,
  int CurrentOffsetHours,
  int CurrentOffsetMinutes);
