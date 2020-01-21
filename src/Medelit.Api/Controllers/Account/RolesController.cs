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
            return Response(roleManager.Roles.ToList());
        }

        [HttpPost("roles/find")]
        public IActionResult FindRoles([FromBody] SearchViewModel viewModel)
        {
            var query = roleManager.Roles;

            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>

                    (!string.IsNullOrEmpty(x.Id) && x.Id.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                );
            }

            switch (viewModel.SortField)
            {
                case "name":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Name);
                    else
                        query = query.OrderByDescending(x => x.Name);
                    break;
              
                default:
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Id);
                    else
                        query = query.OrderByDescending(x => x.Id);
                    break;
            }

            var totalCount = query.LongCount();

            return Response(new
            {
                items = query.Skip(viewModel.PageNumber * viewModel.PageSize).Take(viewModel.PageSize).ToList(),
                totalCount
            });
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
