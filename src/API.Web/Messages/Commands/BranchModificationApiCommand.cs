using Fossa.API.Web.ApiModels;
using TIKSN.Integration.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record BranchModificationApiCommand(
    long Id,
    string? Name,
    string? TimeZoneId,
    AddressModel? Address) : ICommand;
