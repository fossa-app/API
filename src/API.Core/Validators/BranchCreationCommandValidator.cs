using FluentValidation;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;

namespace Fossa.API.Core.Validators;

public class BranchCreationCommandValidator : AbstractValidator<BranchCreationCommand>
{
  public BranchCreationCommandValidator(
    ICompanyQueryRepository companyQueryRepository,
    IDateTimeZoneLookup dateTimeZoneLookup)
  {
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
    RuleFor(x => x.TimeZone).MustAsync(
    (command, branchTimeZone, cancellationToken) => BranchCommandValidatorHelper.BranchTimeZoneCountryMustBeCompanyCountryAsync(
      companyQueryRepository,
      dateTimeZoneLookup,
      command.TenantID,
      branchTimeZone,
      cancellationToken));
  }
}
