using ITI.MVC.ChatService.Models.Entities;
using ITI.MVC.ChatService.Models.Enums;
using ITI.MVC.ChatService.Models.Store;
using ITI.MVC.ChatService.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITI.MVC.ChatService.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private ApplicationDbContext dbCtx;

        public ApplicationDbContext DbCtx
        {
            get => dbCtx ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            
            private set => dbCtx = value;
        }

        // GET: Chat
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();

            var users = DbCtx.Users.Where(u => u.Id != currentUserId).ToList();

            var messageContainerViewModel = users.Count != 0 ? new MessageContainerViewModel
            {
                IsFirstUser = true,
                CurrentUser = DbCtx.Users.Find(User.Identity.GetUserId()),
                ChatMessages = GetCurrentUserMessagesWith(users[0].Id),
                Receiver = users[0] ?? null
            } : new MessageContainerViewModel();

            return View(new ChatPageViewModel
            {
                Users = users,
                MessageContainerViewModel = messageContainerViewModel
            });
        }

        // Returns a partial view with the messages between the current user and the target user.
        // Requested through Ajax.
        [HttpGet]
        public PartialViewResult Messages(string targetUserId)
        {
            var messageContainerViewModel = new MessageContainerViewModel
            {
                IsFirstUser = true,
                CurrentUser = DbCtx.Users.Find(User.Identity.GetUserId()),
                ChatMessages = GetCurrentUserMessagesWith(targetUserId),
                Receiver = DbCtx.Users.Find(targetUserId)
            };

            return PartialView("_Messages", messageContainerViewModel);
        }

        // TODO: Transfer this function to signalR.
        [HttpPost]
        public bool SendMessage(string message, string targetUserId)
        {
            if (message == null || targetUserId == null || message == "" || targetUserId == "")
            {
                return false;
            }

            var chatMessage = new ChatMessage
            {
                Message = message,
                SenderId = User.Identity.GetUserId(),
                ReceiverId = targetUserId,
                Time = DateTime.Now,
                MessageStatus = MessageStatus.Sent
            };

            DbCtx.ChatMessages.Add(chatMessage);

            return DbCtx.SaveChanges() > 0;
        }

        #region Helper Functions

        /// <summary>
        /// Gets the messages between the current user and the target user.
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <returns>A list of chat messages</returns>
        private List<ChatMessage> GetCurrentUserMessagesWith(string targetUserId)
        {
            string currentUserId = User.Identity.GetUserId();
            return DbCtx.ChatMessages.
                Where(c =>
                (c.ReceiverId == targetUserId && c.SenderId == currentUserId) ||
                (c.ReceiverId == currentUserId && c.SenderId == targetUserId)).
                OrderBy(c => c.Time).
                ToList();
        }

        #endregion
    }
}