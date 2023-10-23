using MediatR;

namespace PropCore.Application.Common;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand : IRequest
{
}