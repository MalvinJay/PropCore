using MediatR;

namespace PropCore.Application.Common;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}