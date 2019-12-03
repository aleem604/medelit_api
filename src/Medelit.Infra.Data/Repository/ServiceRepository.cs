using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;

namespace Equinox.Infra.Data.Repository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        public ServiceRepository(MedelitContext context)
            : base(context)
        {

        }
    }
}
