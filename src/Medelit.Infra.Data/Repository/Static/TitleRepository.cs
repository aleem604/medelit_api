using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;

namespace Equinox.Infra.Data.Repository
{
    public class TitleRepository : Repository<Title>, ITitleRepository
    {
        public TitleRepository(MedelitContext context)
            : base(context)
        {

        }
    }
}
