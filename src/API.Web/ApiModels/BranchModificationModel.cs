namespace Fossa.API.Web.ApiModels;

public record BranchModificationModel(
  string? Name,
  string? TimeZoneId,
  AddressModel? Address);
