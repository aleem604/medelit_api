using System;
using System.Threading;
using System.Threading.Tasks;
using Medelit.Domain.Commands;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Medelit.Common;
using Medelit.Domain.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Domain.CommandHandlers
{
    public class AuthCommandHandler : CommandHandler,
        IRequestHandler<LoginCommand, bool>,
        IRequestHandler<SignupCommand, bool>,
        IRequestHandler<UserCreateCommand, bool>,
        IRequestHandler<DeleteUserCommand, bool>,
        IRequestHandler<UpdateUserCommand, bool>,
        IRequestHandler<UpdateUserPasswordCommand, bool>,
        IRequestHandler<GetUserCommand, bool>,
        IRequestHandler<GetAllUsersCommand, bool>,
        IRequestHandler<UserAssignRolesCommand, bool>,
        IRequestHandler<AccessTokenFromRefreshTokenCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly IConfiguration _config;
        private readonly ITinUserRepo _tinUserRepo;
        private readonly ITinRoleRepo _tinRoleRepo;
        private readonly ITinUserRoleRepo _tinUserRoleRepo;
        private readonly ITinPermissionRepo _tinPermissionRepo;
        public AuthCommandHandler(IMapper mapper,
            IConfiguration config,
            IMediatorHandler bus,
            ITinUserRepo tinUserRepo,
            ITinRoleRepo tinRoleRepo,
            ITinPermissionRepo tinPermissionRepo,
            ITinUserRoleRepo tinUserRoleRepo,
            IHttpContextAccessor httpContext,
            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext)
        {
            _mapper = mapper;
            _config = config;
            _tinUserRepo = tinUserRepo;
            _tinRoleRepo = tinRoleRepo;
            _tinPermissionRepo = tinPermissionRepo;
            _tinUserRoleRepo = tinUserRoleRepo;
            _bus = bus;
        }

        public Task<bool> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _tinUserRepo.GetAll().FirstOrDefault(x => x.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase));
                if (user is null)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.EMAIL_ALREADY_TAKEN));
                    return Task.FromResult(false);
                }

                if (request.Verify(request.Password, user.Password))
                {
                    user.Password = null;
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.GenerateToken(user, _config)));
                    return Task.FromResult(true);
                }
                else
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.API_DATA_INVALID));
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(SignupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (_tinUserRepo.GetAll().Where(x => x.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.EMAIL_ALREADY_TAKEN));
                    return Task.FromResult(false);
                }

                var tinuser = _mapper.Map<TinUser>(request);
                tinuser.Password = request.Hash(request.Password);
                tinuser.CreatedBy = CurrentUser.UserId;
                tinuser.UserType = eUserType.Consumer;

                
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                    return Task.FromResult(false);
                
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(UserCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (_tinUserRepo.GetAll().Where(x => x.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.EMAIL_ALREADY_TAKEN));
                    return Task.FromResult(false);
                }

                var tinuser = _mapper.Map<TinUser>(request);
                tinuser.Password = request.Hash(request.Password);
                tinuser.CreatedBy = CurrentUser.UserId;
                tinuser.UserType = request.UserType;

                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, "User Created Successfully."));
                return Task.FromResult(true);

                //_bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                //return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (_tinUserRepo.GetAll().Where(x => x.Id == request.UserId && x.CreatedBy == CurrentUser.Id) is null)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.RECORD_NOT_FOUND));
                    return Task.FromResult(false);
                }
                
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, "User Delete Successfully."));
                    return Task.FromResult(true);
               
                
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, _tinUserRepo.GetAll().Where(x => x.CreatedBy.Equals(CurrentUser.Id)).ToList()));
                return Task.FromResult(true);   
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPassword.Equals(request.OldPassword))
            {
                _bus.RaiseEvent(new DomainNotification(request.MessageType, "Old and New password must be different."));
                return Task.FromResult(false);
            }
            try
            {
                if (!(request.Email.Equals(CurrentUser.Email, StringComparison.InvariantCultureIgnoreCase)))
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.API_DATA_INVALID));
                    return Task.FromResult(false);
                }
                //if (request.Verify(request.OldPassword, user.Password))
                //{
                  
                        _bus.RaiseEvent(new DomainNotification(request.MessageType, null, "Password changed successfully"));
                        return Task.FromResult(true);
                    
              //  }
                //_bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.API_DATA_INVALID));
                //return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(GetUserCommand request, CancellationToken cancellationToken)
        {
            try
            {

                _bus.RaiseEvent(new DomainNotification(request.MessageType, null));
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
        {
            var id = CurrentUser.Id;
            try
            {
                var users = _tinUserRepo.GetAll().Where(x => x.CreatedBy == CurrentUser.Id).Include(x => x.TinUserRole)
                    .Select((u) => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.ImageUrl,
                        u.PhoneNumber,
                        u.PrimaryAddress,
                        u.SecondaryAddress,
                        u.Status,
                        u.CreateDate,
                        CreatedBy = string.Concat(CurrentUser.FirstName, " ", CurrentUser.LastName),
                        u.TinUserRole
                    });

                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, users));
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(UserAssignRolesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.UserId = request.UserId.IndexOf("auth0") == -1 ? $"auth0|{request.UserId}" : request.UserId;
                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, ""));
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(AccessTokenFromRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var email = request.Decrypt(request.RefreshToken);
                var user = _tinUserRepo.GetAll().Where(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.GenerateToken(user, _config)));
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        private Task<bool> HandleException(string messageType, Exception ex)
        {
            _bus.RaiseEvent(new DomainNotification(messageType, ex.Message));
            return Task.FromResult(false);
        }
        public void Dispose()
        {

        }


    }
}