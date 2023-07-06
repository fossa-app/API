using Fossa.API.Core.ProjectAggregate;
using Fossa.API.Core.ProjectAggregate.Events;
using Fossa.API.Core.Interfaces;
using Fossa.API.Core.ProjectAggregate.Handlers;
using Moq;
using Xunit;

namespace Fossa.API.UnitTests.Core.Handlers;

public class ItemCompletedEmailNotificationHandlerHandle
{
  private readonly ItemCompletedEmailNotificationHandler _handler;
  private readonly Mock<IEmailSender> _emailSenderMock;

  public ItemCompletedEmailNotificationHandlerHandle()
  {
    _emailSenderMock = new Mock<IEmailSender>();
    _handler = new ItemCompletedEmailNotificationHandler(_emailSenderMock.Object);
  }

  [Fact]
  public async Task ThrowsExceptionGivenNullEventArgumentAsync()
  {
#nullable disable
    _ = await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
#nullable enable
  }

  [Fact]
  public async Task SendsEmailGivenEventInstance()
  {
    await _handler.Handle(new ToDoItemCompletedEvent(new ToDoItem()), CancellationToken.None);

    _emailSenderMock.Verify(sender => sender.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
  }
}
