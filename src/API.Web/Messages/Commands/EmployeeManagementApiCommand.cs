using TIKSN.Integration.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record EmployeeManagementApiCommand(
    long Id,
    long? AssignedBranchId,
    long? AssignedDepartmentId,
    long? ReportsToId,
    string? JobTitle) : ICommand;
