namespace Fossa.API.Core.Messages;

public class EntityExistsException : Exception
{
  public EntityExistsException() { }
  public EntityExistsException(string message) : base(message) { }
  public EntityExistsException(string message, Exception inner) : base(message, inner) { }
}
