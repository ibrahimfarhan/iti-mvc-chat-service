using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ITI.MVC.ChatService.Models.Entities;
using ITI.MVC.ChatService.Models.Enums;
using ITI.MVC.ChatService.Models.Store;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace ITI.MVC.ChatService.SignalR
{
    public class ChatHub : Hub
    {
        private ApplicationDbContext dbCtx = ApplicationDbContext.Create();

        public ApplicationDbContext DbCtx
        {
            get => dbCtx ?? ApplicationDbContext.Create();

            private set => dbCtx = value;
        }

        public override Task OnConnected()
        {
            var chatConnection = new ChatConnection
            {
                Id = Context.ConnectionId,
                UserId = Context.User.Identity.GetUserId()
            };

            DbCtx.ChatConnections.Add(chatConnection);
            DbCtx.SaveChanges();

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var chatConnection = DbCtx.ChatConnections.FirstOrDefault(c => c.Id == Context.ConnectionId);

            if (chatConnection != null)
            {
                DbCtx.ChatConnections.Remove(chatConnection);
                DbCtx.SaveChanges();
            }

            return base.OnDisconnected(stopCalled);
        }

        public void SendMessage(string targetUserId, string message)
        {
            ChatMessage chatMessage = new ChatMessage
            {
                Message = message,
                SenderId = Context.User.Identity.GetUserId(),
                ReceiverId = targetUserId,
                MessageStatus = MessageStatus.Sent
            };

            DbCtx.ChatMessages.Add(chatMessage);
            
            if (DbCtx.SaveChanges() > 0)
            {
                var targetUserConnections = DbCtx.ChatConnections.Where(c => c.UserId == targetUserId).Select(c => c.Id).ToList();
                var senderConnections = DbCtx.ChatConnections.Where(c => c.UserId == chatMessage.SenderId).Select(c => c.Id).ToList();

                Clients.Clients(targetUserConnections).displayIncomingMessage(Context.User.Identity.GetUserName(), message);
                Clients.Clients(senderConnections).displayOutgoingMessage(Context.User.Identity.GetUserName(), message, "Sent");
            }
        }

        public void DisplayTyping(string targetUserId)
        {
            var targetUserConnections = DbCtx.ChatConnections.Where(c => c.UserId == targetUserId).Select(c => c.Id).ToList();

            Clients.Clients(targetUserConnections).displayTyping(Context.User.Identity.GetUserId());
        }

        public void RemoveTyping(string targetUserId)
        {
            var targetUserConnections = DbCtx.ChatConnections.Where(c => c.UserId == targetUserId).Select(c => c.Id).ToList();

            Clients.Clients(targetUserConnections).removeTyping(Context.User.Identity.GetUserId());
        }
    }
}