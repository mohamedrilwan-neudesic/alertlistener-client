using System.Threading.Tasks;

namespace Chatty.Without.API.Hubs.Clients
{
    public interface IAlertClient
    {
        Task Broadcast(string user, string level, string message);
        Task Echo(string user, string message);
        Task OnConnected(string connectionId);
    }
}
