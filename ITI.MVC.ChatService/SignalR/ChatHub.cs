using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
            // Add a connection for the current user.

            var chatConnection = new ChatConnection
            {
                Id = Context.ConnectionId,
                UserId = Context.User.Identity.GetUserId()
            };

            DbCtx.ChatConnections.Add(chatConnection);

            if (DbCtx.SaveChanges() == 0)
            {
                return base.OnConnected();
            }

            // Mark messages sent to this user as received.

            var messages = DbCtx.ChatMessages.Where(c => c.ReceiverId == chatConnection.UserId && c.MessageStatus == MessageStatus.Sent).ToList();
            messages.ForEach(m => m.MessageStatus = MessageStatus.Received);

            if (DbCtx.SaveChanges() <= 0)
            {
                return base.OnConnected();
            }

            // Update the senders of these messages with the new message status.

            var senderIds = messages.Select(m => m.SenderId).ToList();
            var senderConnections = DbCtx.ChatConnections.Where(c => senderIds.Contains(c.UserId)).
                Select(c => c.Id).ToList();

            foreach (var message in messages)
            {
                Clients.Clients(senderConnections).changeMessageStatus(message.Id, message.MessageStatus.ToString());
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            RemoveCurrentConnection();

            return base.OnDisconnected(stopCalled);
        }

        public void SendMessage(string targetUserId, string message)
        {
            var targetUserConnections = DbCtx.ChatConnections.Where(c => c.UserId == targetUserId).Select(c => c.Id).ToList();
            var messageStatus = targetUserConnections.Count == 0 ? MessageStatus.Sent : MessageStatus.Received;

            ChatMessage chatMessage = new ChatMessage
            {
                Message = message,
                SenderId = Context.User.Identity.GetUserId(),
                ReceiverId = targetUserId,
                MessageStatus = messageStatus
            };

            DbCtx.ChatMessages.Add(chatMessage);
            
            if (DbCtx.SaveChanges() > 0)
            {
                var senderConnections = DbCtx.ChatConnections.Where(c => c.UserId == chatMessage.SenderId).Select(c => c.Id).ToList();

                Clients.Clients(targetUserConnections).displayIncomingMessage(Context.User.Identity.GetUserName(), message);
                Clients.Clients(senderConnections).displayOutgoingMessage(Context.User.Identity.GetUserName(), message, messageStatus.ToString(), chatMessage.Id);
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

        public void MarkReceivedMessagesAsRead()
        {
            string currentUserId = Context.User.Identity.GetUserId();

            var messages = DbCtx.ChatMessages.Where(c => c.ReceiverId == currentUserId).ToList();
            messages.ForEach(m => m.MessageStatus = MessageStatus.Read);

            if (messages.Count != 0)
            {
                if (DbCtx.SaveChanges() > 0)
                {
                    var messageSenders = messages.Select(m => m.SenderId).ToList();
                    var targetUserConnections = DbCtx.ChatConnections.Where(c => messageSenders.Contains(c.UserId)).
                        Select(c => c.Id).ToList();

                    foreach (var message in messages)
                    {
                        Clients.Clients(targetUserConnections).changeMessageStatus(message.Id, MessageStatus.Read.ToString());
                    }
                }
            }
        }

        // Gets called on window "beforeunload" because sometimes "OnDisconnected" isn't called when closing the browser.
        public void RemoveCurrentConnection()
        {
            var chatConnection = DbCtx.ChatConnections.FirstOrDefault(c => c.Id == Context.ConnectionId);

            if (chatConnection != null)
            {
                DbCtx.ChatConnections.Remove(chatConnection);
                DbCtx.SaveChanges();
            }
        }
    }
}