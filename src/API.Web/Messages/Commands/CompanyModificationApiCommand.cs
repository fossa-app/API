using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record CompanyModificationApiCommand(
  string? Name,
  string? CountryCode) : ICommand;
