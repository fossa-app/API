using TIKSN.Integration.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record EmployeeModificationApiCommand(
    string? FirstName,
    string? LastName,
    string? FullName) : ICommand;
