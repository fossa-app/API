namespace Fossa.API.Core.Messages;

public class CrossTenantInboundUnauthorizedAccessException : CrossTenantUnauthorizedAccessException
{
  public CrossTenantInboundUnauthorizedAccessException()
  { }

  public CrossTenantInboundUnauthorizedAccessException(string message) : base(message)
  {
  }

  public CrossTenantInboundUnauthorizedAccessException(string message, Exception inner) : base(message, inner)
  {
  }
}
