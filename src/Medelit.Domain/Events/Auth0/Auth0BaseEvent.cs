using Medelit.Domain.Core.Events;

namespace Medelit.Domain.Events
{
    public class Auth0BaseEvent : Event
    {
        public string Token { get; set; }
        public string Domain { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Code { get; set; }
        public string Connection { get; set; }
        public string AppId { get; set; }
    }
}
