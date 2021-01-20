using Chatty.Api.Models;

using System.Threading.Tasks;

namespace Chatty.Api.Hubs.Clients
{
    public interface IChatClient
    {
        Task GetMessage(ChatMessage message);
    }
}