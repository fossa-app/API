﻿using FluentValidation;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Validators;

public class CompanyCreationCommandValidator : AbstractValidator<CompanyCreationCommand>
{
  public CompanyCreationCommandValidator(ICompanyQueryRepository companyQueryRepository)
  {
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
    RuleFor(x => x.Moniker).NotEmpty();
    RuleFor(x => x.Moniker).SubDomainLength();
    RuleFor(x => x.Moniker).OnlyAsciiLetterLowersAndDigits();
    RuleFor(x => x.Moniker)
      .MustAsync(async (_, moniker, context, cancellationToken) =>
      {
        var result = await companyQueryRepository.FindByMonikerAsync(moniker, cancellationToken).ConfigureAwait(false);
        return result.Match(s =>
          {
            context.MessageFormatter.AppendArgument("ExistingMonikerCompanyID", s.ID);
            context.MessageFormatter.AppendArgument("ExistingMonikerCompanyTenantID", s.TenantID);
            context.MessageFormatter.AppendArgument("ExistingMonikerCompanyName", s.Name);

            return false;
          },
          () => true);
      })
      .WithMessage("Moniker already exists.");
  }
}
