using Fossa.API.Core.Messages.Commands;
using Fossa.API.Web.ApiModels;

namespace Fossa.API.Web.Messages.Commands;

public record BranchCreationApiCommand(
    string? Name,
    string? TimeZoneId,
    AddressModel? Address) : ICommand;
