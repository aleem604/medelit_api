using System.Threading;
using System.Threading.Tasks;
using Medelit.Domain.Events;
using MediatR;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Core.Bus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Medelit.Domain.EventHandlers
{
    public class Auth0EventHandler :
        INotificationHandler<UserRegisteredEvent>
    {
        private readonly IMediatorHandler _bus;
        private readonly IConfiguration _config;
        private readonly ITinUserRepo _tinUserRepo;
        public Auth0EventHandler(IMediatorHandler bus, IConfiguration config, ITinUserRepo tinUserRepo)
        {
            _tinUserRepo = tinUserRepo;
            _bus = bus;
            _config = config;
        }
        public Task Handle(UserRegisteredEvent message, CancellationToken cancellationToken)
        {
           // _tinUserRepo.Add(new Models.TinUser {
           //     UserName = message.UserName,
           //     Email = message.Email,
           //     Password = message.Password,
           //     UserMetadata = JsonConvert.SerializeObject(message.UserMetadata),
           //     AppId = message.AppId
           // });
           //var saved = _tinUserRepo.SaveChanges();

            return Task.CompletedTask;
        }
        
    }
}