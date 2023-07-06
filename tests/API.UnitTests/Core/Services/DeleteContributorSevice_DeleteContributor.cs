using Fossa.API.Core.ContributorAggregate;
using Fossa.API.Core.Services;
using Fossa.API.SharedKernel.Interfaces;
using MediatR;
using Moq;
using Xunit;

namespace Fossa.API.UnitTests.Core.Services;

public class DeleteContributorService_DeleteContributor
{
  private readonly Mock<IRepository<Contributor>> _mockRepo = new Mock<IRepository<Contributor>>();
  private readonly Mock<IMediator> _mockMediator = new Mock<IMediator>();
  private readonly DeleteContributorService _service;

  public DeleteContributorService_DeleteContributor()
  {
    _service = new DeleteContributorService(_mockRepo.Object, _mockMediator.Object);
  }

  [Fact]
  public async Task ReturnsNotFoundGivenCantFindContributorAsync()
  {
    var result = await _service.DeleteContributorAsync(0);

    Assert.Equal(Ardalis.Result.ResultStatus.NotFound, result.Status);
  }
}
