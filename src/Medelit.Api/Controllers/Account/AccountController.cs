using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Medelit.Api.Controllers;
using Medelit.Application;
using Medelit.Auth;
using Medelit.Common;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Helpers;
using Medelit.Infra.CrossCutting.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Medelit.Api.Controllers
{
    public class AccountController : ApiController
    {
        private readonly UserManager<MedelitUser> _userManager;
        private readonly RoleManager<MedelitRole> _roleManager;
        private readonly SignInManager<MedelitUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly INotificationHandler<DomainNotification> _notifications;

        public AccountController(
            RoleManager<MedelitRole> rolesManager,
            UserManager<MedelitUser> userManager,
            SignInManager<MedelitUser> signInManager,
            INotificationHandler<DomainNotification> notifications,
            ILoggerFactory loggerFactory,
            IMediatorHandler mediator,
            IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions) : base(notifications, mediator)
        {
            _roleManager = rolesManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _notifications = notifications;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("account/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(null, "Required data is missing");
            }

            var identity = await GetClaimsIdentity(model.Email, model.Password);
            if (identity == null)
            {
                return Response(null, "Invalid email or password");
            }
            CurrentUserInfo userInfo = await GetUserInfo(model.Email);
            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, userInfo, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
            return Ok(new
            {
                success = true,
                data = JsonConvert.DeserializeObject(jwt)
            });
        }

        //[AllowAnonymous]
        [HttpPost("account/register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(model);
            }

            var user = new MedelitUser { FirstName = model.FirstName, LastName = model.LastName, UserName = model.Email, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // User claim for write customers data
                await _userManager.AddClaimAsync(user, new Claim("Customers", "Write"));
                await _userManager.AddToRoleAsync(user, nameof(UserRoles.Admin));

                await _signInManager.SignInAsync(user, false);
                _logger.LogInformation(3, "User created a new account with password.");
                return Response(model);
            }

            AddIdentityErrors(result);
            return Response(model);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                var roles = await _userManager.GetRolesAsync(userToVerify);
                var currentUser = await GetUserInfo(userName);
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(currentUser, userToVerify, roles));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }

        private async Task<CurrentUserInfo> GetUserInfo(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(UserRoles.Admin.ToString()))
            {
                foreach (var role in Enum.GetValues(typeof(UserRoles)))
                {
                    roles.Add(role.ToString());
                }
            }
            return new CurrentUserInfo
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            };
        }


    }
}
