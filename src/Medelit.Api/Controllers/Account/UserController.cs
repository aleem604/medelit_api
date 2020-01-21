using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Medelit.Application;
using Medelit.Common;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Models;
using Medelit.Infra.CrossCutting.Identity.Data;
using Medelit.Infra.CrossCutting.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Medelit.Api.Controllers
{
    public class UserController : ApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<MedelitUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UserController(
            ApplicationDbContext context,
            UserManager<MedelitUser> userManager,
            RoleManager<IdentityRole> roleManager,
            INotificationHandler<DomainNotification> notifications,
            ILoggerFactory loggerFactory,
            IMapper mapper,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<UserController>();
            _mapper = mapper;
        }

        [HttpGet("account/current-user")]
        public async Task<IActionResult> GetCurrentUsers()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(UserRoles.Admin.ToString()))
            {
                foreach (var role in Enum.GetValues(typeof(UserRoles)))
                {
                    roles.Add(role.ToString());
                }
            }
            return Response(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber,
                roles = roles
            });
        }

        [HttpGet("account/users")]
        public async Task<IActionResult> GetUsers()
        {
            return Response(await Task.FromResult(_context.Users.ToList()));
        }

        [HttpGet("account/users/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await Task.FromResult(_context.Users.FirstOrDefault(x => x.Id == userId));
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleIds = _roleManager.Roles.Where(x => userRoles.Contains(x.Name)).Select(x => x.Id).ToList();
            var viewModel = _mapper.Map<UserViewModel>(user);
            viewModel.Roles = roleIds;
            return Response(viewModel);
        }

        [HttpPost("account/users/find")]
        public IActionResult FindUsers([FromBody] SearchViewModel viewModel)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>

                    (!string.IsNullOrEmpty(x.FirstName) && x.FirstName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.LastName) && x.LastName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))

                );

            }

            switch (viewModel.SortField)
            {
                case "firstname":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.FirstName);
                    else
                        query = query.OrderByDescending(x => x.FirstName);
                    break;

                case "lastname":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.LastName);
                    else
                        query = query.OrderByDescending(x => x.LastName);
                    break;

                case "email":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Email);
                    else
                        query = query.OrderByDescending(x => x.Email);
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

        [HttpPost("account/users")]
        [HttpPut("account/users")]
        public async Task<IActionResult> SaveUser([FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(model);
            }
            if (!string.IsNullOrEmpty(model.Id))
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    foreach (var role in userRoles)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                    await _userManager.AddToRolesAsync(user, model.Roles);
                }
                return Response(result.Succeeded);
            }
            else
            {
                var user = new MedelitUser { FirstName = model.FirstName, LastName = model.LastName, UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var password = model.Password;
                await _userManager.CreateAsync(user, password);
                return Response(user);
            }
        }

        [HttpDelete("account/{userId}")]
        public async Task<IActionResult> DeleteUser(string userid)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userid);
            return Response(await _userManager.DeleteAsync(user));
        }

        [HttpDelete("account/users")]
        public async Task<IActionResult> DeleteUsers([FromBody] IList<MedelitUser> users)
        {
            //users = _context.Users.ToList();
            foreach (var user in users)
            {
                var result = await _userManager.DeleteAsync(user);
            }

            return Response(true);
        }

        [HttpDelete("account/users/add-user-to-role/{role}")]
        public async Task<IActionResult> AddUserToRole(MedelitUser user, string role)
        {
            return Response(await _userManager.AddToRoleAsync(user, role));
        }

        [HttpGet("account/email-taken/{email}")]
        [HttpGet("account/email-taken/{email}/{userId}")]
        public async Task<IActionResult> IsEmailTaken(string email, string userId)
        {
            return Response(await Task.FromResult(_userManager.Users.Where(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) && (x.Id != userId)).Count()));

        }

    }
}
