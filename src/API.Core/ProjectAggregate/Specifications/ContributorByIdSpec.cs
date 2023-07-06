using Ardalis.Specification;
using Fossa.API.Core.ContributorAggregate;

namespace Fossa.API.Core.ProjectAggregate.Specifications;

public class ContributorByIdSpec : Specification<Contributor>, ISingleResultSpecification
{
  public ContributorByIdSpec(int contributorId)
  {
    Query
        .Where(contributor => contributor.Id == contributorId);
  }
}
