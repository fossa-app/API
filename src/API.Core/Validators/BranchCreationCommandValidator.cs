﻿using FluentValidation;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;

namespace Fossa.API.Core.Validators;

public class BranchCreationCommandValidator : AbstractValidator<BranchCreationCommand>
{
  public BranchCreationCommandValidator(
    ICompanyQueryRepository companyQueryRepository,
    IValidator<Address> addressValidator,
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
      cancellationToken))
      .WithMessage(BranchCommandValidatorHelper.BranchTimeZoneCountryMustBeCompanyCountryErrorMessage);
    RuleFor(x => x.Address).IfSome(addressValidator);
    RuleFor(x => x.Address).MustAsync(
      (command, branchTimeZone, cancellationToken) => BranchCommandValidatorHelper.BranchAddressCountryMustBeCompanyCountryAsync(
        companyQueryRepository,
        command.TenantID,
        branchTimeZone,
        cancellationToken))
        .WithMessage(BranchCommandValidatorHelper.BranchAddressCountryMustBeCompanyCountryErrorMessage)
        .OverridePropertyName(BranchCommandValidatorHelper.BranchAddressCountryMustBeCompanyCountryPropertyName);
  }
}
