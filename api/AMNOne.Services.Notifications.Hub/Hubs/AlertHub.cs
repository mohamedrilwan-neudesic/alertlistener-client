using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace AMNOne.Services.Notifications.Hub.Hubs
{
    public class AlertHub : Microsoft.AspNetCore.SignalR.Hub<IAlertClient>
    {
        /// <summary>
        /// Broadcase a message to a single user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public Task BroadcastToUser(string user, string message, string level = AlertLevels.NORMAL)
        {
            return Clients.User(user).Broadcast( user, level, message);
        }

        /// <summary>
        /// Broadcast a alert of moderate level to a single user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task BroadcastModerateAlert(string user, string message)
        {
            return Clients.User(user).Broadcast(user, AlertLevels.MODERATE, message);
        }

        /// <summary>
        /// Broadcast a alert of hight level to a single user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task BroadcastHighAlert(string user, string message)
        {
            return Clients.User(user).Broadcast(user, AlertLevels.HIGH, message);
        }

        /// <summary>
        /// Broadcast a alert of severe level to a single user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task BroadcastSevereAlert(string user, string message)
        {
            return Clients.User(user).Broadcast(user, AlertLevels.SEVERE, message);
        }

        /// <summary>
        /// Broadcast a message to a group of users
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public Task BroadcastToGroup(string group, string user, string message, string level = AlertLevels.NORMAL)
        {
            return Clients.Group(group).Broadcast(user, level, message);
        }

        /// <summary>
        /// Broadcast an alert of moderate level to a group of users
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public Task BroadcastModerateAlertToGroup(string group, string user, string message)
        {
            return Clients.Group(group).Broadcast(user, AlertLevels.MODERATE, message);
        }

        /// <summary>
        /// Broadcast an alert of high level to a group of users
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public Task BroadcastHighAlertToGroup(string group, string user, string message)
        {
            return Clients.Group(group).Broadcast(user, AlertLevels.HIGH, message);
        }

        /// <summary>
        /// Broadcast an alert of severe level to a group of users
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public Task BroadcastSevereAlertToGroup(string group, string user, string message)
        {
            return Clients.Group(group).Broadcast(user, AlertLevels.SEVERE, message);
        }

        /// <summary>
        /// This will send to all connected users.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task BroadcastToAllUsers(string user, string message, string level = AlertLevels.NORMAL)
        {
            await Clients.All.Broadcast(user, level, message);
        }


        public void Echo(string user, string message)
        {
            Clients.Client(Context.ConnectionId).Echo(user, message + " (echo from server)");
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGroup(string name, string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).Echo("_SYSTEM_", $"{name} joined {groupName} with connectionId {Context.ConnectionId}");
        }

        public async Task LeaveGroup(string name, string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Client(Context.ConnectionId).Echo("_SYSTEM_", $"{name} leaved {groupName}");
            await Clients.Group(groupName).Echo("_SYSTEM_", $"{name} leaved {groupName}");
        }
    }
}

