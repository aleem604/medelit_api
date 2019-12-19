using System.Collections.Generic;
using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        void SaveCustomerRelation(List<CustomerServiceRelation> newServices);
        Customer GetByIdWithInclude(long customerId);
        void RemoveCustomerServices(long id);
    }
}