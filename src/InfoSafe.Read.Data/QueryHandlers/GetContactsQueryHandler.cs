﻿using AutoMapper;
using InfoSafe.Read.Data.Repositories.Interfaces;
using InfoSafe.ViewModel;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InfoSafe.Read.Data.Queries.Queries;

namespace InfoSafe.Read.Data.QueryHandlers
{
    public class GetContactsQueryHandler : IQueryHandler<GetContactsQuery, List<ContactVM>>
    {
        private readonly ILogger<GetContactsQueryHandler> _logger;
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public GetContactsQueryHandler(
            ILogger<GetContactsQueryHandler> logger,
            IContactRepository contactRepository,
            IMapper mapper)
        {
            _logger = logger;
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public async Task<List<ContactVM>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Handler}.{Action} start", nameof(GetContactsQueryHandler), nameof(Handle));

            var data = await _contactRepository.GetContactsAsync();

            var vm = _mapper.Map<List<ContactVM>>(data);
            return vm;
        }
    }
}
