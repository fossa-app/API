﻿using FluentValidation;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;

namespace Fossa.API.Core.Validators;

public class CompanyCreationCommandValidator : AbstractValidator<CompanyCreationCommand>
{
  public CompanyCreationCommandValidator(
    ISystemLicenseRetriever systemLicenseRetriever,
    ICompanyQueryRepository companyQueryRepository)
  {
    ArgumentNullException.ThrowIfNull(systemLicenseRetriever);

    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
    RuleFor(x => x.Country).MustAsync(
      (companyCountry, cancellationToken) => CompanyCommandValidatorHelper.CompanyCountryMustBeLicensedAsync(
        systemLicenseRetriever,
        companyCountry,
        cancellationToken));
  }
}
