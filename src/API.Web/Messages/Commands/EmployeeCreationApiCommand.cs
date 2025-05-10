using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record EmployeeCreationApiCommand(
    string FirstName,
    string LastName,
    string FullName) : ICommand;
