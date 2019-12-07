using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Equinox.Infra.Data.Repository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        public ServiceRepository(MedelitContext context)
            : base(context)
        {
        }

        public Service GetByIdWithIncludes(long serviceId)
        {
            return Db.Service.Include(x => x.ServiceProfessionalRelation).Where(x => x.Id == serviceId).FirstOrDefault();
        }

        public void RemoveProfessionals(long serviceId)
        {
            var professionals = Db.ServiceProfessionalRelation.Where(x => x.ServiceId == serviceId).ToList();
            Db.RemoveRange(professionals);
            Db.SaveChanges();

        }
    }
}
