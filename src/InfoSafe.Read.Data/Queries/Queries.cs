using InfoSafe.ViewModel;
using SharedKernel.Interfaces;

namespace InfoSafe.Read.Data.Queries
{
    public class Queries
    {
        public record GetContactsQuery() : IQuery<List<ContactVM>> { }
        public record GetContactByIdQuery(int Id) : IQuery<ContactVM> { }
    }
}