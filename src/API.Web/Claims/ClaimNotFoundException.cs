namespace Fossa.API.Web.Claims;

[Serializable]
public class ClaimNotFoundException : Exception
{
  public ClaimNotFoundException() : base("Claim Not Found")
  {
  }

  public ClaimNotFoundException(string message) : base(message)
  {
  }

  public ClaimNotFoundException(string message, Exception inner) : base(message, inner)
  {
  }

  protected ClaimNotFoundException(
  System.Runtime.Serialization.SerializationInfo info,
  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
