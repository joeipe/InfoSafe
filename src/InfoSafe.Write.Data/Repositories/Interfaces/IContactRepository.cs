using InfoSafe.Write.Domain;

namespace InfoSafe.Write.Data.Repositories.Interfaces
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        Task<Contact> GetContactByIdAsync(int id);
    }
}