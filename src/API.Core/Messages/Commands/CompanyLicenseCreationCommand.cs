using TIKSN.Integration.Messages.Commands;

namespace Fossa.API.Core.Messages.Commands;

public record CompanyLicenseCreationCommand(
  Guid TenantID,
  Seq<byte> LicenseData)
: ICommand;
