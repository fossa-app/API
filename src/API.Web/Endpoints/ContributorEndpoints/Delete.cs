using FastEndpoints;
using Ardalis.Result;
using Fossa.API.Core.Interfaces;

namespace Fossa.API.Web.Endpoints.ContributorEndpoints;

public class Delete : Endpoint<DeleteContributorRequest>
{

  private readonly IDeleteContributorService _deleteContributorService;

  public Delete(IDeleteContributorService service)
  {
    _deleteContributorService = service;
  }

  public override void Configure()
  {
    Delete(DeleteContributorRequest.Route);
    AllowAnonymous();
    Options(x => x
      .WithTags("ContributorEndpoints"));
  }
  public override async Task HandleAsync(
    DeleteContributorRequest request,
    CancellationToken cancellationToken)
  {
    var result = await _deleteContributorService.DeleteContributorAsync(request.ContributorId);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    await SendNoContentAsync(cancellationToken);
  }
}
