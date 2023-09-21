using AutoMapper;
using CSharpFunctionalExtensions;
using InfoSafe.Write.Data.Commands;
using InfoSafe.Write.Data.Repositories.Interfaces;
using InfoSafe.Write.Domain;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;

namespace InfoSafe.Write.Data.CommandHandlers
{
    public class ContactSaveCommandHandler : ICommandHandler<ContactSaveCommand>
    {
        private readonly ILogger<ContactSaveCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IContactRepository _contactRepository;

        public ContactSaveCommandHandler(
            ILogger<ContactSaveCommandHandler> logger,
            IMapper mapper,
            IContactRepository contactRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _contactRepository = contactRepository;
        }

        public async Task<Result> Handle(ContactSaveCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Handler}.{Action} start", nameof(ContactSaveCommandHandler), nameof(Handle));

            request.Validate();
            if (request.ValidationResult.IsFailure)
            {
                _logger.LogInformation("{CommandHandler}.{Action} - {Failure}", nameof(ContactSaveCommandHandler), nameof(Handle), request.ValidationResult.ToString());
                return request.ValidationResult;
            }

            Contact data;
            if (request.Contact.Id == 0)
            {
                data = _mapper.Map<Contact>(request.Contact);
                _contactRepository.Create(data);
            }
            else
            {
                data = await _contactRepository.GetContactByIdAsync(request.Contact.Id);
                _mapper.Map(request.Contact, data);
            }
            await _contactRepository.SaveAsync();
            return request.ValidationResult;
        }
    }
}