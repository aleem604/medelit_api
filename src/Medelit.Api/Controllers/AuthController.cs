using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;

namespace Medelit.Api.Controllers
{
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<AuthController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _authService = authService;
            _notifications = notifications;
            _logger = logger;
        }
        /*----------------Start Users Area--------------------------------------------------*/
        //#region Users
        //[HttpPost("auth/login")]
        //public IActionResult PostLogin([FromBody]LoginViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //        _authService.Login(viewModel);
        //    else
        //        return Response(GetModelStateErrors());
        //    return Response();
        //}

        //[HttpPost("auth/signup")]
        //public IActionResult PostSignup([FromBody]SignupViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //        _authService.Signup(viewModel);
        //    else
        //        return Response(GetModelStateErrors());

        //    return Response();
        //}

        //[Authorize, HttpPost("auth/create_user")]
        //public IActionResult CreateUser([FromBody]SignupViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //        _authService.CreateUser(viewModel);
        //    else
        //        return Response(GetModelStateErrors());

        //    return Response();
        //}
        //[Authorize, HttpDelete("auth/delete_user/{id}")]
        //public IActionResult DeleteUser(long id)
        //{
        //    _authService.DeleteUser(id);
        //    return Response();
        //}
        //[Authorize, HttpPost("auth/update_user")]
        //public IActionResult UpdateUser([FromBody]UpdateUserViewModel viewModel)
        //{
        //    _authService.UpdateUser(viewModel);
        //    return Response();
        //}

        //[Authorize, HttpPost("auth/update_user_password")]
        //public IActionResult UpdateUserPassword([FromBody]UpdatePasswordViewModel viewModel)
        //{
        //    if(ModelState.IsValid)
        //    _authService.UpdateUserPassword(viewModel);
        //    else
        //        return Response(GetModelStateErrors());
        //    return Response();
        //}

        //[Authorize, HttpPost("auth/user_by_id")]
        //public IActionResult UserById([FromBody] UserViewModel viewModel)
        //{
        //    _authService.GetUserById(viewModel);
        //    return Response();
        //}

        //[Authorize, HttpGet("auth/users")]
        //public IActionResult GetAllUsers()
        //{
        //    _authService.GetAllUsers();
        //    return Response();
        //}

        //[Authorize, HttpPost("auth/users_assign_roles")]
        //public IActionResult UsersAssignRoles([FromBody] UserAssignRolesRequestViewModel viewModel)
        //{
        //    _authService.AssignRoles(viewModel);
        //    return Response();
        //}
        //[HttpGet("auth/token_from_refresh_token/{id}")]
        //public IActionResult GetAccessTokenFromRefreshToken(string id)
        //{
        //    _authService.GetAccessTokenFromRefreshToken(id);
        //    return Response();
        //}

        //#endregion Users

        ///*----------------Start Roles Area--------------------------------------------------*/
        //#region Roles

        //[Authorize, HttpPost("auth/create_role")]
        //public IActionResult CreateRole([FromBody] CreateRoleReqestViewModel viewModel)
        //{
        //    _authService.CreateRole(viewModel);
        //    return Response();
        //}
        //[Authorize, HttpPost("auth/update_role")]
        //public IActionResult UpdateRole([FromBody] RoleReqestViewModel viewModel)
        //{
        //    _authService.UpdateRole(viewModel);
        //    return Response();
        //}
        //[Authorize, HttpDelete("auth/delete_role/{id}")]
        //public IActionResult DeleteRole(long id)
        //{
        //    _authService.DeleteRole(id);
        //    return Response();
        //}
        //[Authorize, HttpGet("auth/get_role_by_id/{id}")]
        //public IActionResult GetRoleById(string version, long id)
        //{
        //    _authService.GetRoleById(new RoleReqestViewModel {Id = id });
        //    return Response();
        //}
       
        //[Authorize, HttpGet("auth/roles")]
        //public IActionResult GetAllRoles()
        //{
        //    _authService.GetAllRoles(new RolesRequestViewModel());
        //    return Response();
        //}

        //[Authorize, HttpGet("auth/users_by_role/{id}")]
        //public IActionResult GetUsersByRoleId(long id)
        //{
        //    _authService.GetUsersByRoleId(new RoleReqestViewModel { Id = id});
        //    return Response();
        //}

        //[Authorize, HttpPost("auth/register_users_to_role")]
        //public IActionResult RegisterUsersToRole([FromBody] RoleReqestViewModel viewModel)
        //{
        //    _authService.RegisterUsersToRole(viewModel);
        //    return Response();
        //}

        //#endregion Roles

        //#region permissons

        //[Authorize, HttpPost("auth/create_permission")]
        //public IActionResult CreatePermission([FromBody] PermissionViewModel viewModel)
        //{
        //    _authService.CreatePermission(viewModel);
        //    return Response();
        //}

        //[Authorize, HttpPost("auth/update_permission")]
        //public IActionResult UpdatePermission([FromBody] PermissionViewModel viewModel)
        //{
        //    _authService.UpdatePermission(viewModel);
        //    return Response();
        //}

        //[Authorize, HttpDelete("auth/delete_permission/{id}")]
        //public IActionResult DeletePermission(long id)
        //{
        //    _authService.DeletePermission(new PermissionViewModel {Id = id });
        //    return Response();
        //}

        //[Authorize, HttpGet("auth/permissions")]
        //public IActionResult GetAllPermissions()
        //{
        //    _authService.GetAllPermissions(new PermissionRequestViewModel());
        //    return Response();
        //}

        //[Authorize, HttpPost("auth/add_permission_to_role")]
        //public IActionResult AddPermissionToRole([FromBody] AssignPermissionToRoleViewModel viewModel)
        //{
        //    _authService.AddPermissionToRole(viewModel);
        //    return Response();
        //}

        //[Authorize, HttpGet("auth/get_permissions_by_role/{id}")]
        //public IActionResult GetPermissionsByRole(long id)
        //{
        //    _authService.GetPermissionsByRole(new AssignPermissionToRoleViewModel {RoleId = id });
        //    return Response();
        //}

        //#endregion permissions



    }
}