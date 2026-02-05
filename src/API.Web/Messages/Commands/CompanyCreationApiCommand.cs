using TIKSN.Integration.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record CompanyCreationApiCommand(
  string? Name,
  string? CountryCode) : ICommand;
