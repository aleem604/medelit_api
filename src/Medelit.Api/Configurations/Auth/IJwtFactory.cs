
using Medelit.Application;
using Medelit.Common;
using Medelit.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Medelit.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(CurrentUserInfo userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(CurrentUserInfo ususerInfo, MedelitUser user, IEnumerable<string> roles );
    }
}
