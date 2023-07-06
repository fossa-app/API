using Ardalis.Result;

namespace Fossa.API.Core.Interfaces;

public interface IDeleteContributorService
{
  public Task<Result> DeleteContributorAsync(int contributorId);
}
