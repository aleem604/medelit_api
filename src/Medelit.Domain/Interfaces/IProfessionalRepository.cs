using Medelit.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface IProfessionalRepository : IRepository<Professional>
    {
        IQueryable<ProfessionalLanguageRelation> GetAllLangs();
        IQueryable<Professional> GetByIdWithLangs(long professionalId);
        void DeleteLangs(long id);
    }
}