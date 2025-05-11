using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record DepartmentDeletionApiCommand(long Id) : ICommand;
