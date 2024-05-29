using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;

namespace Extensions
{
    public interface IDomainHub
    {
        Task SendNotification(string method, string uid, object noti);
    }
    public class DomainHub : Hub, IDomainHub
    {
        private readonly IHubContext<DomainHub> _hubContext;
        public DomainHub(IHubContext<DomainHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendNotification(string method, string uid, object noti)
        {
            await this._hubContext.Clients.Group(uid).SendAsync(method, noti);
        }

        //public override Task OnConnectedAsync()
        //{
        //    //var userId = LoginContext.Instance.CurrentUser?.userId.ToString().ToUpper();
        //    //if (userId != null)
        //    //    Groups.AddToGroupAsync(Context.ConnectionId, userId);
        //    return base.OnConnectedAsync();
        //}

        [HubMethodName("join")]
        public async Task Join(JoinMessage joinMessage)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, joinMessage.id);
        }

        public class JoinMessage
        {
            public string id { get; set; }
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
