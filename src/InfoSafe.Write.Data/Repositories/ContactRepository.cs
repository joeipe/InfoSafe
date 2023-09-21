using InfoSafe.Write.Data.Repositories.Interfaces;
using InfoSafe.Write.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfoSafe.Write.Data.Repositories
{
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        private readonly ILogger<ContactRepository> _logger;
        protected WriteDbContext _dbContext;

        public ContactRepository(
            ILogger<ContactRepository> logger,
            WriteDbContext dbContext)
            : base(dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            _logger.LogInformation("{Repository}.{Action} start", nameof(ContactRepository), nameof(GetContactByIdAsync));

            var attendanceData = await SearchForIncludeAsync
                (
                    s => s.Id == id,
                    source => source
                        .Include(x => x.Address)
                        .Include(x => x.EmailAddresses)
                        .Include(x => x.PhoneNumber)
                );

            return attendanceData.First();
        }
    }
}