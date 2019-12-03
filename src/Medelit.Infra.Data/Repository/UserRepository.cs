using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class UserRepository : Repository<TinUser>, ITinUserRepo
    {
        public UserRepository(MedelitContext context)
            : base(context)
        {

        }

        public TinUser GetByEmail(string email)
        {
            return DbSet.AsNoTracking().FirstOrDefault(c => c.Email == email);
        }
    }
}
