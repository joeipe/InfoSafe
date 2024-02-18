using InfoSafe.Read.Domain;

namespace InfoSafe.Read.Data.Repositories.Interfaces
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetContactsAsync();
        Task<Contact> GetContactByIdAsync(int id);
    }
}