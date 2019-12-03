using Medelit.Domain.Interfaces;
using Medelit.Infra.Data.Context;

namespace Medelit.Infra.Data.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MedelitContext _context;

        public UnitOfWork(MedelitContext context)
        {
            _context = context;
        }

        public bool Commit()
        {
            return _context.SaveChanges() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
