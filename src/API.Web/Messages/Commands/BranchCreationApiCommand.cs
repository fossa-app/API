using Fossa.Bridge.Models.ApiModels;
using TIKSN.Integration.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record BranchCreationApiCommand(
    string? Name,
    string? TimeZoneId,
    AddressModel? Address) : ICommand;
