namespace Fossa.API.Web.Claims;

public class ClaimNotFoundException : Exception
{
  public ClaimNotFoundException() : base("Claim not found")
  {
  }

  public ClaimNotFoundException(string message) : base(message)
  {
  }

  public ClaimNotFoundException(string message, Exception inner) : base(message, inner)
  {
  }
}
