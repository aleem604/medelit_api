using System.Collections.Generic;
using System.Linq;
using Medelit.Common;
using Medelit.Domain.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface ITinRolePermissionRepo : IRepository<TinRolePermission>
    {
        int CreateRolePermission(long roleId, long perm, long ownerId);
        int DeleteRolePermissions(long roleId, long ownerId);
        IQueryable<TinRolePermission> GetAll(eIncludeOptions all);
    }
}