using MediatR;

namespace Fossa.API.Core.Messages.Commands;

public interface ICommand : IRequest<Unit>;
