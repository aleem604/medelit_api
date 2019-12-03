using System;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IAuthService : IDisposable
    {
        #region User
        void Login(LoginViewModel viewModel);
        void Signup(SignupViewModel viewModel);
        void CreateUser(SignupViewModel viewModel);
        void DeleteUser(long userId);
        void UpdateUser(UpdateUserViewModel viewModel);
        void UpdateUserPassword(UpdatePasswordViewModel viewModel);
        void GetUserById(UserViewModel userViewModel);
        void GetAllUsers();
        void GetAccessTokenFromRefreshToken(string refreshToken);
        #endregion User

        #region Roles
        void UpdateRole(RoleReqestViewModel viewModel);
        void CreateRole(CreateRoleReqestViewModel viewModel);
        void DeleteRole(long id);
        void GetRoleById(RoleReqestViewModel viewModel);
        void GetAllRoles(RolesRequestViewModel viewModel);
        void AssignRoles(UserAssignRolesRequestViewModel viewModel);
        void GetUsersByRoleId(RoleReqestViewModel viewModel);
        void RegisterUsersToRole(RoleReqestViewModel viewModel);
        #endregion Roles

        #region permission 
        void AddPermissionToRole(AssignPermissionToRoleViewModel viewModel);
        void CreatePermission(PermissionViewModel viewModel);
        void UpdatePermission(PermissionViewModel viewModel);
        void DeletePermission(PermissionViewModel permissionViewModel);
        void GetAllPermissions(PermissionRequestViewModel viewModel);
        void GetPermissionsByRole(AssignPermissionToRoleViewModel assignPermissionToRoleViewModel);

        #endregion

        AuthClaims ReadTokenClaims();
        
    }
}
