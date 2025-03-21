﻿using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Commands;

public class CompanyModificationCommandHandler : IRequestHandler<CompanyModificationCommand, Unit>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly ICompanyRepository _companyRepository;

  public CompanyModificationCommandHandler(
    ICompanyQueryRepository companyQueryRepository,
    ICompanyRepository companyRepository)
  {
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
  }

  public async Task<Unit> Handle(
    CompanyModificationCommand request,
    CancellationToken cancellationToken)
  {
    var entity = await _companyQueryRepository.GetByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);
    entity = entity with { Name = request.Name, Country = request.Country };
    await _companyRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
