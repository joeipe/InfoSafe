using AutoMapper;
using InfoSafe.Read.Data.Repositories.Interfaces;
using InfoSafe.ViewModel;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using static InfoSafe.Read.Data.Queries.Queries;

namespace InfoSafe.Read.Data.QueryHandlers
{
    public class GetFullContactsQueryHandler : IQueryHandler<GetFullContactsQuery, List<ContactVM>>
    {
        private readonly ILogger<GetContactsQueryHandler> _logger;
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public GetFullContactsQueryHandler(
            ILogger<GetContactsQueryHandler> logger,
            IContactRepository contactRepository,
            IMapper mapper)
        {
            _logger = logger;
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public async Task<List<ContactVM>> Handle(GetFullContactsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Handler}.{Action} start", nameof(GetContactsQueryHandler), nameof(Handle));

            var data = await _contactRepository.GetFullContactsAsync();

            var vm = _mapper.Map<List<ContactVM>>(data);
            return vm;
        }
    }
}