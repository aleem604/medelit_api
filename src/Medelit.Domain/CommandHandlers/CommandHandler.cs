using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using MediatR;
using System;
using Medelit.Common;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Reflection;

namespace Medelit.Domain.CommandHandlers
{
    public class CommandHandler
    {
        private readonly IUnitOfWork _uow;
        private readonly IMediatorHandler _bus;
        private readonly IHttpContextAccessor _httpContext;
        private readonly DomainNotificationHandler _notifications;
        public CommandHandler(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IHttpContextAccessor httpContext, IUnitOfWork uow)
        {
            _uow = uow;
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
        //public static void CopyPropertiesTo(this object fromObject, object toObject)
        //{
        //    PropertyInfo[] toObjectProperties = toObject.GetType().GetProperties();
        //    foreach (PropertyInfo propTo in toObjectProperties)
        //    {
        //        PropertyInfo propFrom = fromObject.GetType().GetProperty(propTo.Name);
        //        if (propFrom != null && propFrom.CanWrite)
        //            propTo.SetValue(toObject, propFrom.GetValue(fromObject, null), null);
        //    }
        //}

        public bool Commit()
        {
            if (_notifications.HasNotifications()) return false;
            try
            {
                return _uow.Commit();
                
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("Commit", $"We had a problem during saving your data. {ex.Message}"));
                return false;
            }
        }
    }
}