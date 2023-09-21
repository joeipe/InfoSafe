using CSharpFunctionalExtensions;
using MediatR;

namespace SharedKernel.Interfaces
{
    public interface ICommand : IRequest<Result>
    {
        Result ValidationResult { get; set; }

        void Validate();
    }
}