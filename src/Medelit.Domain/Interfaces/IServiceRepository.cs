using System.Collections.Generic;
using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        void RemoveProfessionals(long serviceId);
        Service GetByIdWithIncludes(long serviceId);
        IEnumerable<Service> GetAllWithProfessionals();
    }
}