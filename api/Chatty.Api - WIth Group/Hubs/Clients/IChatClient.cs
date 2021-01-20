
using System.Threading.Tasks;

namespace Chatty.Api.Hubs.Clients
{
    public interface IChatClient
    {
        Task Broadcast(string user, string level, string message);
        Task Echo(string user, string message);
    }
}