using System.Collections.Generic;
using Medelit.Common;
using Medelit.Domain.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface ITinUserRoleRepo : IRepository<TinUserRole>
    {
        int RegisterUsersToRole(RegisterUsersToRoleCommand request, AuthClaims currentUser, long userId);
        List<TinUserRole> GetAllRecords();
    }
}