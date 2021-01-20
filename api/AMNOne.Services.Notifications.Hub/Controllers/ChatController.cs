

using AMNOne.Services.Notifications.Hub;
using AMNOne.Services.Notifications.Hub.Hubs;
using AMNOne.Services.Notifications.Hub.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using System.Threading.Tasks;

namespace Chatty.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<AlertHub, IAlertClient> _chatHub;

        public ChatController(IHubContext<AlertHub, IAlertClient> chatHub)
        {
            _chatHub = chatHub;
        }

        [HttpPost("messages")]
        public async Task Post(ChatMessage message)
        {
            // run some logic...    

            await _chatHub.Clients.All.Broadcast(message.User, message.Type, message.Message);
        }

        //[HttpPost("messages")]
        //public async Task Group(ChatMessage message)
        //{
        //    // run some logic...    

        //    await _chatHub.Clients.User("Rilwan").ReceiveMessage(message);
        //}

    }
}
