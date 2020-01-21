using System.Collections.Generic;
using System.Linq;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Medelit.Api.Controllers
{
    [Authorize]
    public abstract class ApiController : ControllerBase
    {
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _mediator;

        protected ApiController(INotificationHandler<DomainNotification> notifications,
                                IMediatorHandler mediator)
        {
            _notifications = (DomainNotificationHandler)notifications;
            _mediator = mediator;
        }

        protected IEnumerable<DomainNotification> Notifications => _notifications.GetNotifications();


        protected bool IsValidOperation()
        {
            return (!_notifications.HasNotifications());
        }

        protected new IActionResult Response(object result = null, string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                return Ok(new
                {
                    success = false,
                    errors = message
                });
            }
            else if (IsValidOperation())
            {
                return Ok(new
                {
                    success = true,
                    data = result ?? _notifications.GetDomainData()?.FirstOrDefault()?.Data
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    errors = string.IsNullOrEmpty(message) ? string.Join("<br/> ", _notifications.GetNotifications().Select(n => n.Value).ToArray()) : message
                });
            }
        }

        protected void NotifyModelStateErrors()
        {
            var erros = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var erro in erros)
            {
                var erroMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotifyError(string.Empty, erroMsg);
            }
        }

        protected void NotifyError(string code, string message)
        {
            _mediator.RaiseEvent(new DomainNotification(code, message));
        }

        protected void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                NotifyError(result.ToString(), error.Description);
            }
        }
        protected string GetModelStateErrors()
        {
            var errors = ModelState.Select(x => x.Value.Errors)
                                 .Where(y => y.Count > 0).SelectMany(x => x.Select(y => y.ErrorMessage))
                                 .ToArray();
            return string.Join("<br/> ", errors);
        }
    }
}
