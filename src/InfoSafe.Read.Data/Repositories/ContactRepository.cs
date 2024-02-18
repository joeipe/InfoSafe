using Dapper;
using InfoSafe.Read.Data.Repositories.Interfaces;
using InfoSafe.Read.Domain;
using Microsoft.Extensions.Logging;

namespace InfoSafe.Read.Data.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly ILogger<ContactRepository> _logger;
        private readonly ReadDbContext _dataContext;

        public ContactRepository(
            ILogger<ContactRepository> logger,
            ReadDbContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            _logger.LogInformation("{Repository}.{Action} start", nameof(ContactRepository), nameof(GetContactsAsync));

            var sql = @"SELECT * FROM Main.Contacts";

            var contacts = await _dataContext.db.QueryAsync<Contact>(sql);

            return contacts.ToList();
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            _logger.LogInformation("{Repository}.{Action} start", nameof(ContactRepository), nameof(GetContactByIdAsync));

            var sql =
                @"SELECT * FROM Main.Contacts WHERE Id=@Id
                  SELECT * FROM Main.Addresses WHERE ContactId=@Id
                  SELECT * FROM Main.EmailAddresses WHERE ContactId=@Id
                  SELECT * FROM Main.PhoneNumbers WHERE ContactId=@Id";

            Contact? contact;
            using (var multipleResults = await _dataContext.db.QueryMultipleAsync(sql, new { Id = id }))
            {
                contact = multipleResults.Read<Contact>().SingleOrDefault();
                var address = multipleResults.Read<Address>().SingleOrDefault();
                var emailAddresses = multipleResults.Read<EmailAddress>().ToList();
                var phoneNumber = multipleResults.Read<PhoneNumber>().SingleOrDefault();
                if (contact != null)
                {
                    if (address != null)
                    {
                        contact.Address = address;
                    }
                    if (emailAddresses != null)
                    {
                        contact.EmailAddresses = emailAddresses;
                    }
                    if (phoneNumber != null)
                    {
                        contact.PhoneNumber = phoneNumber;
                    }
                }
            }

            return contact;
        }
    }
}