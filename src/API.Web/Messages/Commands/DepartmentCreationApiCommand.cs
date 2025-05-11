using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record DepartmentCreationApiCommand(
    string? Name,
    long? ParentDepartmentId,
    long? ManagerId) : ICommand;
