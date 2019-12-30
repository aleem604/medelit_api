using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medelit.Common;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Medelit.Api.Controllers
{
    [Authorize]
    public class RolesController : ApiController
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger _logger;
       

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            INotificationHandler<DomainNotification> notifications,
            ILoggerFactory loggerFactory,
            IMediatorHandler mediator
            ) : base(notifications, mediator)
        {
            this.roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<RolesController>();
        }
        [HttpGet("roles")]
        [Authorize(Policy = "readonlypolicy")]
        public IActionResult Index()
        {
            
            var roles = roleManager.Roles.ToList();
            if(roles.FirstOrDefault(x=>x.Name.Equals("Admin",StringComparison.InvariantCultureIgnoreCase)) is null)
            roleManager.CreateAsync(new IdentityRole {Name ="Admin" }).Wait();

            if(roles.FirstOrDefault(x=>x.Name.Equals("Manager",StringComparison.InvariantCultureIgnoreCase)) is null)
            roleManager.CreateAsync(new IdentityRole {Name ="Manager" }).Wait();

            if(roles.FirstOrDefault(x=>x.Name.Equals("DataOperator", StringComparison.InvariantCultureIgnoreCase)) is null)
            roleManager.CreateAsync(new IdentityRole {Name = "DataOperator" }).Wait();

            var clerk = roles.FirstOrDefault(x => x.Name.Equals("Clerk"));
            if(clerk != null)
            roleManager.DeleteAsync(clerk).Wait();

            roles = roleManager.Roles.ToList();
            return Response(roles);
        }
       
        [HttpPost("roles")]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            return Response(await roleManager.CreateAsync(role));
        }

        [HttpPut("roles")]
        public async Task<IActionResult> Update([FromBody]IdentityRole role)
        {
            return Response(await roleManager.UpdateAsync(role));
        }

        [HttpDelete("roles/{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            return Response(await roleManager.DeleteAsync(await roleManager.FindByIdAsync(roleId)));
        }

        [HttpDelete("roles")]
        public async Task<IActionResult> DeleteRoles([FromBody] IEnumerable<IdentityRole> roles)
        {
            //roles = roleManager.Roles.ToList();
            foreach (var role in roles)
            {
                await roleManager.DeleteAsync(role);
            }
            return Response(roleManager.Roles.ToList());
        }

    }
}
