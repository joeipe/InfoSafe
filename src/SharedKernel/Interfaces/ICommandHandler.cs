using CSharpFunctionalExtensions;
using MediatR;

namespace SharedKernel.Interfaces
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : ICommand
    {
    }
}