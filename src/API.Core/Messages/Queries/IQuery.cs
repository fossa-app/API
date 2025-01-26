namespace Fossa.API.Core.Messages.Queries;

public interface IQuery<out TResult> : IRequest<TResult>;
