using Medelit.Common;
using Medelit.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medelit.Api.Configurations.Auth
{
    public static class RolesExtensions
    {
        public static async Task InitializeAsync(RoleManager<MedelitRole> roleManager)
        {
            foreach (string roleName in Enum.GetNames(typeof(UserRoles)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new MedelitRole(roleName));
                }
            }
        }
    }
}
