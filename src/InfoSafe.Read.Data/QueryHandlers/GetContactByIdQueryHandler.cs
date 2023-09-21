using AutoMapper;
using InfoSafe.Read.Data.Repositories.Interfaces;
using InfoSafe.ViewModel;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using static InfoSafe.Read.Data.Queries.Queries;

namespace InfoSafe.Read.Data.QueryHandlers
{
    public class GetContactByIdQueryHandler : IQueryHandler<GetContactByIdQuery, ContactVM>
    {
        private readonly ILogger<GetContactByIdQueryHandler> _logger;
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public GetContactByIdQueryHandler(
            ILogger<GetContactByIdQueryHandler> logger,
            IContactRepository contactRepository,
            IMapper mapper)
        {
            _logger = logger;
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public async Task<ContactVM> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Handler}.{Action} start", nameof(GetContactByIdQueryHandler), nameof(Handle));

            var data = await _contactRepository.GetContactByIdAsync(request.Id);

            var vm = _mapper.Map<ContactVM>(data);
            return vm;
        }
    }
}