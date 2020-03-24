using Medelit.Common;
using Medelit.Infra.CrossCutting.Identity.Data;
using Medelit.Infra.CrossCutting.Identity.Models;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<AuthUser> GetUsers()
        {
            var users =  _context.Users.ToList();
            var commonUser = new List<AuthUser>();
            foreach (var user in users)
            {
                commonUser.Add(new AuthUser {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
                });
            }
            return commonUser;
        }


    }
}
