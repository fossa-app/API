using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record EmployeeManagementApiCommand(
    long Id,
    long? AssignedBranchId,
    long? AssignedDepartmentId) : ICommand;
