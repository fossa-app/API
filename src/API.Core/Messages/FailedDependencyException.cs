namespace Fossa.API.Core.Messages;

public class FailedDependencyException : Exception
{
  public FailedDependencyException() { }
  public FailedDependencyException(string message) : base(message) { }
  public FailedDependencyException(string message, Exception inner) : base(message, inner) { }
}
