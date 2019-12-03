using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Medelit.Domain.Core.Notifications
{
    public class DomainNotificationHandler : INotificationHandler<DomainNotification>
    {
        private List<DomainNotification> _notifications;
        private List<DomainNotification> _domainData;

        public DomainNotificationHandler()
        {
            _notifications = new List<DomainNotification>();
            _domainData = new List<DomainNotification>();
        }

        public Task Handle(DomainNotification message, CancellationToken cancellationToken)
        {
            if (message.Value != null)
                _notifications.Add(message);
            else
                _domainData.Add(message);

            return Task.CompletedTask;
        }

        public virtual List<DomainNotification> GetNotifications()
        {
            return _notifications;
        }

        public virtual List<DomainNotification> GetDomainData()
        {
            return _domainData;
        }

        public virtual bool HasNotifications()
        {
            return GetNotifications().Any();
        }

        public void Dispose()
        {
            _notifications = new List<DomainNotification>();
            _domainData = new List<DomainNotification>();
        }
    }
}