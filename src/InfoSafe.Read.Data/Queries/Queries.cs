using InfoSafe.ViewModel;
using SharedKernel.Interfaces;

namespace InfoSafe.Read.Data.Queries
{
    public class Queries
    {
        public record GetContactByIdQuery(int Id) : IQuery<ContactVM> { }
    }
}