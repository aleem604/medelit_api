using System.Collections.Generic;
using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Equinox.Infra.Data.Repository
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(MedelitContext context)
            : base(context)
        {
        }

        public Customer GetByIdWithInclude(long customerId)
        {
            return Db.Customer.Include(x => x.Services).FirstOrDefault(x => x.Id == customerId);
        }

        public void RemoveCustomerServices(long id)
        {
            var services = Db.CustomerServiceRelation.Where(x => x.CustomerId == id).ToList();
            Db.RemoveRange(services);
            Db.SaveChanges();
        }

        public void SaveCustomerRelation(List<CustomerServiceRelation> newServices)
        {
            Db.CustomerServiceRelation.AddRange(newServices);
            Db.SaveChanges();
        }
    }
}
