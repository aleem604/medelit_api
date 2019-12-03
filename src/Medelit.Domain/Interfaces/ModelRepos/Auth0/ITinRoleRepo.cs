using Medelit.Common;
using Medelit.Domain.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface ITinRoleRepo : IRepository<TinRole>
    {
        bool CreateRole(CreateRoleCommand request, AuthClaims currentUser);
        bool UpdateRole(UpdateRoleCommand r, AuthClaims currentUser);
        bool DeleteRole(DeleteRoleCommand r, AuthClaims currentUser);
    }
}