using InfoSafe.Read.Domain;

namespace InfoSafe.Read.Data.Repositories.Interfaces
{
    public interface IContactRepository : IDapperGenericRepository<Contact>
    {
        Task<List<Contact>> GetContactsAsync();

        Task<Contact> GetContactByIdAsync(int id);

        Task<List<Contact>> GetFullContactsAsync();
    }
}