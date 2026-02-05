using TIKSN.Integration.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record BranchDeletionApiCommand(long Id) : ICommand;
