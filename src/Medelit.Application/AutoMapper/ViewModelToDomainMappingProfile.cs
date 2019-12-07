using AutoMapper;
using Medelit.Common;
using Medelit.Domain;
using Medelit.Domain.Commands;
using Medelit.Domain.Events;
using Medelit.Domain.Models;

namespace Medelit.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            //Auth
            CreateMap<LoginViewModel, LoginCommand>();
            CreateMap<SignupViewModel, SignupCommand>();
            CreateMap<SignupCommand, TinUser>();
            CreateMap<LoginViewModel, UserCreateCommand>(MemberList.Source);

            CreateMap<SignupViewModel, UserCreateCommand>(MemberList.Source);
            CreateMap<UserCreateCommand, TinUser>(MemberList.Destination);

            CreateMap<SignupCommand, UserRegisteredEvent>();

            CreateMap<UserRequestViewModel, GetAllUsersCommand>(MemberList.Source);
            CreateMap<UserViewModel, GetUserCommand>(MemberList.Source);
            
            CreateMap<DeleteUserViewModel, DeleteUserCommand>(MemberList.Source);
            CreateMap<UpdateUserViewModel, UpdateUserCommand>(MemberList.Source);
            CreateMap<UpdatePasswordViewModel, UpdateUserPasswordCommand>(MemberList.Source);
            CreateMap<UserAssignRolesRequestViewModel, UserAssignRolesCommand>(MemberList.Source);
            CreateMap<AccessTokenFromRefreshTokenViewModel, AccessTokenFromRefreshTokenCommand>(MemberList.Source);

            CreateMap<CreateRoleReqestViewModel, CreateRoleCommand>();
            CreateMap<RoleReqestViewModel, DeleteRoleCommand>();
            CreateMap<RoleReqestViewModel, UpdateRoleCommand>((MemberList.Source));
            CreateMap<RoleReqestViewModel, GetRoleCommand>(MemberList.Source);
            CreateMap<RolesRequestViewModel, GetRolesCommand>((MemberList.Source));
            CreateMap<RoleReqestViewModel, GetUsersByRoleIdCommand>((MemberList.Source));
            CreateMap<RoleReqestViewModel, RegisterUsersToRoleCommand>((MemberList.Source));

            CreateMap<AssignPermissionToRoleViewModel, AssignPermissionToRoleCommand>((MemberList.Source));
            CreateMap<PermissionViewModel, CreatePermissionCommand>((MemberList.Source));
            CreateMap<PermissionViewModel, UpdatePermissionCommand>((MemberList.Source));
            CreateMap<PermissionViewModel, DeletePermissionCommand>((MemberList.Source));
            CreateMap<PermissionRequestViewModel, GetPermissionsCommand>((MemberList.Source));
            CreateMap<PermissionRequestViewModel, GetPermissionsByRoleCommand>((MemberList.Source));


            //CreateMap<Source, Destination>()
            //    .ForMember(d => d.Text, o => o.MapFrom(s => s.Name))
            //    .ForMember(d => d.Value, o => o.MapFrom(s => s.Id))
            //    .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<FeeViewModel, Fee>((MemberList.Source));
            CreateMap<ProfessionalRequestViewModel, Professional>((MemberList.Destination));
            CreateMap<FilterModel, ProfessionalLanguageRelation>()
               .ForMember(dest => dest.LanguageId,
                   opts => opts.MapFrom(
                       src => src.Id
                   )).ReverseMap();
            // Services
            CreateMap<ServiceViewModel, Service>((MemberList.Source));


        }
    }
}
