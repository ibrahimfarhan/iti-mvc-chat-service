using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITI.MVC.ChatService.Models.Store;
using Microsoft.AspNet.SignalR;

namespace ITI.MVC.ChatService.SignalR
{
    public class ChatHub : Hub
    {
        private ApplicationDbContext dbCtx;

        public ApplicationDbContext DbCtx
        {
            get => dbCtx ?? ApplicationDbContext.Create();

            private set => dbCtx = value;
        }

        public void Hello()
        {
            var l = dbCtx.Users.ToList();
            Clients.All.hello();
        }

        public void SendMessage(string targetUserId, string message)
        {

        }

        public void DisplayTyping(string targetUserId)
        {

        }
    }
}