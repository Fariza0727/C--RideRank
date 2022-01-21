using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using RR.Data;
using RR.Repo;
using RR.Service;
using RR.Web.SignalRHubs;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppWeb.SignalRHubs
{

    public class ChatHub : Hub
    {
        public static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        private readonly IRepository<UserDetail, RankRideContext> _repoUserDetail;

        public ChatHub(IRepository<UserDetail, RankRideContext> repoUserDetail )
        {
            _repoUserDetail = repoUserDetail;
        }

        public Task SendMessageToAll(string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessageToUser(string connectionId, string message)
        {
            foreach (var _connectionId in _connections.GetConnections(connectionId))
            {
                await Clients.Client(_connectionId).SendAsync("ReceiveMessage", message);
            }
        }

        public async Task JoinGroup(long contestId, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, contestId.ToString());
            var user_ = _repoUserDetail.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();
            var userInfo = new {groupid = contestId,  userid = userId, username = user_.UserName, pic = user_.Avtar };
            await Clients.Group(contestId.ToString()).SendAsync("JoinedContest", userInfo);
        }

        public async Task JoinUser(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                _connections.Add(userId, Context.ConnectionId);
                //await Clients.Client(userId).SendAsync("UserConnected", userId);
                await Clients.All.SendAsync("UserConnected", userId);
                await base.OnConnectedAsync();
            }
        }

        public Task SendMessageToGroup(string group, string message)
        {
            return Clients.Group(group).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                _connections.Add(userId, Context.ConnectionId);
                //await Clients.Client(userId).SendAsync("UserConnected", userId);
                await Clients.All.SendAsync("UserConnected", userId);
                
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {

            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                _connections.Remove(userId, Context.ConnectionId);
                //await Clients.Client(userId).SendAsync("UserDisconnected", userId);
                await Clients.All.SendAsync("UserDisconnected", userId);
                
            }
            await base.OnDisconnectedAsync(ex);
        }

       


    }

   

}
