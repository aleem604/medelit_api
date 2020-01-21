using Medelit.Infra.CrossCutting.Identity.Data;

namespace Medelit.Application
{
    public  class BaseService
    {
        private readonly ApplicationDbContext _context;
        public BaseService(ApplicationDbContext context)
        {
            _context = context;
        }
        public string GetAssignedUser(string assignedToId)
        {
            if (string.IsNullOrEmpty(assignedToId))
                return assignedToId;

            return _context.Users.Find(assignedToId).FullName;
        }
    }
}
