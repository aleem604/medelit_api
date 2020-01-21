using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class LeadRepository : Repository<Lead>, ILeadRepository
    {
        public LeadRepository(MedelitContext context, IHttpContextAccessor contextAccessor)
            : base(context, contextAccessor)
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

        public IQueryable<LeadServiceRelation> GetLeadServiceRelations()
        {
            return Db.LeadServiceRelation;
        }

    }
}
