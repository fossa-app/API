using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Web.Messages.Commands;

public record CompanySettingsModificationApiCommand(
    long ID,
    string? ColorSchemeId) : ICommand;
