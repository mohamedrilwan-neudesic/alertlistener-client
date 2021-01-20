using System;
using System.Threading.Tasks;

namespace AMNOne.Services.Notifications.Hub
{
    public interface IAlertClient
    {
        Task Broadcast(string user, string level, string message);
        Task Echo(string user, string message);
    }
}
