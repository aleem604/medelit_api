using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.AspNetCore.Http;

namespace Medelit.Infra.Data.Repository
{
    public class LanguageRepository : Repository<Language>, ILanguageRepository
    {
        public LanguageRepository(MedelitContext context, IHttpContextAccessor contextAccessor)
            : base(context, contextAccessor)
        {

        }
    }
}
