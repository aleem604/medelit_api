using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Medelit.Common;
using Medelit.Domain.Commands;
using Medelit.Domain.Core.Bus;

namespace Medelit.Application
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public AuthService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus)
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Login(LoginViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<LoginCommand>(viewModel));
        }
        public void Signup(SignupViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<SignupCommand>(viewModel));
        }
        public void CreateUser(SignupViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<UserCreateCommand>(viewModel));
        }
        public void DeleteUser(long userId)
        {
            _bus.SendCommand(new DeleteUserCommand { UserId = userId });
        }
        public void UpdateUser(UpdateUserViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<UpdateUserCommand>(viewModel));
        }
        public void UpdateUserPassword(UpdatePasswordViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<UpdateUserPasswordCommand>(viewModel));
        }

        public void GetUserById(UserViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<GetUserCommand>(viewModel));
        }
        public void GetAllUsers()
        {
            _bus.SendCommand(new GetAllUsersCommand());
        }
        public void AssignRoles(UserAssignRolesRequestViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<UserAssignRolesCommand>(viewModel));
        }
        public void GetAccessTokenFromRefreshToken(string refreshToken)
        {
            _bus.SendCommand(new AccessTokenFromRefreshTokenCommand { RefreshToken = refreshToken });
        }

        public void CreateRole(CreateRoleReqestViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<CreateRoleCommand>(viewModel));
        }
        public void GetRoleById(RoleReqestViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<GetRoleCommand>(viewModel));
        }

        public void GetAllRoles(RolesRequestViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<GetRolesCommand>(viewModel));
        }

        public void DeleteRole(long roleId)
        {
            _bus.SendCommand(new DeleteRoleCommand { Id = roleId });
        }

        public void UpdateRole(RoleReqestViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<UpdateRoleCommand>(viewModel));
        }

        public void GetUsersByRoleId(RoleReqestViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<GetUsersByRoleIdCommand>(viewModel));
        }

        public void RegisterUsersToRole(RoleReqestViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<RegisterUsersToRoleCommand>(viewModel));
        }

        public void AddPermissionToRole(AssignPermissionToRoleViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<AssignPermissionToRoleCommand>(viewModel));
        }
        public void CreatePermission(PermissionViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<CreatePermissionCommand>(viewModel));
        }
        public void UpdatePermission(PermissionViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<UpdatePermissionCommand>(viewModel));
        }

        public void DeletePermission(PermissionViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<DeletePermissionCommand>(viewModel));
        }


        public void GetAllPermissions(PermissionRequestViewModel viewModel)
        {
            _bus.SendCommand(_mapper.Map<GetPermissionsCommand>(viewModel));
        }

        public void GetPermissionsByRole(AssignPermissionToRoleViewModel viewModel)
        {
            _bus.SendCommand(new GetPermissionsByRoleCommand { RoleId = viewModel.RoleId });
        }

        public AuthClaims ReadTokenClaims()
        {
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal user = _httpContext.HttpContext.User;
                var claims = user.Claims;
                //var jwtPayload = new AuthClaims
                //{
                //  UserId = claims.Where(x => x.Type.Contains("nameidentifier", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value.ToString(),
                //  Name = claims.Where(x => x.Type.Equals("name", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value.ToString(),
                //  Email = claims.Where(x => x.Type.Contains("emailaddress", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value.ToString(),
                //  GivenName = claims.Where(x => x.Type.Contains("givenname", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value.ToString(),
                //  SurName = claims.Where(x => x.Type.Contains("givenname", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value.ToString(),
                //  NickName = claims.Where(x => x.Type.Equals("nickname", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value.ToString(),
                //  Picture = claims.Where(x => x.Type.Equals("picture", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value.ToString()
                //};
                //var updatedAt = claims.Where(x => x.Type.Equals("updated_at", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value.ToString();
                //if (DateTime.TryParse(JsonConvert.DeserializeObject<string>(updatedAt), out DateTime outDate))
                //  jwtPayload.UpdatedAt = outDate;
                //jwtPayload.UserId = jwtPayload.UserId.Replace("auth0|", "");

                return new AuthClaims();
            }
            catch
            {
                return new AuthClaims();
            }
        }

    }
}