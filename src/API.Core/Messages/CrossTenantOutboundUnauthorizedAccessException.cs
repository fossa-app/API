namespace Fossa.API.Core.Messages;

public class CrossTenantOutboundUnauthorizedAccessException : CrossTenantUnauthorizedAccessException
{
  public CrossTenantOutboundUnauthorizedAccessException()
  { }

  public CrossTenantOutboundUnauthorizedAccessException(string message) : base(message)
  {
  }

  public CrossTenantOutboundUnauthorizedAccessException(string message, Exception inner) : base(message, inner)
  {
  }
}
