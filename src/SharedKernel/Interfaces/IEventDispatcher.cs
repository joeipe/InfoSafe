using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Interfaces
{
    public interface IEventDispatcher
    {
        void Dispatch(IDomainEvent ev);
        void Dispatch(IEnumerable<IDomainEvent> events);
    }
}
