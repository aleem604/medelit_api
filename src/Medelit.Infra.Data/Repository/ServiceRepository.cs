using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Infra.Data.Repository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        public ServiceRepository(MedelitContext context)
            : base(context)
        {
        }

        public IEnumerable<Service> GetAllWithProfessionals()
        {
            return Db.Service.Include(x => x.ServiceProfessionals);
        }

        public Service GetByIdWithIncludes(long serviceId)
        {
            return Db.Service.Include(x => x.ServiceProfessionals).Where(x => x.Id == serviceId).FirstOrDefault();
        }

        public void RemoveProfessionals(long serviceId)
        {
            var professionals = Db.ServiceProfessionalRelation.Where(x => x.ServiceId == serviceId).ToList();
            Db.RemoveRange(professionals);
            Db.SaveChanges();

        }
    }
}
