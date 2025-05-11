using Fossa.API.Core.Messages.Commands;
using Fossa.API.Web.ApiModels;

namespace Fossa.API.Web.Messages.Commands;

public record BranchModificationApiCommand(
    long Id,
    string? Name,
    string? TimeZoneId,
    AddressModel? Address) : ICommand;
