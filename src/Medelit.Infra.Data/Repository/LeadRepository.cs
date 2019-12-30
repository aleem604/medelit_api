using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class LeadRepository : Repository<Lead>, ILeadRepository
    {
        public LeadRepository(MedelitContext context)
            : base(context)
        {

        }

        public IQueryable<Lead> GetAllWithService()
        {
            return Db.Lead.Include(x => x.Services);
        }
        public Lead GetWithInclude(long leadId)
        {
            return Db.Lead.Include(x => x.Services).FirstOrDefault(x => x.Id == leadId);
        }

        public void RemoveLeadServices(long leadId)
        {
            var services = Db.LeadServiceRelation.Where(x => x.LeadId == leadId).ToList();
            Db.LeadServiceRelation.RemoveRange(services);
            Db.SaveChanges();
        }

        public Customer GetCustomerId(long? fromCustomerId)
        {
            var customer = Db.Customer.Include(x => x.Services).FirstOrDefault(x => x.Id == fromCustomerId);
            return customer;
        }


    }
}
