﻿using Fossa.API.Core.Extensions;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;

namespace Fossa.API.Core.Messages.Commands;

public class BranchModificationCommandHandler : IRequestHandler<BranchModificationCommand, Unit>
{
  private readonly IBranchQueryRepository _branchQueryRepository;
  private readonly IBranchRepository _branchRepository;
  private readonly IPostalCodeParser _postalCodeParser;

  public BranchModificationCommandHandler(
    IBranchQueryRepository branchQueryRepository,
    IBranchRepository branchRepository,
    IPostalCodeParser postalCodeParser)
  {
    _branchQueryRepository = branchQueryRepository ?? throw new ArgumentNullException(nameof(branchQueryRepository));
    _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
    _postalCodeParser = postalCodeParser ?? throw new ArgumentNullException(nameof(postalCodeParser));
  }

  public async Task<Unit> Handle(
    BranchModificationCommand request,
    CancellationToken cancellationToken)
  {
    var entity = await _branchQueryRepository.GetAsync(request.ID, cancellationToken).ConfigureAwait(false);
    entity = entity with
    {
      Name = request.Name,
      TimeZone = request.TimeZone,
      Address = request.Address.Map(x =>
      {
        var postalCode = _postalCodeParser.ParsePostalCode(x.Country, x.PostalCode).GetOrThrow();
        return x with { PostalCode = postalCode };
      }),
    };
    await _branchRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
