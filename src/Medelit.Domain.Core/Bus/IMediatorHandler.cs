using System.Threading.Tasks;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Core.Events;


namespace Medelit.Domain.Core.Bus
{
    public interface IMediatorHandler
    {
        Task SendCommand<T>(T command) where T : Command;
        Task RaiseEvent<T>(T @event) where T : Event;
    }
}
