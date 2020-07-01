using System.Collections.Generic;
using System.Linq;
using Medelit.Common;
using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        void FindCustomer(SearchViewModel viewModel);
        void SaveCustomerRelation(List<CustomerServices> newServices);
        Customer GetByIdWithInclude(long customerId);
        void RemoveCustomerServices(long id);
        IQueryable<Customer> GetAllWithService();
        dynamic GetCustomerConnectedCustomers(long customerId);
        dynamic GetCustomerConnectedServices(long customerId);
        dynamic GetCustomerConnectedProfessionals(long customerId);
        dynamic GetCustomerConnectedBookings(long customerId);
        dynamic GetCustomerConnectedInvoices(long customerId);
        dynamic GetCustomerConnectedLeads(long customerId);
        
    }
}