namespace Fossa.API.Core.Messages;

public class CrossTenantUnauthorizedAccessException : Exception
{
  public CrossTenantUnauthorizedAccessException()
  { }

  public CrossTenantUnauthorizedAccessException(string message) : base(message)
  {
  }

  public CrossTenantUnauthorizedAccessException(string message, Exception inner) : base(message, inner)
  {
  }
}
