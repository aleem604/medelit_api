using System;
using System.Collections.Generic;
using Medelit.Domain.Core.Events;

namespace Medelit.Infra.Data.Repository.EventSourcing
{
    public interface IEventStoreRepository : IDisposable
    {
        void Store(StoredEvent theEvent);
        IList<StoredEvent> All(Guid aggregateId);
    }
}