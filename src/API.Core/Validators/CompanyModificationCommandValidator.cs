using FluentValidation;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Services;

namespace Fossa.API.Core.Validators;

public class CompanyModificationCommandValidator : AbstractValidator<CompanyModificationCommand>
{
  public CompanyModificationCommandValidator(
    ISystemLicenseRetriever systemLicenseRetriever)
  {
    ArgumentNullException.ThrowIfNull(systemLicenseRetriever);

    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
    RuleFor(x => x.Country).MustAsync(
    (companyCountry, cancellationToken) => CompanyCommandValidatorHelper.CompanyCountryMustBeLicensedAsync(
      systemLicenseRetriever,
      companyCountry,
      cancellationToken))
      .WithMessage(CompanyCommandValidatorHelper.CompanyCountryMustBeLicensedErrorMessage);
  }
}
