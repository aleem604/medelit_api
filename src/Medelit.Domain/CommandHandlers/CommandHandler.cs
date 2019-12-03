using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using MediatR;
using System;
using Medelit.Common;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Medelit.Domain.CommandHandlers
{
    public class CommandHandler
    {
        private readonly IMediatorHandler _bus;
        private readonly IHttpContextAccessor _httpContext;
        private readonly DomainNotificationHandler _notifications;
        public CommandHandler(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IHttpContextAccessor httpContext)
        {
            _notifications = (DomainNotificationHandler)notifications;
            _bus = bus;
            _httpContext = httpContext;
        }

        public AuthClaims CurrentUser
        {
            get
            {
                return _httpContext.HttpContext.Items.Where(x => x.Key.Equals(eTinUser.TinUser)).FirstOrDefault().Value as AuthClaims;
            }
        }

        protected void NotifyValidationErrors(Command message)
        {
            foreach (var error in message.ValidationResult.Errors)
            {
                _bus.RaiseEvent(new DomainNotification(message.MessageType, error.ErrorMessage));
            }
        }

        public bool Commit()
        {
            if (_notifications.HasNotifications()) return false;
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("Commit", $"We had a problem during saving your data. {ex.Message}"));
                return false;
            }
        }
    }
}