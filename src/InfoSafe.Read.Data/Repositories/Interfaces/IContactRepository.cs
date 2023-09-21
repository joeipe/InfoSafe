using InfoSafe.Read.Domain;

namespace InfoSafe.Read.Data.Repositories.Interfaces
{
    public interface IContactRepository
    {
        Task<Contact> GetContactByIdAsync(int id);
    }
}