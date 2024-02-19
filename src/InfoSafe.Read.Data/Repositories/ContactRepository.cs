using Dapper;
using InfoSafe.Read.Data.Repositories.Interfaces;
using InfoSafe.Read.Domain;
using Microsoft.Extensions.Logging;

namespace InfoSafe.Read.Data.Repositories
{
    public class ContactRepository : DapperGenericRepository<Contact>, IContactRepository
    {
        private readonly ILogger<ContactRepository> _logger;
        private readonly ReadDbContext _dataContext;

        public ContactRepository(
            ILogger<ContactRepository> logger,
            ReadDbContext dataContext)
            : base(dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            _logger.LogInformation("{Repository}.{Action} start", nameof(ContactRepository), nameof(GetContactsAsync));

            var contacts = await GetAllAsync();

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

        //Multi Mapping
        public async Task<List<Contact>> GetFullContactsAsync()
        {
            _logger.LogInformation("{Repository}.{Action} start", nameof(ContactRepository), nameof(GetContactByIdAsync));

            var sql =
                @"SELECT * 
                  FROM Main.Contacts C
                   INNER JOIN Main.Addresses A ON A.ContactId=C.Id
                   INNER JOIN Main.EmailAddresses E ON E.ContactId=C.Id
                   INNER JOIN Main.PhoneNumbers P ON P.ContactId=C.Id";

            var contactDict = new Dictionary<int, Contact>();
            var contacts = await _dataContext.db.QueryAsync<Contact, Address, EmailAddress, PhoneNumber, Contact>(sql, (contact, address, emailAddress, phoneNumber) => 
                {
                    if (!contactDict.TryGetValue(contact.Id, out var currentContact))
                    {
                        currentContact = contact;
                        contactDict.Add(currentContact.Id, currentContact);
                    }

                    currentContact.Address = address;
                    currentContact.EmailAddresses?.Add(emailAddress);
                    currentContact.PhoneNumber = phoneNumber;
                    return currentContact;
                });

            return contacts.Distinct().ToList();
        }
    }
}