using System.Collections.Generic;
using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class ProfessionalRepository : Repository<Professional>, IProfessionalRepository
    {
        public ProfessionalRepository(MedelitContext context)
            : base(context)
        {

        }

        public IQueryable<ProfessionalLanguageRelation> GetAllLangs()
        {
            return Db.ProfessionalLanguageRelation;
        }

        public IQueryable<Professional> GetByIdWithLangs(long professionalId)
        {
            return Db.Professional.Include(x => x.ProfessionalLangs).Where(x=>x.Id == professionalId).AsNoTracking();
        }

        public void DeleteLangs(long id)
        {
            var langs = Db.ProfessionalLanguageRelation.Where(x => x.ProfessionalId == id).ToList();
            Db.ProfessionalLanguageRelation.RemoveRange(langs);
            Db.SaveChanges();
        }

    }
}
