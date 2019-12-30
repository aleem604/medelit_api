using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Infra.CrossCutting.Identity.Data;
using Medelit.Infra.CrossCutting.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Medelit.Api.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            INotificationHandler<DomainNotification> notifications,
            ILoggerFactory loggerFactory,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _context = context;
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<UserController>();
        }

        [HttpGet("users/current-user")]
        public async Task<IActionResult> GetCurrentUsers()
        {
            return Response(await _userManager.FindByEmailAsync(User.Identity.Name));
        }



        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            return Response(await Task.FromResult(_context.Users.ToList()));
        }


        [HttpPost("users")]
        [HttpPut("users")]
        public async Task<IActionResult> SaveUser([FromBody] ApplicationUser model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(model);
            }
            if (!string.IsNullOrEmpty(model.Id))
            {
                var result = _userManager.UpdateAsync(model);
                return Response(result);
            }
            else
            {
                var user = new ApplicationUser { FirstName = model.FirstName, LastName = model.LastName, UserName = model.Email, Email = model.Email };
                var password = "Password!@#";
                var result = await _userManager.CreateAsync(user, password);
                return Response(result);
            }
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userid)
        {
            var user = _context.Users.FirstOrDefault(x=>x.Id == userid);
            return Response(await _userManager.DeleteAsync(user));
        }

        [HttpDelete("users")]
        public async Task<IActionResult> DeleteUsers([FromBody] IList<ApplicationUser> users)
        {
            //users = _context.Users.ToList();
            foreach (var user in users)
            {
                var result = await _userManager.DeleteAsync(user);
            }

            return Response(true);
        }

        [HttpDelete("users/add-user-to-role/{role}")]
        public async Task<IActionResult> AddUserToRole(ApplicationUser user, string role)
        {
            return Response(await _userManager.AddToRoleAsync(user, role));
        }

    }
}
