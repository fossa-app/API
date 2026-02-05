using TIKSN.Integration.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record DepartmentModificationApiCommand(
    long Id,
    string? Name,
    long? ParentDepartmentId,
    long? ManagerId) : ICommand;
