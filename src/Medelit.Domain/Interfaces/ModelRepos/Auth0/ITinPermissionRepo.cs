using Medelit.Common;
using Medelit.Domain.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface ITinPermissionRepo : IRepository<TinPermission>
    {
        bool CreatePermission(CreatePermissionCommand request, AuthClaims currentUser);
        bool UpdatePermission(UpdatePermissionCommand request, AuthClaims currentUser);
        bool DeletePermission(DeletePermissionCommand request, AuthClaims currentUser);
    }
}